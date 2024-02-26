using System.Linq;
using UnityEngine;

public readonly struct RenderInfo
{
    private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");
    public readonly Mesh mesh;
    public readonly Material mat;

    public RenderInfo(Mesh m, Material material)
    {
        mesh = m;
        mat = material;
    }

    public static RenderInfo FromSprite(Material baseMaterial, Sprite s)
    {
        var renderMaterial = Object.Instantiate(baseMaterial);
        renderMaterial.enableInstancing = true;
        renderMaterial.SetTexture(MainTexPropertyId, s.texture);
        Mesh m = new Mesh
        {
            vertices = s.vertices.Select(v => (Vector3)v).ToArray(),
            triangles = s.triangles.Select(t => (int)t).ToArray(),
            uv = s.uv
        };
        return new RenderInfo(m, renderMaterial);
    }
}