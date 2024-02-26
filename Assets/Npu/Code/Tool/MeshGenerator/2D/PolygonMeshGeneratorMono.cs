using System.Collections.Generic;
using System.Linq;
using Npu.Helper;
using UnityEditor;
using UnityEngine;

namespace Npu.Scripts.Tool.MeshGenerator._2D
{
    public class PolygonMeshGeneratorMono : MonoBehaviour
    {
        [SerializeField] private PolygonVertexGroupMono[] vertexGroups;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private string savePath = "Assets/GeneratedMesh/";
        [SerializeField] private string meshName = "Untitled";

        private Mesh _mesh;

        //When the array of vertices changed, call this
        [ContextMenu("ValidatePoints")]
        public void ValidatePoints()
        {
            var vertices = GetComponentsInChildren<IPolygonVertexGroup>();
            vertexGroups = vertices.Select(v => v as PolygonVertexGroupMono).ToArray();
            var index = 0;
            vertices.ForEach(p =>
            {
                p.Index = index++;
                p.Transform.gameObject.name = $"VertexGroup - {p.Index}";
                p.ValidatePoints();
            });
        }

        [ContextMenu(nameof(CreateMeshInMemory))]
        public void CreateMeshInMemory()
        {
            IEnumerable<Vector3> vertices = new List<Vector3>();
            var triangles = new List<int>();
            var offset = 0;
            foreach (var vg in vertexGroups)
            {
                var points = vg.Points;

                if (points.Length < 3) return;
                meshFilter.mesh = _mesh = new Mesh();
                _mesh.name = meshName;

                var ps = points.Select(t => vg.Transform.TransformPoint(t.localPosition)).ToList();

                var tri = new Triangulator(ps.ToArray());
                var tris = tri.Triangulate();

                for (var i = 0; i < tris.Length; i++)
                {
                    triangles.Add(offset + tris[i]);
                }

                vertices = vertices.Concat(ps);
                offset += ps.Count;
            }

            _mesh.vertices = vertices.ToArray();
            _mesh.triangles = triangles.ToArray();
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(SaveMeshToAsset))]
        public void SaveMeshToAsset()
        {
            if (meshFilter.sharedMesh == null || !vertexGroups.Any(vg => vg.Points.Length >= 3)) return;

            var path = $"{savePath}/{_mesh.name}";

            AssetDatabase.CreateAsset(meshFilter.sharedMesh, path + ".asset");
            AssetDatabase.SaveAssets();

            var newGO = Instantiate(meshFilter.gameObject, transform.parent, true);
            newGO.name = _mesh.name;

            // PrefabUtility.SaveAsPrefabAsset(newGO, path + ".prefab");

            // DestroyImmediate(newGO);
        }

        private void OnDrawGizmos()
        {
            foreach (var vg in vertexGroups)
            {
                var points = vg.Points;
                if (points.Length <= 2 || !_gizmos) return;

                for (var i = 0; i < points.Length - 1; i++)
                {
                    Gizmos.DrawLine(points[i].position, points[i + 1].position);
                    DrawString($"{i}", points[i].position, Color.cyan);
                }

                Gizmos.DrawLine(points[0].position, points[points.Length - 1].position);
                DrawString($"{points.Length - 1}", points[points.Length - 1].position, Color.cyan);
            }
        }

        private bool _gizmos = true;

        [ContextMenu(nameof(ToggleGizmos))]
        private void ToggleGizmos()
        {
            _gizmos = !_gizmos;
        }

        private static void DrawString(string text, Vector3 worldPos, Color? colour = null)
        {
            UnityEditor.Handles.BeginGUI();
            if (colour.HasValue) GUI.color = colour.Value;
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            var screenPos = view.camera.WorldToScreenPoint(worldPos);
            var size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height - 45, size.x, size.y),
                text);
            UnityEditor.Handles.EndGUI();
        }
#endif
    }
}