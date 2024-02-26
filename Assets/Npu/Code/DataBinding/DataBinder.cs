using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using Npu.Helper;
using UnityEditor;
using Npu.EditorSupport;
using Npu.EditorSupport.Inspector;

#endif

namespace Npu
{

    public class BinderValueAttribute : Attribute
    {

    }
    
    public class ProviderMemberSelectorAttribute : MemberSelectorAttribute
    {
        public ProviderMemberSelectorAttribute(Type returnType = null, Type parameterType = null) : base(returnType, parameterType) { }
    }

    public class MemberSelectorAttribute : PropertyAttribute
    {
        public Type returnType;
        public Type parameterType;
        public float labelWidth;
        public bool box;

        public MemberSelectorAttribute(Type returnType = null, Type parameterType = null, float labelWidth = 90)
        {
            this.returnType = returnType;
            this.parameterType = parameterType;
        }
    }

    public partial class DataBinder : MonoBehaviour
    {
        public const int FlagAll = 0xffff;
        public const int FlagNone = 0x00;

        [SerializeField, HideInInspector] private string description;
        [SerializeField, HideInInspector] private int flag = 0x01;
        [SerializeField, TypeConstraint(typeof(IDataProvider)), HideInInspector] private UnityEngine.Object provider;
        [SerializeField, HideInInspector] private BindingTarget[] targets = new BindingTarget[0];

        public UnityEngine.Object Provider => provider;
        public IEnumerable<BindingTarget> Targets => targets;
        private IDataProvider DataProvider { get; set; }

        public Type ProviderDataType => (provider as IDataProvider)?.GetDataType();

        public int Flag
        {
            get => flag;
            set => flag = value;
        }

        public bool IsLateBind { get; private set; }
        public bool IsDataDirty { get; private set; }

        private void Awake()
        {
            InitLateBind();
        }

        private void OnEnable()
        {
            if (provider == null) return;

            if (!DataBinderManager.Register(this))
            {
                Logger._Error<DataBinder>($"({gameObject.name}) Unable to Register to Manager");
                IsLateBind = false;
            }

            OptionalInit();
            if (DataProvider.Ready) Bind();
        }

        private void OnDisable()
        {
            DataBinderManager.Unregister(this);
            if (DataProvider != null) DataProvider.DataChanged -= OnDataChanged;
        }

        private bool _targetInitialized;
        private void OptionalInit()
        {
            if (!_targetInitialized)
            {
                foreach (var i in targets)
                {
                    i.Init(ProviderDataType, this);
                }
                _targetInitialized = true;
            }

            if (DataProvider != null) DataProvider.DataChanged -= OnDataChanged;
            DataProvider = provider as IDataProvider;
            if (DataProvider != null) DataProvider.DataChanged += OnDataChanged;
        }

        private void OnDataChanged(IDataProvider dataProvider, int flags)
        {
            if (dataProvider == null) return;
            
            if ((flag & flags) != 0)
            {
                MarkDataDirty(true);
            }

            if (!IsLateBind)
            {
                Bind(flags, dataProvider.GetData());
            }
            else
            {
                DataBinderManager.OnBindDelayed(this);
            }
        }
        
        public void Bind()
        {
            if (DataProvider != null)
            {
                Bind(flag, DataProvider.GetData());
            }
        }

        private void Bind(int flags, object data)
        {
            if ((flag & flags) == 0) return;

            MarkDataDirty(false);
            foreach (var i in targets)
            {
                i.Bind(data);
            }
        }

        public void MarkDataDirty(bool dirty) => IsDataDirty = dirty;

        public void DoLateBind()
        {
            if (IsLateBind && IsDataDirty) Bind();
        }

        private static readonly List<DataBinderLateBindGroup> _lateBindGroups_Temp = new List<DataBinderLateBindGroup>();

        private void InitLateBind()
        {
            IsLateBind = false;
            _lateBindGroups_Temp.Clear();
            var p = transform;
            while (true)
            {
                p.GetComponents(_lateBindGroups_Temp);
                if (_lateBindGroups_Temp.Count > 0)
                {
                    var found = false;
                    for (var i = 0; i < _lateBindGroups_Temp.Count; i++)
                    {
                        var b = _lateBindGroups_Temp[i];
                        if (!b.enabled) continue;
                        
                        IsLateBind = b.LateBind;
                        found = true;
                        break;
                    }
                    
                    if (found) break;
                    
                    _lateBindGroups_Temp.Clear();
                }

                p = p.parent;
                if (p == null) break;
            }
        }
        

        [Serializable]
        public class ProviderMember : MemberSelector
        {
            public IDataProvider DataProvider => target as IDataProvider;
            public override Type TargetType => (target as IDataProvider)?.GetDataType();
            public override object Target => (target as IDataProvider)?.GetData();

            public ProviderMember(Object target, string member) : base(target, member)
            {
            }
            
            public Type MemberType
            {
                get
                {
                    FindMemberInfo();
                    return _memberInfoFacade.ReturnType;
                }
            }
        }


        [Serializable]
        public class BindingTarget
        {
            public string[] paths;
            [MemberSelector(parameterType = typeof(object), labelWidth = 80)] public MemberSelector target;
            public DataConverter converter;

            private MemberInfoChain _chain;

            public void Init(Type type, UnityEngine.Object context)
            {
                _chain = new MemberInfoChain();
                target.Setup();
                _chain.Init(type, paths, context);
            }

            public void Bind(object data)
            {
                data = _chain.GetValue(data);
                target.SetValue(converter.Convert(data));
            }
#if UNITY_EDITOR
            public void _BindStringFormat()
            {
                if (Application.isPlaying) return;

                if ((converter.type != DataConverter.Type.String &&
                     converter.type != DataConverter.Type.UpperCaseString &&
                     converter.type != DataConverter.Type.LowerCaseString &&
                     converter.type != DataConverter.Type.CapitalizedString) ||
                    string.IsNullOrEmpty(converter.format)) return;

                try
                {
                    target.Setup();
                    if (target.IsValid) target.SetValue(converter.format);
                }
                catch (Exception)
                {

                }
            }
#endif
        }

        private class MemberInfoChain
        {
            private List<MemberInfoFacade> _facades;

            public void Init(Type type, string[] path, UnityEngine.Object context)
            {
                _facades = new List<MemberInfoFacade>();
                for (var i = 0; i < path.Length; i++)
                {
                    var w = new MemberInfoFacade();
                    w.Init(type, path[i], context);
                    if (w.IsValid)
                    {
                        _facades.Add(w);
                        type = w.ReturnType;
                    }
                    else
                    {
                        Logger.Error<MemberInfoChain>(context, $"Invalid member {path[i]} of type {type} ({string.Join(".", path)})");
                        break;
                    }
                }
            }

            public object GetValue(object @object)
            {
                for (var i = 0; i < _facades.Count; i++)
                {
                    @object = _facades[i].GetValue(@object);
                    if (@object == null) break;
                }

                return @object;
            }
        }
    }

    
}