using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

public class ManyObjectHolder : MonoBehaviour
{
    private class FObject
    {
        private static readonly Random r = new Random();
        public Vector2 position;
        public readonly float scale;
        public float speed;
        private readonly Vector2 velocity;
        public float rotation;
        public float time;

        public FObject()
        {
            position = new Vector2((float)r.NextDouble() * 10f - 5f, (float)r.NextDouble() * 8f - 4f);
            velocity = new Vector2((float)r.NextDouble() * 0.4f - 0.2f, (float)r.NextDouble() * 0.4f - 0.2f);
            rotation = 0;
            scale = 1f;
            speed = 1;
            time = (float)r.NextDouble() * 6f;
        }

        public void DoUpdate(float dT, Vector2 dir)
        {
            position += dir * (speed * dT);
            // rotation += rotationRate * dT;
            // time += dT;
        }
    }

    private static readonly int posDirPropertyId = Shader.PropertyToID("posDirBuffer");

    private MaterialPropertyBlock pb;
    private readonly Vector4[] posDirArr = new Vector4[batchSize];
    private readonly Matrix4x4[] posMatrixArr = new Matrix4x4[batchSize];
    private const int batchSize = 100;
    public int instanceCount;

    public Sprite sprite;
    public Material baseMaterial;
    private RenderInfo ri;
    public string layerRenderName;
    private int layerRender;
    private FObject[] objects;

    private void Start()
    {
        pb = new MaterialPropertyBlock();
        layerRender = LayerMask.NameToLayer(layerRenderName);
        ri = RenderInfo.FromSprite(baseMaterial, sprite);
        Camera.onPreCull += RenderMe;
        objects = new FObject[instanceCount];
        for (int ii = 0; ii < instanceCount; ++ii)
        {
            objects[ii] = new FObject();
        }

        Application.targetFrameRate = 1000;
    }

    private void Update()
    {
        float dT = Time.deltaTime;
        for (int ii = 0; ii < instanceCount; ++ii)
        {
            objects[ii].DoUpdate(dT, (Vector2.zero - objects[ii].position).normalized);
        }
    }

    private float Smoothstep(float low, float high, float t)
    {
        t = Mathf.Clamp01((t - low) / (high - low));
        return t * t * (3 - 2 * t);
    }

    private void RenderMe(Camera c)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        for (int done = 0; done < instanceCount; done += batchSize)
        {
            int run = Math.Min(instanceCount - done, batchSize);
            for (int batchInd = 0; batchInd < run; ++batchInd)
            {
                var obj = objects[done + batchInd];
                posDirArr[batchInd] = new Vector4(obj.position.x, obj.position.y,
                    Mathf.Cos(obj.rotation) * obj.scale, Mathf.Sin(obj.rotation) * obj.scale);
                ref var m = ref posMatrixArr[batchInd];

                m.m00 = m.m11 = Mathf.Cos(obj.rotation);
                m.m01 = -(m.m10 = Mathf.Sin(obj.rotation));
                m.m22 = m.m33 = 1;
                m.m03 = obj.position.x;
                m.m13 = obj.position.y;
            }

            pb.SetVectorArray(posDirPropertyId, posDirArr);
            //CallRender(c, run);
            CallLegacyRender(c, run);
        }
    }


    private void CallRender(Camera c, int count)
    {
        Graphics.DrawMeshInstancedProcedural(ri.mesh, 0, ri.mat,
            bounds: new Bounds(Vector3.zero, Vector3.one * 1000f),
            count: count,
            properties: pb,
            castShadows: ShadowCastingMode.Off,
            receiveShadows: false,
            layer: layerRender,
            camera: c);
    }

    //Use this for legacy GPU support or WebGL support
    private void CallLegacyRender(Camera c, int count)
    {
        Graphics.DrawMeshInstanced(ri.mesh, 0, ri.mat,
            posMatrixArr,
            count: count,
            properties: pb,
            castShadows: ShadowCastingMode.Off,
            receiveShadows: false,
            layer: layerRender,
            camera: c);
    }
}