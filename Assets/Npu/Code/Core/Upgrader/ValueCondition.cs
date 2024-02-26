using UnityEngine;
using System;
using Npu.Helper;
#if UNITY_EDITOR
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu
{
    public interface IValueTrigger
    {
        event Action<int, double> ValueChanged;
        string[] CategoryNames { get; }
        double GetValue(int category);
        string GetDescription(int category, double value);
        (bool, string) Validate(int category, double value);

        /*
        #region IValueTrigger
        public event Action<int, double> ValueChanged;
        public string[] CategoryNames => new[]
        {
            "Available", "SiteBuilt", // 0,1
            "Lumber", "Nails", "Paint" // 2, 3, 4
        };

        public double GetValue(int cat)
        {
            if (cat == 0) return IsAvailable ? 1 : 0;
            if (cat == 1) return IsSiteBuilt ? 1 : 0;
            if (cat == 2) return GetResourceAmount(ResourceType.Lumber);
            if (cat == 3) return GetResourceAmount(ResourceType.Nail);
            if (cat == 4) return GetResourceAmount(ResourceType.Paint);
            return 0;
        }

        public string GetDescription(int category, double value) => $"{category} {GetValue(category)}";
        (bool, string) IValueTrigger.Validate(int category, double value) => (true, "");

        void _ValueTrigger(int cat) => ValueChanged?.Invoke(cat, GetValue(cat));
        void _ValueTriggers(params int[] cats) { foreach (int i in cats) _ValueTrigger(i); }
        void _ValueTrigger_Available() => _ValueTrigger(0);
        #endregion
        */
    }


    [Serializable]
    public class ValueCondition : IEquatable<ValueCondition>
    {
        [TypeConstraint(typeof(IValueTrigger), labelWidth = 100), SerializeField] private UnityEngine.Object target;
        [SerializeField] private int category;
        [SerializeField] private ValueOperator value;

        public event Action<ValueCondition> Changed;

        public int Category => category;

        public ValueOperator ValueOp => value;
        public ValueOperator.Operator Operator => value.op;
        public double Value => value.value;

        [field: System.NonSerialized] public bool Valid { get; private set; }
        [field: System.NonSerialized] public IValueTrigger Target { get; private set; }
        public UnityEngine.Object InstantTarget => target;

        public double CurrentValue => Target.GetValue(category);

        public ValueCondition(UnityEngine.Object target, int category, ValueOperator value)
        {
            this.target = target;
            this.category = category;
            this.value = value;
        }

        public void Setup()
        {
            Target = target as IValueTrigger;
            Valid = Target != null;
            if (Valid)
            {
                Target.ValueChanged += OnTargetValueChanged;
            }
        }

        public void TearDown()
        {
            if (Valid)
            {
                Target.ValueChanged -= OnTargetValueChanged;
            }
        }

        public void Listen(Action<ValueCondition> listener, bool register)
        {
            if (!Valid) return;

            if (register) Changed += listener;
            else Changed -= listener;
        }

        private void OnTargetValueChanged(int cat, double v)
        {
            if (cat == category) Changed?.Invoke(this);
        }

        public bool Equals(ValueCondition other)
        {
            if (other == null) return false;

            return target == other.target && category == other.category && value.Equals(other.value);
        }

        public bool Meets(int cat, double v) => cat == category && value.Meets(v);
        public bool Meets() => Valid && Meets(category, Target.GetValue(category));
        public bool NoneOrMeets => !Valid || Meets();

        public override string ToString()
        {
            return $"{Target?.GetType()}: category {category}, op {value.op}, value {Target?.GetValue(category)}/{value.value}";
        }

        public string Description => $"{Target?.GetDescription(category, value.value)}: {CurrentValue} {Operator} {Value} => {Meets()}";
    }

    [Serializable]
    public struct ValueOperator
    {
        public Operator op;
        public double value;

        public bool Meets(double v)
        {
            switch (op)
            {
                case Operator.Greater: return v > value;
                case Operator.GEqual: return v >= value;
                case Operator.Equal: return Math.Abs(v - value) < 0.01;
                case Operator.Less: return v < value;
                case Operator.LEqual: return v <= value;
                case Operator.NotEqual: return Math.Abs(v - value) > 0.1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum Operator
        {
            Greater, GEqual, Equal, Less, LEqual, NotEqual
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ValueOperator))]
    public class ValueOperatorEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (label != GUIContent.none)
                position = EditorGUI.PrefixLabel(position, label);

            var opProp = property.FindPropertyRelative(nameof(ValueOperator.op));
            var valueProp = property.FindPropertyRelative(nameof(ValueOperator.value));

            var (r1, r2) = position.HSplit(0.4f);

            EditorGUI.PropertyField(r1, opProp, GUIContent.none);
            EditorGUI.PropertyField(r2, valueProp, GUIContent.none);

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ValueCondition))]
    public class ValueConditionDrawer : PropertyDrawer
    {
        private const float Spacing = 5f;
        private int Lines(SerializedProperty property) => property.isExpanded ? 3 : 1;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var rect = EditorGUI.IndentedRect(position);
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);

            var lines = Lines(property);
            var (r1, rr) = position.VSplit(ratio: 1.0f / lines);

            using (new LabelWidth(100))
            {
                r1 = r1.Indented(1).Extended(-Spacing / 2, 0, -Spacing / 2, 0);
                property.isExpanded = EditorGUI.Foldout(r1, property.isExpanded, label, true);

                if (property.isExpanded)
                {
                    var targetProp = property.FindPropertyRelative("target");
                    var valueProp = property.FindPropertyRelative("value");
                    var categoryProp = property.FindPropertyRelative("category");

                    rr = rr.Indented(1f).Extended(0, -3, 0, 0);
                    var (r2, r3) = rr.VSplit(ratio: 0.5f);
                    EditorGUI.PropertyField(r2, targetProp);

                    var (r31, r32) = r3.HSplit(ratio: 0.4f);

                    r31 = r31.Extended(-Spacing / 2, 0, -Spacing / 2, 0);
                    DrawCategory(r31, categoryProp, targetProp.objectReferenceValue as IValueTrigger);

                    r32 = r32.Extended(-Spacing / 2, 0, -Spacing / 2, 0);
                    //EditorGUI.PropertyField(r32, valueProp, GUIContent.none);
                    DrawValue(r32, valueProp, categoryProp.intValue, targetProp.objectReferenceValue as IValueTrigger);
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private void DrawCategory(Rect rect, SerializedProperty property, IValueTrigger valueTrigger)
        {
            EditorGUI.BeginProperty(rect, GUIContent.none, property);

            var options = valueTrigger?.CategoryNames ?? new string[0];
            var selected = property.intValue;
            var selection = EditorGUI.Popup(rect, selected, options);
            if (selected != selection)
            {
                property.intValue = selection;
            }

            EditorGUI.EndProperty();
        }

        private void DrawValue(Rect rect, SerializedProperty property, int category, IValueTrigger valueTrigger)
        {
            EditorGUI.BeginProperty(rect, GUIContent.none, property);

            var validate = valueTrigger?.Validate(category, property.FindPropertyRelative("value").doubleValue) ?? (true, "");

            using (new GuiColor(validate.Item1 ? Color.white : Color.yellow))
            {
                EditorGUI.PropertyField(rect, property, GUIContent.none);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + Spacing) * Lines(property);
        }
    }
#endif
}