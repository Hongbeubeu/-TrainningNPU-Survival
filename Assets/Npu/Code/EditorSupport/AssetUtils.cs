using System;
using UnityEngine;
using System.Collections.Generic;
using Npu.Helper;

using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEngine.U2D;

namespace Npu.EditorSupport
{

    public static class AssetUtils
    {
        private static string Tag => nameof(AssetUtils);

        public static TObject LoadOrCreate<TObject>(string assetPath) where TObject : ScriptableObject
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(assetPath) as TObject;
            if (!asset)
            {
                var dir = Path.GetDirectoryName(assetPath);
                var file = Path.GetFileName(assetPath);
                asset = ScriptableObject.CreateInstance<TObject>();
                EnsureFolderExists(dir);
                AssetDatabase.CreateAsset(asset, Path.Combine(dir, $"{file}.asset"));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return asset;
        }

        public static Object LoadAssetByGuid(string guid, string subAsset)
        {
            return LoadAssetByPath(AssetDatabase.GUIDToAssetPath(guid), subAsset);
        }

        public static Object LoadAssetByPath(string path, string subAsset)
        {
            if (string.IsNullOrEmpty(subAsset)) return AssetDatabase.LoadMainAssetAtPath(path);

            return AssetDatabase.LoadAllAssetRepresentationsAtPath(path).FirstOrDefault(i => i.name == subAsset);
        }

        public static string AssetGuid(this Object asset) => AssetDatabase.AssetPathToGUID(asset.AssetPath());

        public static Object LoadAsset(string path)
        {
            var index = path.IndexOf('[');
            if (index >= 0)
            {
                var mainPath = path.Substring(0, index);
                if (path[path.Length - 1] != ']') throw new InvalidOperationException($"Invalid path {path}");
                var subPath = path.Substring(index + 1, path.Length - index - 2);

                var obs = AssetDatabase.LoadAllAssetsAtPath(mainPath);
                Logger.Log(Tag, $"Find at {mainPath}/{subPath}");

                return obs.FirstOrDefault(i => i.name.Equals(subPath, StringComparison.Ordinal));
            }

            return AssetDatabase.LoadMainAssetAtPath(path);
        }

        /// <summary>
        /// Create folder if not exist
        /// </summary>
        /// <param name="path">Relative to project root. Ex "Assets/Resources/Data"</param>
        public static void EnsureFolderExists(string path)
        {
            var parts = path.Trim('/', ' ').Split('/');
            if (parts.Length == 0) return;
            var parent = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var current = parts[i];
                if (!AssetDatabase.IsValidFolder($"{parent}/{current}")) AssetDatabase.CreateFolder(parent, current);
                parent = $"{parent}/{current}";
            }
        }

        public static void AddChildAsset(this Object parent, Object child)
        {
            AssetDatabase.AddObjectToAsset(child, parent);
        }

        public static T AddChildAsset<T>(this Object parent) where T : ScriptableObject
        {
            var o = ScriptableObject.CreateInstance<T>();
            parent.AddChildAsset(o);
            return o;
        }

        public static Object[] GetChildAssets(this Object asset)
        {
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(asset));
        }

        public static T[] GetChildAssets<T>(this Object asset)
        {
            return asset.GetChildAssets().OfType<T>().ToArray();
        }

        public static T[] GetChildAssets<T>(this Object asset, Predicate<T> predicate)
        {
            return asset.GetChildAssets().OfType<T>().Where(i => predicate(i)).ToArray();
        }

        public static Object GetChildAsset(this Object mainAsset, string subAssetName)
            => mainAsset.GetChildAssets().FirstOrDefault(i => i.name == subAssetName);

        public static void RemoveFromParentAsset(this Object asset)
        {
            AssetDatabase.RemoveObjectFromAsset(asset);
        }

        public static bool IsSubAsset(this Object asset) => AssetDatabase.IsSubAsset(asset);
        public static bool IsMainAsset(this Object asset) => !AssetDatabase.IsSubAsset(asset);

        public static Object MainAsset(this Object subAsset) =>
            subAsset.IsSubAsset() ? AssetDatabase.LoadMainAssetAtPath(subAsset.AssetPath()) : subAsset;


        public static void CreateChildAsset(SerializedProperty property, Type type, string name = null)
        {
            var asset = ScriptableObject.CreateInstance(type);
            if (asset == null)
            {
                Logger.Error("AssetTools", $"Failed to create {type}");
                return;
            }

            var parent = property.GetParent() as Object;
            var parentIsSubAsset = IsSubAsset(parent);

            var target = property.serializedObject.targetObject as ScriptableObject;
            var path = target.AssetPath();

            Debug.Log($"{parent} {target}");

            var fileEndingName = ObjectNames.NicifyVariableName(property.name);
            if (parentIsSubAsset) asset.name = $"{parent?.name}/{fileEndingName}";
            else asset.name = $"|{fileEndingName}";

            target.AddChildAsset(asset);
            property.objectReferenceValue = asset;

            property.serializedObject.ApplyModifiedProperties();

            EditorGUIUtility.PingObject(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);
        }

        public static IEnumerable<Object> FindAssets(Type type, string path = "Assets")
        {
            var typeStr = type.ToString();
            if (typeStr.StartsWith("UnityEngine")) typeStr = type.Name;
            return AssetDatabase.FindAssets($"t:{typeStr}", new[] {path})
                .Select(i => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(i), type));
        }

        public static IEnumerable<TData> FindAssets<TData>(string path = "Assets")
        {
            var typeStr = typeof(TData).ToString();
            if (typeStr.StartsWith("UnityEngine")) typeStr = typeof(TData).Name;
            return AssetDatabase.FindAssets($"t:{typeStr}", new[] {path})
                .Select(i => AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(i)))
                .OfType<TData>();
        }

        public static SpriteAtlas FindAtlas(this Sprite sprite)
        {
            return FindAssets<SpriteAtlas>().FirstOrDefault(i => i.Sprites().Contains(sprite));
        }


        public static IEnumerable<(Object asset, string path)> EnumerateAssets(this string rootPath)
        {
            if (!Path.IsPathRooted(rootPath)) rootPath = EditorUtils.AbsolutePath(rootPath);
            foreach (var i in rootPath.EnumerateFiles())
            {
                var rel = EditorUtils.RelativePath(i);
                var asset = AssetDatabase.LoadMainAssetAtPath(rel);
                if (asset) yield return (asset, rel);
            }
        }

        public static IEnumerable<(Object asset, string path, AssetImporter importer)> EnumerateAssetsMore(
            this string rootPath)
        {
            if (!Path.IsPathRooted(rootPath)) rootPath = EditorUtils.AbsolutePath(rootPath);
            foreach (var i in rootPath.EnumerateFiles())
            {
                var rel = EditorUtils.RelativePath(i);
                var asset = AssetDatabase.LoadMainAssetAtPath(rel);
                if (asset) yield return (asset, rel, AssetImporter.GetAtPath(rel));
            }
        }

        public static IEnumerable<Object> EnumerateChildAssets(this Object mainAsset)
        {
            return EnumerateChildAssets(mainAsset.AssetPath());
        }

        public static IEnumerable<Object> EnumerateChildAssets(this string mainAssetPath)
        {
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(mainAssetPath);
        }


        public static Texture PreviewTexture(this Object @object)
        {
            return @object is Texture tex ? tex
                : @object ? AssetPreview.GetAssetPreview(@object) ?? AssetDatabase.GetCachedIcon(@object.AssetPath())
                : null;
        }

        public static void ReplaceAsset(Object original, Object newAsset)
        {
            if (!original || !newAsset) return;
            if (original.GetType() != newAsset.GetType())
            {
                Debug.LogError("Asset not same type");
                return;
            }

            var srcPath = EditorUtils.AbsolutePath(newAsset.AssetPath());
            var dstPath = EditorUtils.AbsolutePath(original.AssetPath());

            if (!File.Exists(srcPath) || !File.Exists(dstPath))
            {
                Debug.LogError($"\"{srcPath}\" or \"{dstPath}\" not found");
                return;
            }

            Debug.Log($"Move \"{srcPath}\" => \"{dstPath}\" ");
            File.Copy(srcPath, dstPath, true);
        }

        public static List<T> FindAllAssets<T>() where T : Object
        {
            var l = new List<T>();
#if UNITY_EDITOR
            var typeStr = typeof(T).ToString();
            if (typeStr.StartsWith("UnityEngine")) typeStr = typeof(T).Name;

            if (typeof(T) == typeof(SceneAsset)) typeStr = "Scene";
            else if (typeof(T) == typeof(GameObject)) typeStr = "gameobject";

            var guids = AssetDatabase.FindAssets("t:" + typeStr);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var mainObj = AssetDatabase.LoadAssetAtPath<T>(path);
                var allObjs = new[] {mainObj};

                if (!(mainObj is SceneAsset) && !(mainObj is GameObject))
                {
                    try
                    {
                        allObjs = AssetDatabase.LoadAllAssetsAtPath(path).OfType<T>().ToArray();
                    }
                    catch (Exception)
                    {
                    }
                }

                foreach (var obj in allObjs)
                {
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var testGUID, out long _))
                    {
                        if (testGUID == guid) l.Add(obj);
                    }
                }
            }
#else
            l.AddRange(Resources.FindObjectsOfTypeAll<T>());
#endif
            l = l.Distinct().ToList();
            return l;
        }
    }
    
}
#endif
