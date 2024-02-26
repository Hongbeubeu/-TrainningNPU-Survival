using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class EnemyRenderController : MonoBehaviour
{
    #region Property Render

    [SerializeField] private Mesh testMesh;
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Material _material;
    public static readonly int posDirPropertyId = Shader.PropertyToID("posDirBuffer");
    public MaterialPropertyBlock pb;
    public const int batchSize = 100;
    private RenderInfo _renderInfo;
    public string layerRenderName;
    public int layerRender;
    private readonly Dictionary<EnemyTier, EnemyRenderUnit> _enemyRenderUnits = new();

    #endregion

    private FlyweightEnemyFactory _factory;
    private Transform _target;
    private bool _isCalculate;

    public Transform Target => _target;
    public FlyweightEnemyFactory Factory => _factory;

    public void SetTarget(Transform target)
    {
        _target = target;
        _isCalculate = true;
    }

    public bool HasEnemy()
    {
        foreach (var es in _enemyRenderUnits)
        {
            if (!es.Value.HasEnemy())
                continue;
            return true;
        }

        return false;
    }

    private void Start()
    {
        _factory = new FlyweightEnemyFactory();
        pb = new MaterialPropertyBlock();
        layerRender = LayerMask.NameToLayer(layerRenderName);
        Camera.onPreCull += Render;
        _isCalculate = false;
    }

    public void CreateEnemy(EnemyTier tier, int level, int id)
    {
        var e = _factory.GetEnemy();
        e.SetInfo(_factory.GetEnemySharedData(tier, level), this);
        e.Position = (Vector2)_target.position + Random.insideUnitCircle.normalized *
            Random.Range(GameManager.Instance.GameConfig.MinSpawnRange, GameManager.Instance.GameConfig.MaxSpawnRange);
        e.ID = id;
        AddEnemy(e);
    }

    public void Reset()
    {
        foreach (var e in _enemyRenderUnits)
        {
            e.Value.Reset();
        }

        _enemyRenderUnits.Clear();
    }

    private void Update()
    {
        if (!_isCalculate) return;
        foreach (var u in _enemyRenderUnits)
        {
            u.Value.Update(Time.deltaTime);
        }
    }

    private void AddEnemy(FlyweightEnemy enemy)
    {
        var tier = enemy.Data.Tier;
        if (!_enemyRenderUnits.ContainsKey(tier))
        {
            _enemyRenderUnits.Add(tier, new EnemyRenderUnit());
            _enemyRenderUnits[tier].Init(this);
            _enemyRenderUnits[tier].InitRenderInfo(_material, _sprites[(int)tier]);
        }

        if (_enemyRenderUnits[tier].Contains(enemy))
            return;
        _enemyRenderUnits[tier].AddEnemy(enemy);
    }

    public void RemoveEnemy(FlyweightEnemy enemy)
    {
        var tier = enemy.Data.Tier;
        if (!_enemyRenderUnits.ContainsKey(tier))
            return;
        _enemyRenderUnits[tier].RemoveEnemy(enemy);
    }

    private void Render(Camera c)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        foreach (var u in _enemyRenderUnits)
        {
            u.Value.Render(c);
        }

        // for (var done = 0; done < Enemies.Count; done += batchSize)
        // {
        //     var run = Math.Min(Enemies.Count - done, batchSize);
        //     for (var batchInd = 0; batchInd < run; ++batchInd)
        //     {
        //         var obj = Enemies[done + batchInd];
        //         posDirArr[batchInd] = new Vector4(obj.Position.x, obj.Position.y,
        //             Mathf.Cos(obj.Rotation) * obj.Scale, Mathf.Sin(obj.Rotation) * obj.Scale);
        //         ref var m = ref posMatrixArr[batchInd];
        //
        //         m.m00 = m.m11 = Mathf.Cos(obj.Rotation);
        //         m.m01 = -(m.m10 = Mathf.Sin(obj.Rotation));
        //         m.m22 = m.m33 = 1;
        //         m.m03 = obj.Position.x;
        //         m.m13 = obj.Position.y;
        //     }
        //
        //     pb.SetVectorArray(posDirPropertyId, posDirArr);
        //     CallRender(c, posMatrixArr, run);
        // }
    }

    //Use this for legacy GPU support or WebGL support
    public void CallRender(Camera c, RenderInfo renderInfo, Matrix4x4[] posMatrixArr, int count)
    {
        Graphics.DrawMeshInstanced(renderInfo.mesh, 0, renderInfo.mat,
            posMatrixArr,
            count: count,
            properties: pb,
            castShadows: ShadowCastingMode.Off,
            receiveShadows: false,
            layer: layerRender,
            camera: c);
    }
}