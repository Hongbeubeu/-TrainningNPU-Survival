using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using Npu.EditorSupport;
using UnityEditor;
#endif

namespace Npu.Helper
{
    public class MeshMerger : MonoBehaviour
    {
        public bool includeInactive;
        public MeshFilter[] excludes = new MeshFilter[0];

        [Header("Save To Folder")]
        [FolderSelector] public string folder = "Assets/BakedMesh";

        [ReadOnly] public Mesh combinedMesh;

        public IEnumerable<MeshFilter> Targets => GetComponentsInChildren<MeshFilter>(includeInactive)
            .Except(excludes)
            .Where(i =>
            {
                var v = i.GetComponent<MeshRenderer>();
                return v != null && v.enabled;
            })
            .Where(i => i.GetComponent<Tag>() == null);
            

        [ContextMenu("Do All")]
        public void All()
        {
            AutoExclude();
            Merge();
            Save();
        }
        
        [ContextMenu("Merge")]
        public void Merge()
        {
            var p = transform.position;
            
            transform.position = Vector3.zero;

            var fs = Targets.ToArray();
            var combine = new CombineInstance[fs.Length];
            
            if (fs.Length == 0) return;

            var i = 0;
            while (i < fs.Length)
            {
                combine[i].mesh = fs[i].sharedMesh;
                combine[i].transform = fs[i].transform.localToWorldMatrix;
                var r = fs[i].GetComponent<MeshRenderer>();
                if (r)
                {
                    RecordUndo(r, "Disable renderer");
                    r.enabled = false;
                }
                i++;
            }

            var mesh = new Mesh {name = $"{gameObject.name}-combined-mesh"};
            mesh.CombineMeshes(combine);
            combinedMesh = mesh;
            
            var o = new GameObject("combined-mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(Tag));
            o.transform.parent = transform;
            o.transform.localPosition = Vector3.zero;
            var render = o.GetComponent<MeshRenderer>();
            render.sharedMaterial = fs[0].GetComponent<MeshRenderer>().sharedMaterial;
            render.shadowCastingMode = ShadowCastingMode.Off;
            render.receiveShadows = false;
            o.GetComponent<MeshFilter>().mesh = mesh;
            
            RecordUndoCreation(o, "Create combined-mesh");

            transform.position = p;
        }

        [ContextMenu("Nullify Old Meshes")]
        public void NullifyOldMeshes()
        {
            var fs = GetComponentsInChildren<MeshFilter>(includeInactive)
                .Except(excludes)
                .Where(i => i.GetComponent<Tag>() == null)
                .Where(i => !i.GetComponent<MeshRenderer>().enabled);
            fs.ForEach(i =>
            {
                RecordUndo(i, "Nullify Old Meshes");
                i.sharedMesh = null;
            });
        }

        [ContextMenu("Save")]
        public void Save()
        {
            if (combinedMesh == null) return;
            
#if UNITY_EDITOR            
            var p = EditorUtility.SaveFilePanelInProject("Save Mesh", combinedMesh.name, "asset", "Select Destination", folder);
            if (string.IsNullOrEmpty(p)) return;
            
            Debug.LogFormat("Save to {0}", p);
            AssetDatabase.CreateAsset(combinedMesh, p);
#endif            
        }

        [ContextMenu("Auto Exclude")]
        public void AutoExclude()
        {
            var fs = GetComponentsInChildren<MeshRenderer>(includeInactive)
                .GroupBy(i => i.sharedMaterial)
                .OrderBy(i => i.Count())
                .ToList();

            if (fs.Count > 1)
            {
                excludes = fs.First().Select(i => i.GetComponent<MeshFilter>()).ToArray();
            }
        }

        private void RecordUndo(Object target, string message)
        {
#if UNITY_EDITOR
            Undo.RecordObject(target, message);      
#endif            
        }
        
        private void RecordUndoCreation(Object target, string message)
        {
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(target, message);      
#endif            
        }
        
        private void RecordUndoAddComponent<T>(GameObject target) where T : Component
        {
#if UNITY_EDITOR
            Undo.AddComponent<T>(target);
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MeshMerger))]
    public class MeshMergerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.Separator();
            
            EditorGUIUtils.Section("Preview", DoPreview);
        }

        private  void DoPreview()
        {
            var merger = target as MeshMerger;
            var targets = merger.Targets;
            var i = 0;
            foreach (var m in targets)
            {
                using (new HorizontalLayout())
                {
                    EditorGUILayout.LabelField($"{++i}", GUILayout.Width(60));
                    EditorGUILayout.ObjectField(m, typeof(MeshFilter), false);
                }
            }
        }
    }
#endif    
    
}