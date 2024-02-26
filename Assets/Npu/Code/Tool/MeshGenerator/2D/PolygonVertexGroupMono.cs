using System.Linq;
using Npu.Helper;
using UnityEngine;

namespace Npu.Scripts.Tool.MeshGenerator._2D
{
    public interface IPolygonVertexGroup
    {
        Transform[] Points { get; }
        Transform Transform { get; }
        void ValidatePoints();
        int Index { get; set; }
    }

    public class PolygonVertexGroupMono : MonoBehaviour, IPolygonVertexGroup
    {
        [SerializeField] private Transform[] points;

        public Transform[] Points => points;
        public Transform Transform => transform;

        public void ValidatePoints()
        {
            var vertices = GetComponentsInChildren<IPolygonVertex>();
            points = vertices.Select(v => v.Transform).ToArray();
            var index = 0;
            vertices.ForEach(p =>
            {
                p.Index = index++;
                p.Transform.gameObject.name = $"Vertex - {p.Index}";
            });
        }

        public int Index { get; set; }
    }
}