using System;
using System.Collections.Generic;
using Npu.Common;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using System.Reflection;


namespace Npu.EditorSupport
{
    
    public class EditorCommonSettings
    {
        public string SettingKey { get; }

        public EditorCommonSettings(string key)
        {
            SettingKey = key;

            _currentPage = new PlayerPrefsValue<int>($"{SettingKey}_CurrentPage", EditorPrefs.GetInt, EditorPrefs.SetInt, 1);
            _itemsPerPage = new PlayerPrefsValue<int>($"{SettingKey}_ItemsPerPage", EditorPrefs.GetInt, EditorPrefs.SetInt, 20);
            _searchPattern = new PlayerPrefsValue<string>($"{SettingKey}_SearchPattern", EditorPrefs.GetString, EditorPrefs.SetString, "");
        }

        private PlayerPrefsValue<int> _currentPage;
        public int CurrentPage
        {
            get => _currentPage.Value;
            set => _currentPage.Value = value;
        }

        private PlayerPrefsValue<int> _itemsPerPage;
        public int ItemsPerPage
        {
            get => _itemsPerPage.Value;
            set => _itemsPerPage.Value = value;
        }

        private PlayerPrefsValue<string> _searchPattern;
        public string SearchPattern
        {
            get => _searchPattern.Value;
            set => _searchPattern.Value = value;
        }

        public bool GetBool(string key) => PlayerPrefs.GetInt($"{SettingKey}_{key}", 0) > 0;
        public void SetBool(string key, bool value) => PlayerPrefs.SetInt($"{SettingKey}_{key}", value ? 1 : 0);
        
        public float GetFloat(string key, float def=0) => PlayerPrefs.GetFloat($"{SettingKey}_{key}", def);
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat($"{SettingKey}_{key}", value);

    }

    public class DirectorySelector
    {
        public string Path
        {
            get => EditorPrefs.GetString($"{_key}_folder_select");
            set => EditorPrefs.SetString($"{_key}_folder_select", value);
        }

        private string _key;

        public DirectorySelector(string key, string path)
        {
            _key = key;
            if (string.IsNullOrEmpty(Path)) Path = path;
        }

        public void DoLayout(string label, List<string> suggestions)
        {
            using (new HorizontalLayout())
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(100));

                using (new GuiColor(Directory.Exists(EditorUtils.AbsolutePath(Path)) ? Color.white : Color.yellow))
                {
                    Path = EditorGUILayout.DelayedTextField(GUIContent.none, Path);
                }

                if (suggestions != null && suggestions.Count > 0 && GUILayout.Button("...", GUILayout.Width(30)))
                {
                    var menu = new GenericMenu();
                    foreach (var i in suggestions)
                    {
                        menu.AddItem(new GUIContent(i), false, data => { Path = i; }, i);
                    }

                    menu.ShowAsContext();
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Folder Icon", "X|Select Folder"),
                    GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    var chosen = EditorUtility.OpenFolderPanel("Select Folder", Path, "");
                    if (string.IsNullOrEmpty(chosen)) return;

                    Path = EditorUtils.RelativePath(chosen);
                }
            }

        }
    }

    public static partial class EditorUtils
    {
        public static string ProjectPath => Path.GetDirectoryName(Application.dataPath);
        public static string EditorSavedDataPath => Path.Combine(ProjectPath, "SavedData");

        public static string RelativePath(string absPath)
        {
            var p = ProjectPath;
            if (!absPath.StartsWith(p)) throw new InvalidOperationException($"{absPath} is not a valid path");
            var ret = absPath.Substring(p.Length);
            return ret[0] == '/' || ret[0] == '\\' ? ret.Substring(1) : ret;
        }

        public static string AbsolutePath(string relativePath)
        {
            return Path.Combine(ProjectPath, relativePath);
        }

        

        public static Texture2D DefaultIcon
        {
            get
            {
                var ts = MethodGetAll.Invoke(null, new object[] {""}) as Texture2D[];
                return ts == null || ts.Length == 0 ? null : ts[0];
            }

            set { MethodSetIcons.Invoke(null, new object[] {"", new Texture2D[] {value}}); }
        }
        
        [MenuItem("GameObject/Copy Path", false, 11)]
        public static void CopyPath()
        {
            var currentGameObject = Selection.activeGameObject;
 
            if (currentGameObject == null)
                return;
 
            var path = currentGameObject.name;
 
            while (currentGameObject.transform.parent != null)
            {
                currentGameObject = currentGameObject.transform.parent.gameObject;
 
                path = $"{currentGameObject.name}/{path}";
            }
 
            EditorGUIUtility.systemCopyBuffer = path;
        }

        [MenuItem("GameObject/Copy Path", true)]
        public static bool CopyPathValidation() => Selection.gameObjects.Length == 1;

        private static MethodInfo _methodGetAll;

        private static MethodInfo MethodGetAll => _methodGetAll ??= typeof(PlayerSettings).GetMethod("GetAllIconsForPlatform",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        private static MethodInfo _methodSetIcons;

        private static MethodInfo MethodSetIcons => _methodSetIcons ??= typeof(PlayerSettings).GetMethod(
            "SetIconsForPlatform",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new Type[] {typeof(string), typeof(Texture2D[])},
            null);
    }
}
#endif
