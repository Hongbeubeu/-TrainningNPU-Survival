using System;
using System.Collections.Generic;
using System.Linq;
using Npu.EditorSupport;
using Npu.EditorSupport.Inspector;
using Npu.Helper;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Npu
{
    public partial class DataBinder
    {
        private void OnValidate()
        {
            if (Application.isPlaying) return;

            foreach (var t in targets)
            {
                t._BindStringFormat();
            }
        }

        [ContextMenu("Force Bind")]
        private void EditorOnlyForceRebind()
        {
            _targetInitialized = false;
            OptionalInit();
            Bind();
            Log();
        }

        [ContextMenu("Log")]
        private void Log()
        {
            Debug.LogFormat("Provider: {0}", provider);
            Debug.LogFormat("Provider Value: {0}", DataProvider?.GetData());
        }

        [ContextMenu("Add Debugger")]
        private void AddDebugger()
        {
            gameObject.AddComponent<DataBinderDebugger>();
        }

        [ContextMenu("Migrate To Children")]
        private void MigrateToChildObjects()
        {
            var groups = targets.GroupBy(t =>
            {
                if (t.target.Target is GameObject) return t.target.Target;
                else if (t.target.Target is Component c) return c.gameObject;
                return t.target.Target;
            });
            foreach (var g in groups)
            {
                if (g.Key is GameObject go && go != gameObject)
                {
                    if (UnityEditorInternal.ComponentUtility.CopyComponent(this))
                    {
                        if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(go))
                        {
                            var dbs = go.GetComponents<DataBinder>();
                            var clone = dbs.FirstOrDefault(db =>
                            {
                                return db.description == description &&
                                    db.flag == flag &&
                                    db.targets.Zip(targets, (t1, t2) =>
                                        t1.paths.SequenceEqual(t2.paths) &&
                                        t1.target.Target == t2.target.Target && t1.target.MemberName == t2.target.MemberName &&
                                        t1.converter.format == t2.converter.format && t1.converter.type == t2.converter.type && t1.converter.defaultValue == t2.converter.defaultValue
                                        ).All(b => b);
                            });
                            if (clone)
                            {
                                clone.targets = clone.targets
                                    .Where(t => t.target.Target == go || t.target.Target is Component c && c.gameObject == go)
                                    .Where(t => !(t.target.Target is GameObject && t.target.MemberName == "SetActive"))
                                    .ToArray();
                                EditorUtility.SetDirty(clone);
                                Debug.Log($"Migrated DataBinder {this} to {clone}", clone);
                            }
                        }
                    }
                }
            }
        }

    }
    
    [CustomPropertyDrawer(typeof(ProviderMemberSelectorAttribute))]
    public class ProviderMemberDrawer : MemberSelectorAttributeDrawer
    {
        protected override Type TargetType(SerializedProperty property)
        {
            var target = property.FindPropertyRelative("target").objectReferenceValue;
            return (target as IDataProvider)?.GetDataType();
        }
    }

    [CustomPropertyDrawer(typeof(MemberSelectorAttribute))]
    public class MemberSelectorAttributeDrawer : PropertyDrawer
    {

        private const float Spacing = 2;

        protected virtual Type TargetType(SerializedProperty property)
        {
            return property.FindPropertyRelative("target").objectReferenceValue?.GetType();
        }

        private string[] Options(SerializedProperty property)
        {
            var type = TargetType(property);

            if (type == null) return new string[0];

            var selectorAttribute = attribute as MemberSelectorAttribute;

            if (selectorAttribute.returnType != null)
            {
                var fields = ReflectionUtils.GetFields(type, selectorAttribute.returnType, true, true);
                var properties = ReflectionUtils.GetProperties(type, selectorAttribute.returnType, true, true, false);
                var methods = ReflectionUtils.GetInstanceMethods(type, selectorAttribute.returnType, false);
                return properties.Concat(methods).Concat(fields).ToArray();
            }

            if (selectorAttribute.parameterType != null)
            {
                var fields = ReflectionUtils.GetFields(type, selectorAttribute.parameterType, true, true);
                var properties = ReflectionUtils.GetInstanceProperties(type, selectorAttribute.parameterType, true, true, true);
                var methods = ReflectionUtils.GetInstanceMethodsWithParameter(type, selectorAttribute.parameterType, false);
                return properties.Concat(methods).Concat(fields).ToArray();
            }

            return new string[0];
        }

        private static GenericMenu CreatMenu(UnityEngine.Object o, Action<UnityEngine.Object> select)
        {
            var m = new GenericMenu();

            var go = o as GameObject;
            if (o is Component c)
            {
                go = c.gameObject;
            }

            if (!go) return m;

            m.AddItem(new GUIContent(go.name + "(GameObject)"), o == go, () => @select(go));
            var ps = go.GetComponents(typeof(Component));
            foreach (var i in ps)
            {
                m.AddItem(new GUIContent(i.GetType().ToString()), i == o, () => @select(i));
            }

            return m;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var msa = attribute as MemberSelectorAttribute;

            if (msa.box)
            {
                EditorGUIUtils.Box(position);
                position = position.Extended(-Spacing);
            }

            var lw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = msa.labelWidth;

            position = EditorGUI.PrefixLabel(position, label);

            var targetProp = property.FindPropertyRelative("target");

            var (r1, r2) = position.HSplit(0.5f);
            var (r11, r12) = r1.HFixedSplit(r1.width - 25);

            EditorGUI.PropertyField(r11, targetProp, GUIContent.none);
            if (GUI.Button(r12.Extended(0, 0, 0, -5), "..."))
            {
                CreatMenu(targetProp.objectReferenceValue,
                    v =>
                    {
                        targetProp.objectReferenceValue = v;
                        targetProp.serializedObject.ApplyModifiedProperties();
                    }
                ).ShowAsContext();
            }

            var options = Options(property);
            var member = property.FindPropertyRelative("member").stringValue;

            var selected = Array.IndexOf(options, member);
            var selection = EditorGUI.Popup(r2, "", selected, options);
            if (selected != selection)
            {
                property.FindPropertyRelative("member").stringValue = options[selection];
            }

            EditorGUIUtility.labelWidth = lw;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var msa = attribute as MemberSelectorAttribute;
            return base.GetPropertyHeight(property, label) + (msa.box ? 2 * Spacing : 0);
        }
    }

    [CustomEditor(typeof(DataBinder))]
    public class DataBinderEditor : BaseInspector<DataBinder>
    {
        private List<MonoBehaviour> Providers => Target.gameObject.GetComponentsInParent<IDataProvider>(true)
            .OfType<MonoBehaviour>()
            .ToList();

        private string[] ProviderNames => Providers.Select(i => $"{i.gameObject.name} ({i.GetType()})").ToArray();

        private GenericMenu ProviderMenu(UnityEngine.Object selected, Action<UnityEngine.Object> select)
        {
            var p = Providers;
            var ps = ProviderNames;

            var m = new GenericMenu();
            for (var i = 0; i < p.Count; i++)
            {
                var n = ps[i];
                var o = p[i];
                m.AddItem(new GUIContent(n), selected == p[i],
                    () =>
                    {
                        select(o);
                    }
                );
            }
            return m;
        }

        private static string[] Options(Type type)
        {
            if (type == null) return new string[0];
            var fields = ReflectionUtils.GetFields(type, typeof(object), true, true);
            var properties = ReflectionUtils.GetProperties(type, typeof(object), true, true, false);
            var methods = ReflectionUtils.GetInstanceMethods(type, typeof(object), false);
            return properties.Concat(methods).Concat(fields).ToArray();
        }

        private void MoveTarget(int index, int dir)
        {
            var props = serializedObject.FindProperty("targets");
            props.MoveArrayElement(index, index + dir);
        }

        private void InsertTarget(int index)
        {
            var props = serializedObject.FindProperty("targets");
            props.InsertArrayElementAtIndex(index);
        }

        protected override void DrawInspectorGUI()
        {
            var binder = target as DataBinder;

            using (new GuiColor(Color.cyan))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
            }

            // EditorGUILayout.PropertyField(serializedObject.FindProperty("bindOnEnable"));

            var provider = binder.Provider as IDataProvider;

            var pp = serializedObject.FindProperty("provider");
            EditorGUILayout.PropertyField(pp);

            if (provider != null)
            {
                var flag = binder.Flag;
                binder.Flag = DrawBindingFlag(provider.BindingFilters, flag);
                if (binder.Flag != flag)
                    EditorUtility.SetDirty(binder);
            }

            using (new VerticalHelpBox())
            using (new Indent())
            {
                var props = serializedObject.FindProperty("targets");
                using (new HorizontalLayout())
                {
                    props.isExpanded = EditorGUILayout.Foldout(props.isExpanded,
                        $"Bindings ({props.arraySize})", true);

                    using (new GuiColor(Color.cyan))
                        if (GUILayout.Button("+", GUILayout.Width(40)))
                        {
                            props.InsertArrayElementAtIndex(props.arraySize);
                        }
                }

                if (props.isExpanded)
                {
                    var type = (target as DataBinder).ProviderDataType;

                    EditorGUI.indentLevel++;
                    for (var i = 0; i < props.arraySize; i++)
                    {
                        if (!DrawTarget(props.GetArrayElementAtIndex(i), type, i)) continue;

                        props.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    EditorGUI.indentLevel--;
                }

            }

            serializedObject.ApplyModifiedProperties();
        }

        private static string Description(SerializedProperty property)
        {
            var array = property.FindPropertyRelative("paths");

            var paths = new List<string>();
            for (var i = 0; i < array.arraySize; i++)
            {
                paths.Add(array.GetArrayElementAtIndex(i).stringValue);
            }

            var target = property.FindPropertyRelative("target").FindPropertyRelative("target").objectReferenceValue;
            var member = property.FindPropertyRelative("target").FindPropertyRelative("member").stringValue;

            return $"{string.Join(".", paths.ToArray())} → {(target != null ? $"{target}.{member}" : "")}";
        }

        private bool DrawTarget(SerializedProperty property, Type type, int index)
        {
            var options = Options(type);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            using (new HorizontalLayout())
            {
                using (new GuiColor(Color.cyan))
                {
                    if (GUILayout.Button("↑", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        MoveTarget(index, -1);
                    }

                    if (GUILayout.Button("↓", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        MoveTarget(index, 1);
                    }

                    if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        InsertTarget(index);
                    }
                }

                // using (new GuiColor(Color.red))
                {
                    var style = new GUIStyle(GUI.skin.button);
                    style.normal.textColor = Color.red;
                    if (GUILayout.Button("X", style, GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        EditorGUILayout.EndVertical();
                        return true;
                    }
                }

                EditorGUILayout.LabelField(Description(property), EditorStyles.boldLabel);
            }


            // Paths
            using (new HorizontalLayout())
            {
                using (new LabelWidth(100))
                {
                    EditorGUILayout.LabelField("Path");
                }

                var array = property.FindPropertyRelative("paths");

                for (var i = 0; i < array.arraySize; i++)
                {
                    type = DrawPath(type, array, i);
                }

                using (new GuiColor(Color.cyan))
                {
                    if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        array.InsertArrayElementAtIndex(array.arraySize);
                    }

                    if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)) && array.arraySize > 0)
                    {
                        array.DeleteArrayElementAtIndex(array.arraySize - 1);
                    }
                }
            }

            EditorGUILayout.PropertyField(property.FindPropertyRelative("target"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("converter"));
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
            return false;
        }

        private static Type DrawPath(Type type, SerializedProperty array, int index)
        {
            var lw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;

            var options = Options(type);
            var property = array.GetArrayElementAtIndex(index);
            var value = property.stringValue;

            var rect = GUILayoutUtility.GetRect(new GUIContent(value), GUIStyle.none);
            var dropDown = false;
            using (new GuiColor(options.Contains(value) ? Color.white : Color.yellow))
            {
                dropDown = EditorGUI.DropdownButton(rect, new GUIContent(value), FocusType.Passive);
            }
            if (dropDown)
            {
                var menu = new GenericMenu();
                options.ForEach(i => menu.AddItem(new GUIContent(i), i == value, data =>
                {
                    property.stringValue = (string)data;
                    property.serializedObject.ApplyModifiedProperties();
                }, i));
                menu.DropDown(rect);
            }

            EditorGUIUtility.labelWidth = lw;
            var w = new MemberInfoFacade();
            w.Init(type, property.stringValue, array.serializedObject.targetObject);
            return w.ReturnType;
        }

        private bool flagFoldout;

        private int DrawBindingFlag(string[] options, int value)
        {
            void Draw()
            {
                EditorGUI.indentLevel++;

                for (var i = 0; i < options.Length; i++)
                {
                    if (i % 2 == 0) EditorGUILayout.BeginHorizontal();

                    var selected = ((1 << i) & value) != 0;
                    var style = new GUIStyle(GUI.skin.toggle)
                    {
                        normal = { textColor = selected ? Color.cyan : Color.white }
                    };
                    if (EditorGUILayout.Toggle(options[i], selected, style))
                    {
                        value |= 1 << i;
                    }
                    else
                    {
                        value &= ~(1 << i);
                    }

                    if (i % 2 == 1 || i == options.Length - 1) EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            using (new Indent())
            using (new VerticalHelpBox())
            {
                using (new HorizontalLayout())
                {
                    flagFoldout = EditorGUILayout.Foldout(flagFoldout, $"Filter ({value})", true);

                    using (new GuiColor(Color.cyan))
                        if (GUILayout.Button("ALL", GUILayout.Width(60)))
                        {
                            value = All(options);
                        }

                    using (new GuiColor(Color.cyan))
                        if (GUILayout.Button("NONE", GUILayout.Width(60)))
                        {
                            value = 0x00;
                        }

                }

                if (flagFoldout) Draw();
            }


            return value;
        }

        private static int All(string[] options)
        {
            var s = 0;
            for (var i = 0; i < options.Length; i++) s |= 1 << i;
            return s;
        }
    }
}

#endif