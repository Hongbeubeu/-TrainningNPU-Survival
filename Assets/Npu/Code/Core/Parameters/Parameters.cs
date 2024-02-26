using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Npu.Core;
using Npu.Helper;

#if UNITY_EDITOR
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu.Formula
{
    public interface IParameterContainer
    {
        string[] Keys { get; }
        IParameter Get(string key);
    }

    public interface IConditionalParameterActivator
    {
        void ConditionalParametersActivation();
    }

    public class ParametersSettingAttribute : Attribute
    {
        public bool inherit = true;
    }

    public class Parameters : ScriptableObject, IParameterContainer, IConditionalParameterActivator
    {
        [SerializeField] protected new string name;
        
        [SerializeField, Box] protected Activator[] activators;
        [SerializeField, Box] protected ConditionalActivator[] _conditionalActivators;
        
        protected readonly Dictionary<string, IParameter> parameters = new Dictionary<string, IParameter>();

#if UNITY_EDITOR
        public string[] Keys => AttributedParameterNames(GetType(), Inherit);
#else
        public string[] Keys => parameters.Keys.ToArray();
#endif
        public string Name => name;
#if UNITY_EDITOR        
        public bool Inherit => GetType().GetCustomAttribute<ParametersSettingAttribute>(false)?.inherit ?? true;
#else
        public bool Inherit { get; private set; }

#endif

        public IParameter Get(string key) => parameters.TryGetValue(key, out var p) ? p : null;
        

        public virtual void Setup()
        {
#if !UNITY_EDITOR            
            var att = GetType().GetCustomAttribute<ParametersSettingAttribute>(false);
            Inherit = att?.inherit ?? true;
#endif
            
            parameters.Clear();
            ProcessAttributes();
            ProcessAutoBind();
            ProcessAutoAssign();
            
            foreach (var i in activators) i.Setup();
            foreach (var i in _conditionalActivators) i.Setup();
            
        }
        
        public void Activate(bool active)
        {
            foreach (var i in activators)
            {
                i.Activate(active);
            }
        }
        
        [ContextMenu("Activate Conditionals")]
        public void ConditionalParametersActivation()
        {
            foreach (var i in _conditionalActivators)
            {
                i.Begin();
            }
        }

        [ContextMenu("Deactivate Conditionals")]
        public void DeactivateConditionalActivators()
        {
            foreach (var i in _conditionalActivators)
            {
                i.ActivateSilent(false);
            }
        }

        public virtual void TearDown()
        {
            foreach (var i in activators) i.TearDown();
            foreach (var i in _conditionalActivators) i.TearDown();
        }

        public void Prefix(string prefix, bool propertyOnly=false)
        {
            if (propertyOnly)
            {
                var ps = GetType().GetInstanceAttributedProperties<AutoBindAttribute>().Select(i => i.property);
                foreach (var p in ps)
                {
                    var v = p.GetValue(this);
                    if (v is IParameter pp)
                    {
                        pp.Name = $"[{prefix}] {pp.Name}";
                    }
                }
            }
            else
            {
                foreach (var p in parameters)
                {
                    p.Value.Name = $"[{prefix} {p.Value.Name}]";
                }
            }
        }
        
        public void Inject(object target)
        {
            var ps = target.GetType().GetInstanceAttributedProperties<InjectParameterAttribute>();
            InjectProperties(ps, target);
            
            var fs = target.GetType().GetInstanceAttributedFields<InjectParameterAttribute>();
            InjectFields(fs, target);
        }
        
        public void AssignFrom(object container)
        {
            var fs = container.GetType().GetInstanceAttributedFields<AutoAssignAttribute>();
            foreach (var (field, attribute) in fs)
            {
                if (field.FieldType != typeof(SecuredDouble))
                {
                    Logger.Error(this, container.GetType().Name, $"Field {field.Name} cannot be assigned");
                    continue;
                }

                var p = Get(attribute.name);
                if (p == null)
                {
                    Logger.Error(this, GetType().Name, $"Cannot find parameter {attribute.name}");
                    continue;
                }

                p.Value = (SecuredDouble) field.GetValue(container);
            }
        }

        private void InjectProperties(List<(PropertyInfo property, InjectParameterAttribute attribute)> ps, object target)
        {
            foreach (var i in ps)
            {
                var o = i.property.GetValue(target);
                
                var p = Get(i.attribute.name);
                if (p == null)
                {
                    Logger.Error(GetType().ToString(), "Unable to bind to {0} ({1}.{2}): No parameter {3} found",
                        target, target.GetType().ToString(), i.property.Name, i.attribute.name);
                    continue;
                }

                try
                {
                    i.property.SetValue(target, p, null);
                    // Logger._Log(GetType().ToString() + ".Inject", "{0} => {1}.{2}", i.attribute.name, target.GetType(), i.property.Name);
                }
                catch (Exception e)
                {
                    Logger.Error(GetType().ToString(), "Exception binding to {0} ({1}.{2}): {3}",
                        target, target.GetType().ToString(), i.property.Name, e.Message);
                }
            }
        }

        private void InjectFields(List<(FieldInfo field, InjectParameterAttribute attribute)> fs, object target)
        {
            foreach (var i in fs)
            {
                var o = i.field.GetValue(target);
                
                var p = Get(i.attribute.name);
                if (p == null)
                {
                    Logger.Error(GetType().ToString(), "Unable to bind to {0} ({1}.{2}): No parameter {3} found",
                        target, target.GetType().ToString(), i.field.Name, i.attribute.name);
                    continue;
                }

                try
                {
                    i.field.SetValue(target, p);
                    // Logger._Log(GetType().ToString() + ".Inject", "{0} => {1}.{2}", i.attribute.name, target.GetType(), i.field.Name);
                }
                catch (Exception e)
                {
                    Logger.Error(GetType().ToString(), "Exception binding to {0} ({1}.{2}): {3}",
                        target, target.GetType().ToString(), i.field.Name, e.Message);
                }
            }
        }
        private void ProcessAttributes()
        {
            var attributes = GetType().GetCustomAttributes(typeof(AbstractParameterAttribute), Inherit);
            var ats = new List<AbstractParameterAttribute>(attributes.Length);
            for (var i = 0; i < attributes.Length; i++)
            {
                var o = attributes[i] as AbstractParameterAttribute;
                var p = o.Create();
                if (p == null)
                {
                    Debug.LogErrorFormat("[{0}] Cannot create parameter with key {1}", o.GetType(), o.Name);
                    continue;
                }

                if (AddParameter(o.Name, p))
                {
                    ats.Add(o);
                }
                else
                {
                    Debug.LogErrorFormat("Parameter with key {0} already existed", o.Name);
                }
            }

            foreach (var p in ats)
            {
                p.DoPostProcess(Get(p.Name), this);
            }
        }

        private bool AddParameter(string key, IParameter param)
        {
            if (parameters.ContainsKey(key))
            {
                return false;
            }

            parameters.Add(key, param);
            return true;
        }

        private void ProcessAutoBind()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var info in properties)
            {
                var atts = info.GetCustomAttributes(typeof(AutoBindAttribute), false);
                if (atts.Length > 0)
                {
                    Bind(info, atts[0] as AutoBindAttribute);
                }
            }
        }

        private void ProcessAutoAssign()
        {
            AssignFrom(this);
        }

        private void Bind(PropertyInfo info, AutoBindAttribute attribute)
        {
            var p = Get(attribute.Name);
            if (p != null)
            {
                try
                {
                    info.SetValue(this, p, null);
                }
                catch (ArgumentException e)
                {
                    Debug.LogErrorFormat("Error binding {0} ({1})", info.Name, e.ToString());
                }
            }

            if (!(info.GetValue(this, null) is IParameter v))
            {
                Debug.LogWarningFormat("Binding for {0} is null", info.Name);
            }
        }
        
        public static string[] AttributedParameterNames(Type type, bool inherit=true) => type.GetCustomAttributes(inherit)
            .OfType<AbstractParameterAttribute>()
            .Select(i => i.Name).ToArray();
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(Parameters), true)]
    public class ParametersEditor : UnityEditor.Editor
    {
        Dictionary<string, bool> states = new Dictionary<string, bool>();

        private Parameters Target => target as Parameters;
        
        public override void OnInspectorGUI()
        {
            using (new DisabledGui(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }
            
            EditorGUILayout.Space();
            DrawPropertiesExcluding(serializedObject, "activators", "_conditionalActivators", "m_Script");

            EditorGUIUtils.Header("Activators");
            using (new VerticalHelpBox())
            using (new Indent())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("activators"));
            }
            
            using (new VerticalHelpBox())
            using (new Indent())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_conditionalActivators"));
            }
            
            GUILayout.Space(30);
            EditorGUIUtils.Section("Preview", () =>
            {
                ParameterViewerEditor.DrawDynamic(Target);
            });

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}