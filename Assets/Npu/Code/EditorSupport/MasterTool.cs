using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu.Helper
{

    public class MasterTool : MonoBehaviour
    {
        public List<IToolProvider> Tools { get; private set; }
        public Dictionary<object, List<ToolMethod>> methods;

        private static readonly Type[] DontDestroyOnloadRoots =
        {
        };

#if UNITY_EDITOR
        private void Awake()
        {
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Setup();
        }
#endif

        [ContextMenu("Refresh")]
        public void Setup()
        {
            Tools = FindAll<IToolProvider>();
            Tools.Sort((x, y) => x.Order - y.Order);
            methods = new Dictionary<object, List<ToolMethod>>();
            foreach (var i in Tools)
            {
                methods[i] = FindMethods(i);
            }
        }

        public static List<T> FindAll<T>()
        {
            var results = new List<T>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    var roots = s.GetRootGameObjects();
                    for (var j = 0; j < roots.Length; j++)
                    {
                        var go = roots[j];
                        results.AddRange(go.GetComponentsInChildren<T>(true));
                    }
                }
            }

            foreach (var i in DontDestroyOnloadRoots)
            {
                var o = FindObjectOfType(i) as Component;
                if (o)
                {
                    results.AddRange(o.GetComponentsInChildren<T>(true));
                }
            }

            return results;
        }

        public List<ToolMethod> GetMethods(object target)
        {
            if (methods.TryGetValue(target, out var ms)) return ms;
            return null;
        }

        public IEnumerable<(string, IOrderedEnumerable<ToolMethod>)> GetGroups(object target)
        {
            var ms = GetMethods(target);

            var gs = ms?.GroupBy(i => i.attribute.group)
                .OrderBy(i => i.Average(k => k.attribute.order))
                .Select(i => (i.Key, i.OrderBy(k => k.attribute.order)));

            return gs;
        }

        private List<ToolMethod> FindMethods(object target)
        {
            var ms = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                 BindingFlags.NonPublic)
                .Select(i => (method: i, attribute: i.GetCustomAttributes<MethodAttribute>(true).FirstOrDefault()))
                .Where(i => i.attribute != null)
                .OrderBy(i => i.attribute.order)
                .Select(i => new ToolMethod
                {
                    attribute = i.attribute,
                    method = i.method,
                }).ToList();
            return ms;
        }

        public class ToolMethod
        {
            public MethodAttribute attribute;
            public MethodInfo method;

#if UNITY_EDITOR
            public string Label => attribute.name ?? ObjectNames.NicifyVariableName(method.Name);
#else
        public string Label => attribute.name;
#endif
            public void Invoke(object target) => method.Invoke(target, new object[0]);
        }

        public class MethodAttribute : Attribute
        {
            public string group;
            public string name;
            public int order;
            public bool gui;
            public bool runtimeOnly = true;

        }

        public interface IToolProvider
        {
            int Order { get; }
            string Label { get; }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MasterTool))]
    public class MasterToolEditor : Editor
    {
        MasterTool Target => target as MasterTool;

        private bool this[MasterTool.IToolProvider target]
        {
            get => EditorPrefs.GetBool($"master_tool_{target.GetType()}_{target.Label}");
            set => EditorPrefs.SetBool($"master_tool_{target.GetType()}_{target.Label}", value);
        }

        public override void OnInspectorGUI()
        {

            if (Target.Tools == null) Target.Setup();

            foreach (var i in Target.Tools)
            {
                using (new VerticalHelpBox())
                {
                    Show(i);
                }
            }
        }

        private void Show(MasterTool.IToolProvider toolProvider)
        {
            var margin = EditorStyles.foldout.margin;
            margin = new RectOffset(margin.left + 10, margin.right, margin.top, margin.bottom);

            var style = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = EditorStyles.boldFont.fontSize + 5,
                margin = margin
            };

            using (new HorizontalLayout())
            {
                this[toolProvider] = EditorGUILayout.Foldout(this[toolProvider], new GUIContent(toolProvider.Label), true, style);

                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    if (toolProvider is MonoBehaviour m)
                    {
                        EditorGUIUtility.PingObject(m.gameObject);
                    }
                }
            }

            if (this[toolProvider])
            {
                var gs = Target.GetGroups(toolProvider);
                foreach (var i in gs) ShowGroup(toolProvider, i);
            }
        }

        private void ShowGroup(MasterTool.IToolProvider target, (string, IOrderedEnumerable<MasterTool.ToolMethod>) group)
        {
            var playing = Application.isPlaying;

            using (new VerticalHelpBox())
            {
                if (!string.IsNullOrEmpty(group.Item1)) EditorGUILayout.LabelField(group.Item1, EditorStyles.boldLabel);

                var itemPerRow = 5;//string.IsNullOrEmpty(group.Item1) ? 5 : group.Item2.Count();
                var k = 0;
                while (true)
                {
                    var items = group.Item2.Skip(k * itemPerRow).Take(itemPerRow).ToList();

                    if (!items.Any()) break;
                    using (new HorizontalLayout())
                    {
                        foreach (var i in items)
                        {
                            using (new DisabledGui(!playing && i.attribute.runtimeOnly && !i.attribute.gui))
                            {
                                if (i.attribute.gui) i.Invoke(target);
                                else if (GUILayout.Button(i.Label)) i.Invoke(target);
                            }
                        }
                    }

                    k++;
                }
            }

            EditorGUILayout.Separator();
        }
    }
#endif

}