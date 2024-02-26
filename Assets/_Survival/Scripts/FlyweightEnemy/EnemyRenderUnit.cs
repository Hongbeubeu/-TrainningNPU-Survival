using System.Collections.Generic;
using UnityEngine;

public class EnemyRenderUnit
{
    private EnemyRenderController _controller;
    private RenderInfo _renderInfo;
    private Vector4[] posDirArr;
    private Matrix4x4[] posMatrixArr;

    private readonly List<FlyweightEnemy> _enemies = new();

    public bool Contains(FlyweightEnemy e)
    {
        return _enemies.Contains(e);
    }

    public bool HasEnemy()
    {
        return _enemies.Count > 0;
    }

    public void Reset()
    {
        for (var i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].Destroy();
        }

        _enemies.Clear();
    }

    public void AddEnemy(FlyweightEnemy enemy)
    {
        if (_enemies.Contains(enemy))
            return;
        _enemies.Add(enemy);
        GameController.Instance.GridManager.Add(enemy);
    }

    public void RemoveEnemy(FlyweightEnemy enemy)
    {
        if (!_enemies.Contains(enemy)) return;
        _enemies.Remove(enemy);
        _controller.Factory.ReturnEnemy(enemy);
        GameController.Instance.CheckWin();
    }

    public void Init(EnemyRenderController controller)
    {
        _controller = controller;
        posDirArr = new Vector4[EnemyRenderController.batchSize];
        posMatrixArr = new Matrix4x4[EnemyRenderController.batchSize];
    }

    public void InitRenderInfo(Material material, Sprite sprite)
    {
        _renderInfo = RenderInfo.FromSprite(material, sprite);
    }

    public void Update(float dt)
    {
        for (var i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].DoUpdate(dt, ((Vector2)_controller.Target.position - _enemies[i].Position).normalized);
        }
    }

    public void Render(Camera c)
    {
        for (var done = 0; done < _enemies.Count; done += EnemyRenderController.batchSize)
        {
            var run = Mathf.Min(_enemies.Count - done, EnemyRenderController.batchSize);
            for (var batchInd = 0; batchInd < run; ++batchInd)
            {
                var obj = _enemies[done + batchInd];
                var objPosition = obj.Position;
                var objRotation = Quaternion.Euler(0, 0, obj.Rotation);
                var objScale = Vector3.one * obj.Scale;
                objScale.x = GameController.Instance.Player.transform.position.x - obj.Position.x < 0 ? 1 : -1;

                posMatrixArr[batchInd] = Matrix4x4.TRS(objPosition, objRotation, objScale);
            }

            _controller.CallRender(c, _renderInfo, posMatrixArr, run);
        }
    }
}