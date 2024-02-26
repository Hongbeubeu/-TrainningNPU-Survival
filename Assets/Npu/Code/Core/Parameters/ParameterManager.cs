using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using Npu.EditorSupport;
using UnityEditor;
#endif

namespace Npu.Formula
{
    [CreateAssetMenu(fileName = "ParameterManager", menuName = "Parameters/Manager", order = 0)]
    public class ParameterManager : ScriptableObject
    {
        [SerializeField] protected Parameters[] parameters;

        public Parameters[] Parameters => parameters;

        //[Resolvable] public VideoBoostParameters VideoBoostParameters => First<VideoBoostParameters>();
        
        
        public void Setup()
        {
            foreach (var i in parameters)
            {
                i.Setup();
            }
        }

        public void Begin()
        {
            foreach (var i in parameters)
            {
                i.Activate(true);
            }
        }

        public void TearDown()
        {
            foreach (var i in parameters)
            {
                i.TearDown();
            }
        }

        private T First<T>() where T : Parameters => parameters.First(i => i is T) as T;
        
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ParameterManager))]
    public class ParameterManagerEditor : UnityEditor.Editor
    {
        private ParameterManager Target => target as ParameterManager;
        
        Dictionary<string, bool> states = new Dictionary<string, bool>();
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUIUtils.HorizontalSeparator();

            var calculators = Target.Parameters;

            foreach (var i in calculators)
            {
                if (i == null) continue;
                
                EditorGUIUtils.Section(i.Name,
                    () =>
                    {
                        foreach (var e in i.Keys)
                        {
                            var p = i.Get(e);
                            using (new GuiColor(p == null ? Color.red : Color.white))
                            using (new VerticalHelpBox())
                            {
                                if (p != null) ParameterViewerEditor.Display(p, 1, p.Name, states);
                                else EditorGUILayout.LabelField($"{e}");
                            
                            }
                        }
                    },  keyPrefix: "ppm");
            }

        }
    }
#endif
}