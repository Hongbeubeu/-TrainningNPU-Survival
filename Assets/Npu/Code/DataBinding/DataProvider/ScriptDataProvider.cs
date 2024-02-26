using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{
    public class ScriptDataProvider : MonoBehaviour, IDataProvider
    {
        public event Action<IDataProvider, int> DataChanged;
        public string type;

        public object GetData()
        {
            return null;
        }

        public bool Ready => true;
        public Type GetDataType()
        {
            return type == null ? null : Type.GetType(type);
        }

        static string[] _bindingFilters = {"default"};
        public string[] BindingFilters => _bindingFilters;

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ScriptDataProvider))]
    public class ScriptDataProviderEditor : Editor
    {
        string[] options;

        private void OnEnable()
        {
            options = GetAllTypes().Select(i => i.ToString()).ToArray();
            Array.Sort(options);
        }

        public override void OnInspectorGUI()
        {
            var t = target as ScriptDataProvider;

            var selected = Array.IndexOf(options, t.type);
            var selection = EditorGUILayout.Popup("Type", selected, options);
            if (selected != selection)
            {
                t.type = options[selection];
                EditorUtility.SetDirty(t);
            }
        }

        public static List<Type> GetAllTypes()
        {
            var res = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                res.AddRange(assembly.GetTypes());
            }

            var x = new Func<string, bool>(name => !name.Contains("unity") && !name.Contains("system.")
                                                                           && !name.Contains("mono.") &&
                                                                           !name.Contains("mono.") &&
                                                                           !name.Contains("icsharpcode.")
                                                                           && !name.Contains("nsubstitute") &&
                                                                           !name.Contains("nunit.") &&
                                                                           !name.Contains("microsoft.")
                                                                           && !name.Contains("boo.") &&
                                                                           !name.Contains("serializ") &&
                                                                           !name.Contains("json")
                                                                           && !name.Contains("log.") &&
                                                                           !name.Contains("logging") &&
                                                                           !name.Contains("test")
                                                                           && !name.Contains("editor") &&
                                                                           !name.Contains("debug") &&
                                                                           !name.Contains("ms.internal")
                                                                           && !name.Contains("privateimplementation") &&
                                                                           !name.Contains("jetbrains")
                                                                           && !name.Contains("firebase") &&
                                                                           !name.Contains("excss")
                                                                           && !name.Contains("<") &&
                                                                           !name.Contains("google")
                                                                           && !name.Contains("tmpro") &&
                                                                           !name.Contains("cdi.")
                                                                           && !name.Contains("dg.") &&
                                                                           !name.Contains("facebookgames") &&
                                                                           !name.Contains("+"));

            return res.Where(i => x(i.ToString().ToLower())).ToList();
        }
    }
#endif

}