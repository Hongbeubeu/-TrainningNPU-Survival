using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;
using System.Linq;
using Npu.EditorSupport;
using Npu.Helper;
using UnityEngine.U2D;


namespace Npu
{
    public static partial class EditorMenu
    {
        [MenuItem("Npu/Editor/Scene Organizer")]
        public static void OpenSceneOrganizerWindow()
        {
            SceneOrganizerWindow.Open();
        }
        
        [MenuItem("Game/Enable Run in Background", false, 22)]
        public static void EnableRunInBG()
        {
            Application.runInBackground = true;
        }

        [MenuItem("Game/Enable Run in Background", true, 22)]
        public static bool EnableRunInBGConditional()
        {
            return !Application.runInBackground;
        }

        [MenuItem("Game/Disable Run in Background", false, 22)]
        public static void DisableRunInBG()
        {
            Application.runInBackground = false;
        }

        [MenuItem("Game/Disable Run in Background", true, 22)]
        public static bool DisableRunInBGConditional()
        {
            return Application.runInBackground;
        }

        [MenuItem("Game/Clear PlayerPrefs", false, 44)]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Game/Clear Data", false, 44)]
        public static void ClearData()
        {
            Directory.Delete(EditorUtils.EditorSavedDataPath, true);
        }

        [MenuItem("Game/Clear All", false, 44)]
        public static void ClearAll()
        {
            ClearPlayerPrefs();
            ClearData();
        }

        [MenuItem("Npu/Tools/Sprite Atlas")]
        public static void SpriteAtlas()
        {
            var ss = AssetDatabase.FindAssets("t:SpriteAtlasTool");
            SpriteAtlasTool asset = null;
            if (ss.Length == 0)
            {
                asset = ScriptableObject.CreateInstance<SpriteAtlasTool>();
                AssetDatabase.CreateAsset(asset, "Assets/SpriteAtlasTool.asset");
                AssetDatabase.SaveAssets();
            }
            else
            {
                asset = AssetDatabase.LoadAssetAtPath<SpriteAtlasTool>(AssetDatabase.GUIDToAssetPath(ss[0]));
            }

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        [MenuItem("Npu/Tools/Log Non Atlas Sprites")]
        public static void LogNonAtlasSprites()
        {
            var altlases = AssetDatabase.FindAssets("t:SpriteAtlas")
                .Select(i => AssetUtils.LoadAssetByGuid(i, null))
                .OfType<SpriteAtlas>();
                
            var sprites = AssetDatabase.FindAssets("t:sprite", new string[] {"Assets/Textures"})
                .Select(i => AssetUtils.LoadAssetByGuid(i, null))
                .Select(i => i.GetChildAssets<Sprite>().FirstOrDefault())
                .Where(i => i)
                .ToList();
            
            var non = sprites.Where(i => !altlases.Any(a => a.Contains(i)))
                .Select(i => i.AssetPath())
                .OrderBy(i => i).ToList();
            
            Logger.Log("Tools", $"{non.Count}\n{non.Join("\n")}");
        }

        [MenuItem("CONTEXT/MeshFilter/Save Mesh")]
        public static void ExportMesh(MenuCommand m)
        {
            var mesh = (m.context as MeshFilter).sharedMesh;
            var p = EditorUtility.SaveFilePanelInProject("Save", mesh.name, "asset", "");
            if (!string.IsNullOrEmpty(p))
            {
                Debug.LogFormat("Save to {0}", p);
                AssetDatabase.CreateAsset(mesh, p);
            }

        }

        [MenuItem("Window/New Locked Inspector")]
        static void ShowLockedInspector()
        {
            var w = CreateEditorWindow("UnityEditor.InspectorWindow");
            w?.Show();
            w?.GetType()?.GetProperty("isLocked", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(w, true);
        }

        static EditorWindow CreateEditorWindow(string windowTypeName)
        {
            var windowType = typeof(Editor).Assembly.GetType(windowTypeName);

            if (windowType == null)
            {
                Debug.LogErrorFormat("{0} not found", windowTypeName);
                return null;
            }

            var w = Activator.CreateInstance(windowType, true);

            if (w == null)
            {
                Debug.LogErrorFormat("Cannot create window of type {0}", windowType);
                return null;
            }

            return w as EditorWindow;
        }

        static EditorWindow GetEditorWindow(string windowTypeName)
        {
            var windowType = typeof(Editor).Assembly.GetType(windowTypeName);

            if (windowType == null)
            {
                Debug.LogErrorFormat("{0} not found", windowTypeName);
                return null;
            }

            return EditorWindow.GetWindow(windowType);
        }

        [MenuItem("CONTEXT/ScriptableObject/Delete Child Asset")]
        public static void DeleteChildAsset(MenuCommand m)
        {
            var o = m.context as ScriptableObject;
            var path = o.AssetPath();
            UnityEngine.Object.DestroyImmediate(o, true);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);
        }
        
        [MenuItem("CONTEXT/ScriptableObject/Ping me!")]
        public static void PingMe(MenuCommand m)
        {
            var o = m.context as ScriptableObject;
            EditorGUIUtility.PingObject(o);
        }
    }

}
