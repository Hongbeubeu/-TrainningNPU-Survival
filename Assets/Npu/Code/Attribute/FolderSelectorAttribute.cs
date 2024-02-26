using Npu.Helper;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using Npu.EditorSupport;
#endif

namespace Npu
{
    public class FolderSelectorAttribute : PropertyAttribute
    {
        public bool relative = true;
        public float labelWith = -1;
        public bool fileSelect = false;
        public bool showLabel = true;
        public bool coloring = true;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FolderSelectorAttribute))]
    public class FolderSelectorAttributeDrawer : PropertyDrawer
    {
        static string ProjectPath => Path.GetDirectoryName(Application.dataPath);

        FolderSelectorAttribute TargetAttribute => attribute as FolderSelectorAttribute;

        string FullPath(string path) => TargetAttribute.relative ? Path.Combine(ProjectPath, path) : path;

        string VisiblePath(string fullPath)
        {
            if (!TargetAttribute.relative)
                return fullPath;

            if (fullPath.StartsWith(ProjectPath))
            {
                var p = fullPath.Substring(ProjectPath.Length);
                if (p.StartsWith(new string(Path.DirectorySeparatorChar, 1))) return p.Substring(1);
                return p;
            }

            return fullPath;
        }

        bool IsValidPath(string path) =>
            TargetAttribute.fileSelect ? File.Exists(FullPath(path)) : Directory.Exists(FullPath(path));

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            using (new LabelWidth(TargetAttribute.labelWith > 0 ? TargetAttribute.labelWith : EditorGUIUtility.labelWidth))
            {
                if (TargetAttribute.showLabel) position = EditorGUI.PrefixLabel(position, label);

                var (r1, r2) = position.HFixedSplit(position.width - 30);

                using (new GuiColor(!TargetAttribute.coloring
                    ? Color.white
                    : (IsValidPath(property.stringValue) ? Color.white : Color.yellow)))
                {
                    EditorGUI.PropertyField(r1, property, GUIContent.none);
                }

                if (GUI.Button(r2.Extended(left:-5), "..."))
                {
                    var s = TargetAttribute.fileSelect ? 
                        EditorUtility.OpenFilePanel("Select File", Path.GetDirectoryName(FullPath(property.stringValue)), "") 
                        : EditorUtility.OpenFolderPanel("Select Folder", FullPath(property.stringValue), "");

                    if (!string.IsNullOrEmpty(s))
                    {
                        property.stringValue = VisiblePath(s);
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}