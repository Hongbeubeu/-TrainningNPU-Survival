using System.Linq;
using Npu.Helper;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Npu.EditorSupport;
using UnityEditor;
#endif

namespace Npu.Formula
{
    public class ParameterViewer : MonoBehaviour
    {
        [TypeConstraint(typeof(IParameterContainer))] public Object container;
        
    }    

#if UNITY_EDITOR
    [CustomEditor(typeof(ParameterViewer))]
    public class ParameterViewerEditor : UnityEditor.Editor
    {
        private ParameterViewer Target => target as ParameterViewer;
        
        Dictionary<string, bool> states = new Dictionary<string, bool>();
        
        private static Dictionary<Type, Dictionary<string, bool>> UniversalStateMap = new Dictionary<Type, Dictionary<string, bool>>();

        private static Dictionary<string, bool> FindOrCreate(IParameterContainer container)
        {
            if (UniversalStateMap.TryGetValue(container.GetType(), out var states)) return states;
            states = new Dictionary<string, bool>();
            UniversalStateMap[container.GetType()] = states;
            return states;
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUIUtils.HorizontalSeparator();

            var container = Target.container as IParameterContainer;
            if (container == null) return;
            
            Draw(container);
        }

        public static void Draw(IParameterContainer container)
        {
            var states = FindOrCreate(container);
            var keys = container.Keys;
            foreach (var i in keys)
            {
                var p = container.Get(i);
                using (new VerticalHelpBox())
                using (new GuiColor(p == null ? Color.yellow : Color.white))
                {
                    if (p != null) Display(p, 1, p.Name, states);
                    else EditorGUILayout.LabelField($"No parameter {i}");
                }
            }
        }
        
        public static void DrawCompact(IParameterContainer container, ref string selection)
        {
            var states = FindOrCreate(container);
            var keys = container.Keys;
            var index = Array.IndexOf(keys, selection);
            index = EditorGUILayout.Popup("Select", index, keys);

            selection = keys.TryGet(index);
            var p = container.Get(selection ?? "");
            using (new VerticalHelpBox())
            using (new GuiColor(p == null ? Color.yellow : Color.white))
            {
                if (p != null) Display(p, 1, p.Name, states);
                else EditorGUILayout.LabelField($"No parameter {selection}");
            }
        }
        
        private static List<string> GetViewings(IParameterContainer container) 
            =>  EditorPrefs.GetString($"{container.GetType().Name}Editor_Viewings", "").Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
        private static void SetViewings(IParameterContainer container, List<string> viewings) 
            =>  EditorPrefs.SetString($"{container.GetType().Name}Editor_Viewings", string.Join(",", viewings));
        
        public static void DrawDynamic(IParameterContainer container)
        {
            var states = FindOrCreate(container);
            var viewings = GetViewings(container);
            var removed = new List<string>();
            foreach (var i in viewings)
            {
                var p = container.Get(i);
                using (new HorizontalHelpBox())
                {
                    using (new VerticalLayout())
                    using (new GuiColor(p == null ? Color.yellow : Color.white))
                    {
                        if (p != null) Display(p, 1, p.Name, states);
                        else EditorGUILayout.LabelField($"No parameter {i}");
                    }

                    if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        removed.Add(i);
                    }
                }
            }

            if (removed.Count > 0)
            {
                viewings.RemoveAll(i => removed.Contains(i));
                SetViewings(container, viewings);
            }
            using (new HorizontalLayout())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(40)))
                {
                    var menu = new GenericMenu();
                    var others = container.Keys.Where(i => !viewings.Contains(i));
                    foreach (var i in others)
                    {
                        menu.AddItem(new GUIContent(i), false, () => 
                        { 
                            viewings.Add(i);
                            SetViewings(container, viewings);
                        });
                    }
                    menu.ShowAsContext();
                }
                
            }

        }
        
        public static void Display(IParameter parameter, int indent, string key, Dictionary<string, bool> states)
        {
            const string valueFormat = "0.##";
            var label = $"{parameter.Name} [{parameter.GetType().Name}] ({parameter.Value.ToString(valueFormat)})";

            EditorGUI.indentLevel = indent;
            switch (parameter)
            {
                case AbstractEvaluator evaluator:
                {
                    states.TryGetValue(key, out var fold);
                    states[key] = EditorGUILayout.Foldout(fold, label, true);
                    if (states[key])
                    {
                        foreach (var p in evaluator.Parameters)
                        {
                            Display(p, indent + 1, key + p.Name, states);
                        }
                    }

                    break;
                }
                case BridgeParameter p:
                {
                    if(p.Source != null)
                    { 
                        states.TryGetValue(key, out var fold);
                        states[key] = EditorGUILayout.Foldout(fold, label, true);
                        if (states[key])
                            Display(p.Source, indent + 1, key + parameter.Name, states);
                    }
                    break;
                }
                case TwoParamsEvaluator pp:
                {
                    states.TryGetValue(key, out var fold);
                    states[key] = EditorGUILayout.Foldout(fold, label, true);
                    if (states[key])
                    {
                        Display(pp.A, indent + 1, key + pp.A.Name, states);
                        Display(pp.B, indent + 1, key + pp.B.Name, states);
                    }

                    break;
                }
                case TernaryEvaluator pp:
                {
                    states.TryGetValue(key, out var fold);
                    states[key] = EditorGUILayout.Foldout(fold, label, true);
                    if (states[key])
                    {
                        Display(pp.Condition, indent + 1, key + pp.Condition.Name, states);
                        Display(pp.A, indent + 1, key + pp.A.Name, states);
                        Display(pp.B, indent + 1, key + pp.B.Name, states);
                    }

                    break;
                }
                default:
                    EditorGUILayout.LabelField(label);
                    break;
            }

            EditorGUI.indentLevel = indent;
        }
    }
#endif

    
}