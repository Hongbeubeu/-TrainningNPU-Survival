using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npu.Helper;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu.EditorSupport.Inspector
{
    public class InspectorGUIAttribute : Attribute
    {
        public string group;
        public string name;
        public int order;
        public bool gui;
        public bool runtimeOnly;
        public int expand = 1;
        public bool box = true;
    }
    
#if UNITY_EDITOR

    public class BaseInspector<TTarget> : BaseInspector where TTarget : class
    {
        protected TTarget Target => target as TTarget;
    }
    
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class BaseInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            DrawInspectorGUI();
            
            EditorGUILayout.Separator();
            DrawGroups();
            DrawContextMenu();
        }
        
        /// <summary>
        /// Override <see cref="DrawInspectorGUI"/> for custom inspector
        /// </summary>
        protected virtual void DrawInspectorGUI()
        {
            
        }

        protected void DrawGroups()
        {
            var groups = target.GetType().GetAttributedMethods<InspectorGUIAttribute>()
                .GroupBy(i => i.attribute.group)
                .OrderBy(i => i.Average(k => k.attribute.order));
            foreach (var g in groups) DrawGroup(g.Key, g);
        }

        protected void DrawGroup(string label, IEnumerable<(MethodInfo method, InspectorGUIAttribute attribute)> contents)
        {
            var m = contents.ToList();
            Section(label, () =>
            {
                DrawGroupInner(m);
            }, helpBox: m.All(i => i.attribute.box));
            EditorGUILayout.Space();
        }

        private void DrawGroupInner(List<(MethodInfo method, InspectorGUIAttribute attribute)> contents)
        {
            while (true)
            {
                const int itemPerRow = 5;
                var acc = 0;
                var i = 0;

                for (; i < contents.Count; i++)
                {
                    if (contents[i].attribute.expand + acc > itemPerRow) break;
                    acc += contents[i].attribute.expand;
                }

                var slice = contents.Take(i);
                using (new HorizontalLayout())
                {
                    foreach (var (m, a) in slice)
                    {
                        MethodGui(m, a);
                    }
                }

                var left = contents.Skip(i).ToList();
                if (left.Count > 0)
                {
                    contents = left;
                    continue;
                }

                break;
            }
        }

        private void MethodGui(MethodInfo method, InspectorGUIAttribute attribute)
        {
            using (new DisabledGui(!Application.isPlaying && attribute.runtimeOnly))
            {
                if (attribute.gui || GUILayout.Button(ObjectNames.NicifyVariableName(attribute.name ?? method.Name)))
                {
                    method.Invoke(target, new object[0]);
                }
            }
        }
        
        
        protected void DrawContextMenu()
        {
            var ctx = target.GetType().GetAttributedMethods<ContextMenu>().OrderBy(i => i.attribute.priority).ToList();
            if (ctx.Count == 0) return;
            Section("Context Menu", () =>
            {
                DoGUI(ctx, x => ContextGUI(x.method, x.attribute), 5);
            });
        }
        
        private void ContextGUI(MethodInfo info, ContextMenu attribute)
        {
            if (GUILayout.Button(attribute.menuItem))
            {
                info.Invoke(target, new object[0]);
            }
        }

        protected void DoGUI<TContent>(IEnumerable<TContent> contents, Action<TContent> gui, int itemPerRow)
        {
            var row = 0;
            using (new VerticalLayout())
            {
                while (true)
                {
                    var slice = contents.Skip(row++ * itemPerRow).Take(itemPerRow).ToList();
                    if (slice.Count == 0) break;

                    using (new HorizontalLayout())
                    {
                        foreach (var i in slice)
                        {
                            gui?.Invoke(i);
                        }
                    }       
                }
            }
        }

        protected void Section(string label, Action onGUI, TextAnchor alignment = TextAnchor.MiddleLeft, bool helpBox = true)
        {
            EditorGUIUtils.Section(label, onGUI, alignment, target.GetType().Name, helpBox);
        }
    }
#endif    
}
