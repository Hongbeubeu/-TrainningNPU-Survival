using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Npu.Helper
{
    public class MeshExporter : MonoBehaviour
    {
        [FolderSelector(fileSelect = true)] public string path;

        public string AbsolutePath => Path.Combine(Path.GetDirectoryName(Application.dataPath), path ?? "");

        public void Export()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(path)) return;

            var absPath = AbsolutePath;
            var mesh = GetComponent<MeshFilter>()?.sharedMesh;
            if (mesh) AssetDatabase.CreateAsset(mesh, absPath);
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(path)) return;
            var mf = GetComponent<MeshFilter>();
            if (!mf || mf.sharedMesh == null) return;

            path = mf.sharedMesh.AssetPath();
            EditorUtility.SetDirty(this);
        }

#endif
        
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(MeshExporter))]
    public class MeshExporterEditor : Editor
    {
        private MeshExporter Target => target as MeshExporter;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Export"))
            {
                Target.Export();
            }
        }
    }
#endif
}    
