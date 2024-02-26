using UnityEngine;
using System;
using Npu.Helper;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{

    [Serializable]
    public class BindingData
    {
        public Kind kind;
        public string text;
        public UnityEngine.Object asset;

        private int? _intV;
        public int IntValue
        {
            get
            {
                if (_intV == null)
                {
                    int.TryParse(text, out var v);
                    _intV = v;
                }
                return _intV.Value;
            }
        }

        private float? _floatV;
        public float FloatValue
        {
            get
            {
                if (_floatV == null)
                {
                    float.TryParse(text, out var v);
                    _floatV = v;
                }
                return _floatV.Value;
            }
        }

        private bool? _boolV;
        public bool BoolValue => (_boolV ?? (_boolV = text.Equals("1", StringComparison.Ordinal))).Value;

        private Vector2? _vector2V;
        public Vector2 Vector2Value => (_vector2V ?? (_vector2V = text.ToVector2())).Value;

        private Vector3? _vector3V;
        public Vector3 Vector3Value => (_vector3V ?? (_vector3V = text.ToVector3())).Value;

        private Vector4? _vector4V;
        public Vector4 Vector4Value =>  (_vector4V ?? (_vector4V = text.ToVector4())).Value;
        
        public Color ColorValue => Vector4Value;

        public object Value
        {
            get
            {
                switch (kind)
                {
                    case Kind.Text: return text;
                    case Kind.Asset: return asset;
                    case Kind.Vector2: return Vector2Value;
                    case Kind.Vector3: return Vector3Value;
                    case Kind.Vector4: return Vector4Value;
                    case Kind.Color: return ColorValue;
                    case Kind.Bool: return BoolValue;
                    case Kind.Int: return IntValue;
                    case Kind.Float: return FloatValue;
                    //case Kind.AssetRef: return AssetManager.Instance.Find(text);
                    default: return text;
                }
            }

            set
            {
                switch (kind)
                {
                    case Kind.Asset:
                        asset = value as UnityEngine.Object;
                        break;
                    case Kind.Text:
                        text = value as string;
                        break;
                    case Kind.Bool:
                        text = (bool) value ? "1" : "0";
                        _boolV = null;
                        break;
                    case Kind.Int:
                        text = value.ToString();
                        _intV = null;
                        break;
                    case Kind.Float:
                        text = value.ToString();
                        _floatV = null;
                        break;
                    case Kind.Vector2:
                        text = ((Vector2) value).ToSerializeString();
                        _vector2V = null;
                        break;
                    case Kind.Vector3:
                        text = ((Vector3) value).ToSerializeString();
                        _vector3V = null;
                        break;
                    case Kind.Vector4:
                        text = ((Vector4) value).ToSerializeString();
                        _vector4V = null;
                        break;
                    case Kind.Color:
                        text = ((Vector4) value).ToSerializeString();
                        _vector4V = null;
                        break;
                    
                }
            }
        }

        public void Uncache()
        {
            _boolV = null;
            _intV = null;
            _floatV = null;
            _vector2V = null;
            _vector3V = null;
            _vector4V = null;
        }

        public BindingData Clone()
        {
            return new BindingData
            {
                kind = kind, text = text, asset = asset
            };
        }

        public void CopyFrom(BindingData other)
        {
            this.kind = other.kind;
            this.text = string.Copy(other.text);
            this.asset = other.asset;
        }

        public enum Kind
        {
            Text,
            Asset,
            Float,
            Vector2,
            Vector3,
            Vector4,
            Color,
            Bool,
            Int,
            AssetRef
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BindingData))]
    public class BindingDataEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var kindProp = property.FindPropertyRelative(nameof(BindingData.kind));
            var textProp = property.FindPropertyRelative(nameof(BindingData.text));
            var assetProp = property.FindPropertyRelative(nameof(BindingData.asset));

            var w = position.width;

            position.width = 100;
            EditorGUI.PropertyField(position, kindProp, GUIContent.none);

            position.x += position.width + 5;
            position.width = w - position.width - 5;

            if (kindProp.intValue == (int) BindingData.Kind.Text)
            {
                textProp.stringValue = EditorGUI.TextArea(position, textProp.stringValue);
            }
            else if (kindProp.intValue == (int) BindingData.Kind.Float)
            {
                textProp.stringValue = DrawFloat(position, textProp.stringValue);
            }
            else if (kindProp.intValue == (int) BindingData.Kind.Vector2)
            {
                textProp.stringValue = DrawVector2(position, textProp.stringValue);
            }
            else if (kindProp.intValue == (int) BindingData.Kind.Vector3)
            {
                textProp.stringValue = DrawVector3(position, textProp.stringValue);
            }
            else if (kindProp.intValue == (int) BindingData.Kind.Vector4)
            {
                textProp.stringValue = DrawVector4(position, textProp.stringValue);
            }
            else if (kindProp.intValue == (int) BindingData.Kind.Color)
            {
                textProp.stringValue = DrawColor(position, textProp.stringValue);
            }
            else if (kindProp.intValue == (int) BindingData.Kind.Bool)
            {
                textProp.stringValue = DrawBool(position, textProp.stringValue);
            }
            else if (kindProp.intValue == (int) BindingData.Kind.Int)
            {
                textProp.stringValue = DrawInt(position, textProp.stringValue);
            }
            else if (kindProp.intValue == (int) BindingData.Kind.AssetRef)
            {
                DrawAssetRef(position, textProp);
            }
            else
            {
                EditorGUI.PropertyField(position, assetProp, GUIContent.none);
            }


            EditorGUI.EndProperty();
        }

        private void DrawAssetRef(Rect position, SerializedProperty property)
        {
#if false            
            var addresses = AssetManager.Instance.Addresses;
            var address = addresses.FirstOrDefault(i => i.ID == property.stringValue);

            var (r1, r2) = position.HFixedSplit(position.width - 30);
            using (new GuiColor(address == null ? Color.red : Color.white))
            {
                EditorGUI.TextField(r1, address?.Path);
            }
            if (GUI.Button(r2.Extended(left:-3), "..."))
            {
                var menu = new GenericMenu();
                foreach (var i in addresses.OrderBy(i => i.Path))
                {
                    menu.AddItem(new GUIContent(i.Path), i.ID == property.stringValue, data =>
                    {
                        property.stringValue = (data as AssetAddress).ID;
                        property.serializedObject.ApplyModifiedProperties();
                    }, i);
                }
                menu.ShowAsContext();
            }
#endif
        }

        string DrawFloat(Rect position, string text)
        {
            float v;
            float.TryParse(text, out v);
            v = EditorGUI.FloatField(position, v);

            return v.ToString("R");
        }

        string DrawVector4(Rect position, string text)
        {
            var v = text.ToVector4();
            v = EditorGUI.Vector4Field(position, GUIContent.none, v);
            return v.ToSerializeString();
        }

        string DrawVector3(Rect position, string text)
        {
            var v = text.ToVector3();
            v = EditorGUI.Vector3Field(position, GUIContent.none, v);
            return v.ToSerializeString();
        }

        string DrawVector2(Rect position, string text)
        {
            var v = text.ToVector2();
            v = EditorGUI.Vector2Field(position, GUIContent.none, v);
            return v.ToSerializeString();
        }

        string DrawColor(Rect position, string text)
        {
            var v = text.ToVector4();
            v = EditorGUI.ColorField(position, GUIContent.none, v);
            return v.ToSerializeString();
        }

        string DrawBool(Rect position, string text)
        {
            int vv;
            int.TryParse(text, out vv);
            var v = vv != 0;
            v = EditorGUI.Toggle(position, v);
            return v ? "1" : "0";
        }

        string DrawInt(Rect position, string text)
        {
            int v;
            int.TryParse(text, out v);
            v = EditorGUI.IntField(position, v);
            return v.ToString();
        }

        public int Lines(SerializedProperty property)
        {
            var kindProp = property.FindPropertyRelative(nameof(BindingData.kind));
            var textProp = property.FindPropertyRelative(nameof(BindingData.text));
            if (kindProp.intValue == (int) BindingData.Kind.Text && textProp.stringValue.IndexOf('\n') >= 0)
            {
                return 2;
            }

            return 1;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Lines(property) * base.GetPropertyHeight(property, label);
        }
    }
#endif

}