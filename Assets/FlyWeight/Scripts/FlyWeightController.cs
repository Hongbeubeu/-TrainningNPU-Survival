using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// public readonly struct RenderInfo
// {
//     private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");
//     public readonly Mesh mesh;
//     public readonly Material mat;
//
//     public RenderInfo(Mesh m, Material material)
//     {
//         mesh = m;
//         mat = material;
//     }
//
//     public static RenderInfo FromSprite(Material baseMaterial, Sprite s)
//     {
//         var renderMaterial = Object.Instantiate(baseMaterial);
//         renderMaterial.enableInstancing = true;
//         renderMaterial.SetTexture(MainTexPropertyId, s.texture);
//         Mesh m = new Mesh
//         {
//             vertices = s.vertices.Select(v => (Vector3)v).ToArray(),
//             triangles = s.triangles.Select(t => (int)t).ToArray(),
//             uv = s.uv
//         };
//         return new RenderInfo(m, renderMaterial);
//     }
// }

public class FlyWeightController : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Material _material;
    private RenderInfo _renderInfo;
    private List<EnemyBaseData> _enemyList = new();
    private const int batchSize = 7;
    private readonly Matrix4x4[] posMatrixArr = new Matrix4x4[batchSize];

    private void Start()
    {
        _enemyList.Clear();
        for (var i = 0; i < 10; i++)
        {
            _enemyList.Add(new EnemyBaseData { Position = Random.insideUnitCircle * 10f });
        }

        _renderInfo = RenderInfo.FromSprite(_material, _sprite);
    }

    private void Update()
    {
        DrawEnemy();
    }

    private void DrawEnemy()
    {
        Graphics.DrawMeshInstanced(_renderInfo.mesh, 0, _renderInfo.mat, posMatrixArr, _enemyList.Count);
    }
}

public struct EnemyBaseData
{
    public Vector3 Position;
}