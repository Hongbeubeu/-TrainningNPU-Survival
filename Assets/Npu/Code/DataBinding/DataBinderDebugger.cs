using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu
{
    public class DataBinderDebugger : MonoBehaviour
    {
        public UnityEngine.Object provider;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DataBinderDebugger))]
    public class DataBinderDebuggerEditor : Editor
    {
        DataBinderDebugger Target => target as DataBinderDebugger;

        List<MonoBehaviour> Providers => Target.gameObject.GetComponents<IDataProvider>()
            .OfType<MonoBehaviour>()
            .ToList();

        string[] ProviderNames =>
            Providers.Select(i => string.Format("{0} ({1})", i.gameObject.name, i.GetType())).ToArray();

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            var pp = serializedObject.FindProperty("provider");
            using (new HorizontalLayout())
            {
                EditorGUILayout.PropertyField(pp);
                using (new GuiColor(Color.cyan))
                {
                    if (GUILayout.Button("...", GUILayout.Width(20)))
                    {
                        ProviderMenu(pp.objectReferenceValue,
                            o =>
                            {
                                pp.objectReferenceValue = o;
                                serializedObject.ApplyModifiedProperties();
                            }).ShowAsContext();
                    }
                }
            }

            if (pp.objectReferenceValue != null)
            {
                ShowDataBinders(pp.objectReferenceValue);
            }
        }

        void ShowDataBinders(UnityEngine.Object p)
        {
            var dataBinders = Target.GetComponentsInChildren<DataBinder>()
                .Where(d => d.Provider != null && d.Provider == p);

            if (!dataBinders.Any())
            {
                EditorGUILayout.Space();
                GUILayout.Label("No data binder have been found!");
            }

            var providerType = (p as IDataProvider).GetDataType();

            foreach (var dataBinder in dataBinders)
            {
                EditorGUILayout.Space();

                using (new GuiColor(EditorGUIUtility.isProSkin ? Color.white : Color.cyan))
                {
                    EditorGUILayout.BeginVertical("HelpBox");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Binder", GUIStyle.none, TextColor(HighlightTextColor()));
                    using (new GuiColor(Color.white))
                    {
                        EditorGUILayout.ObjectField(dataBinder, typeof(UnityEngine.Object), true);
                    }

                    EditorGUILayout.EndHorizontal();

                    foreach (var t in dataBinder.Targets)
                    {
                        using (new GuiColor(Color.white))
                        {
                            EditorGUILayout.BeginHorizontal("Box");

                            //path
                            var pathsColor = ValidatePaths(providerType, t.paths) ? DefaultTextColor() : Color.red;

                            GUILayout.Label(string.Join(".", t.paths), TextColor(pathsColor));
                            GUILayout.Label("→");

                            var converterColor = ValidateConverter(providerType, t) ? DefaultTextColor() : Color.red;

                            //convert
                            GUILayout.Label(t.converter.type.ToString(), TextColor(converterColor));
                            if (!string.IsNullOrEmpty(t.converter.format))
                            {
                                GUILayout.Label(t.converter.format, TextColor(converterColor));
                            }

                            GUILayout.Label("→");

                            //target
                            var targetColor = t.target.Target == null ? Color.red : Color.white;
                            using (new GuiColor(targetColor))
                            {
                                EditorGUILayout.ObjectField((UnityEngine.Object)t.target.Target, typeof(UnityEngine.Object), true);
                            }

                            if (t.target.Target != null)
                            {
                                var targetPathColor = ValidatePaths(t.target.TargetType, new[] {t.target.MemberName})
                                    ? DefaultTextColor()
                                    : Color.red;
                                GUILayout.Label($".{t.target.MemberName}", TextColor(targetPathColor));
                            }

                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }

        bool ValidatePaths(Type providerType, string[] paths)
        {
            var type = providerType;

            return paths.All(path =>
            {
                var w = new MemberInfoFacade();
                w.Init(type, path, null);
                type = w.ReturnType;
                return w.IsValid;
            });
        }

        bool ValidateConverter(Type providerType, DataBinder.BindingTarget t)
        {
            var sourceType = providerType;
            foreach (var path in t.paths)
            {
                var w = new MemberInfoFacade();
                w.Init(sourceType, path, null);
                sourceType = w.ReturnType;
                if (!w.IsValid)
                {
                    return false;
                }
            }

            var targetType = t.target.TargetType;
            var tw = new MemberInfoFacade();
            tw.Init(targetType, t.target.MemberName, null);
            if (!tw.IsValid)
            {
                return false;
            }

            var converter = t.converter;

            try
            {
                var sourceData = sourceType.IsValueType ? Activator.CreateInstance(sourceType) :
                    sourceType == typeof(string) ? string.Empty : null;
                var convertedData = converter.Convert(sourceData);

                GetFieldPropertyMethod(targetType, t.target.MemberName, out var fieldInfo, out var propertyInfo,
                    out var methodInfo);
                Type setType = null;
                if (fieldInfo != null)
                {
                    setType = fieldInfo.FieldType;
                }
                else if (propertyInfo != null)
                {
                    setType = propertyInfo.PropertyType;
                }
                else if (methodInfo != null)
                {
                    var ps = methodInfo.GetParameters();
                    if (ps.Length == 1)
                    {
                        setType = ps[0].ParameterType;
                    }
                }

                if (setType == null)
                {
                    return false;
                }

                if (convertedData != null)
                {
                    return setType.IsInstanceOfType(convertedData);
                }

                return !setType.IsValueType && setType != typeof(string);
            }
            catch
            {
                return false;
            }
        }

        void GetFieldPropertyMethod(Type t, string member, out FieldInfo fieldInfo, out PropertyInfo propertyInfo,
            out MethodInfo methodInfo)
        {
            fieldInfo = null;
            propertyInfo = null;
            methodInfo = null;

            if (t != null && !string.IsNullOrEmpty(member))
            {
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                fieldInfo = t.GetField(member, flags);
                if (fieldInfo == null)
                {
                    propertyInfo = t.GetProperty(member, flags);
                }

                if (fieldInfo == null && propertyInfo == null)
                {
                    try
                    {
                        methodInfo = t.GetMethod(member, flags);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogErrorFormat("{0} / {1} / {2}", t, member, ex);
                    }
                }
            }
        }

        GenericMenu ProviderMenu(UnityEngine.Object selected, Action<UnityEngine.Object> select)
        {
            var p = Providers;
            var ps = ProviderNames;

            var m = new GenericMenu();

            for (var i = 0; i < p.Count; i++)
            {
                var n = ps[i];
                var o = p[i];
                m.AddItem(new GUIContent(n), selected == p[i],
                    () => { select(o); }
                );
            }

            return m;
        }

        GUIStyle TextColor(Color color)
        {
            var style = new GUIStyle
            {
                normal = {textColor = color},
                richText = false
            };
            return style;
        }

        Color HighlightTextColor()
        {
            return EditorGUIUtility.isProSkin ? Color.cyan : Color.black;
        }

        Color DefaultTextColor()
        {
            return EditorGUIUtility.isProSkin ? HexColor("#B4B4B4") : Color.black;
        }

        Color HexColor(string s)
        {
            ColorUtility.TryParseHtmlString(s, out var c);
            return c;
        }
    }
#endif

}