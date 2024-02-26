using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Npu
{
    public partial class ComponentPresets : ScriptableObject
    {

        static ComponentPresets _instance;
        public static ComponentPresets Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                var ss = AssetDatabase.FindAssets("t:Npu.ComponentPresets");
                if (ss.Length == 0)
                {
                    _instance = CreateInstance<ComponentPresets>();
                    AssetDatabase.CreateAsset(_instance, "Assets/Screw/App/Editor/ComponentPresets.asset");
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    _instance = AssetDatabase.LoadAssetAtPath<ComponentPresets>(AssetDatabase.GUIDToAssetPath(ss[0]));
                }
                return _instance;
            }
        }
        
        [Serializable]
        public class TextPreset
        {
            public Font font;
            public int fontSize = 30;
            public Color color = Color.white;
            
            public void Apply(Text text)
            {
                if (font) text.font = font;
                text.fontSize = fontSize;
                text.color = color;
                EditorUtility.SetDirty(text);
            }
        }
        
        [MenuItem("Npu/Tools/Component Presets")]
        private static void ShowMe()
        {
            var settings = Instance;
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }
    }
    
}
    