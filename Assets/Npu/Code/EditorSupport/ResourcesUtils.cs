using System.IO;
using UnityEditor;
using UnityEngine;

namespace Npu.EditorSupport
{
    public static class ResourcesUtils
    {
        public static TObject LoadOrCreate<TObject>(string resourceDir, string assetName) where TObject : ScriptableObject
        {
            var resourcePath = Path.Combine(resourceDir, assetName);
            var asset = Resources.Load<TObject>(resourcePath);
#if UNITY_EDITOR
            if (!asset)
            {
                asset = ScriptableObject.CreateInstance<TObject>();
                var fullDir = Path.Combine("Assets/Resources", resourceDir);
                AssetUtils.EnsureFolderExists(fullDir);
                AssetDatabase.CreateAsset(asset, Path.Combine(fullDir, $"{assetName}.asset"));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
            return asset;
        }
    }
}