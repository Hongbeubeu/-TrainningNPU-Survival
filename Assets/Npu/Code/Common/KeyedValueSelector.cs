using System;
using Npu.Helper;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using Npu.EditorSupport;
#endif

namespace Npu.Common
{
    [Serializable]
    public class KeyedValueSelector
    {
        [TypeConstraint(typeof(IKeyedValueProvider)), SerializeField] private Object provider;
        [SerializeField] private string key;
     
        public IKeyedValueProvider Provider => provider as IKeyedValueProvider;

        public object Value => Provider.Get(key);
        public string StringValue => (string) Provider.Get(key);
        public double DoubleValue => (double) Provider.Get(key);
        public int IntValue => (int) DoubleValue;
        public float FloatValue => (float) DoubleValue;
        public bool BoolValue => DoubleValue > 0.2f;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(KeyedValueSelector))]
    public class KeyedValueSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var providerProp = property.FindPropertyRelative("provider");
            var keyProp = property.FindPropertyRelative("key");
            var provider = providerProp.objectReferenceValue as IKeyedValueProvider;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            var (r1, r2) = position.HSplit(0.5f);

            EditorGUI.PropertyField(r1, providerProp, GUIContent.none);
            DrawKey(r2, keyProp, provider);
            
            EditorGUI.EndProperty();
        }

        private static void DrawKey(Rect rect, SerializedProperty property, IKeyedValueProvider provider)
        {
            var keys = provider?.Keys ?? new string[0];
            var key = property.stringValue;
            var selection = Array.IndexOf(keys, key);
            using (new GuiColor(selection >= 0 ? Color.white : Color.yellow))
            {
                var selected = EditorGUI.Popup(rect, selection, keys);
                if (selected != selection) property.stringValue = keys[selected];
            }
        }
    }    
#endif
}