using System.Collections.Generic;
using System.Linq;
using Npu.Utilities;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class GridCell
{
    private readonly GridManager _manager;
    private Vector2 _localPosition;
    private readonly int ID;
    private readonly List<FlyweightEnemy> _enemiesInGrid;
    private List<Vector2> _vertices = new(4);
    private Vector2[] _dir = { new(1, 1), new(1, -1), new(-1, -1), new(-1, 1) };
    public Vector2 LocalPosition => _localPosition;

    public List<Vector2> Vertices => _vertices;

    public GridCell(int index, GridManager manager)
    {
        _manager = manager;
        ID = index;
        _localPosition = _manager.GetLocalPosition(ID);
        _enemiesInGrid = new List<FlyweightEnemy>();
        InitVerties();
    }

    private void InitVerties()
    {
        for (var i = 0; i < 4; i++)
        {
            _vertices.Add(LocalPosition + _dir[i] * _manager.GridUnitLength / 2f);
        }
    }

    public void AddEnemy(FlyweightEnemy enemy)
    {
        if (_enemiesInGrid.Contains(enemy))
            return;
        enemy.CurrentIndex = ID;
        _enemiesInGrid.Add(enemy);
    }

    public bool HasEnemy()
    {
        return _enemiesInGrid.Count > 0;
    }

    public void RemoveEnemy(FlyweightEnemy enemy)
    {
        if (_enemiesInGrid.Contains(enemy))
            _enemiesInGrid.Remove(enemy);
    }

    public bool CheckCollideWith(FlyweightEnemy target)
    {
        return _enemiesInGrid.Any(e =>
            Vector2.SqrMagnitude(target.Position - e.Position) <
            e.Data.Size * e.Data.Size + target.Data.Size * target.Data.Size);
    }

    public bool CheckCollideAtPosition(FlyweightEnemy enemy, Vector2 pos)
    {
        for (var i = 0; i < _enemiesInGrid.Count; i++)
        {
            if (_enemiesInGrid[i].ID == enemy.ID)
                continue;
            if (Vector2.SqrMagnitude(pos - _enemiesInGrid[i].Position) <=
                _enemiesInGrid[i].Data.Size * _enemiesInGrid[i].Data.Size)
                return true;
        }

        return false;
    }

    public FlyweightEnemy FindEnemyCollideToPoint(Vector2 point)
    {
        return _enemiesInGrid.FirstOrDefault(t =>
            Vector2.SqrMagnitude(t.Position - point) <= t.Data.Size * t.Data.Size);
    }

    public (FlyweightEnemy, float) FindNearestEnemy(Vector2 fromPos, float range)
    {
        var min = range * range;
        FlyweightEnemy enemy = null;
        for (var i = 0; i < _enemiesInGrid.Count; i++)
        {
            var e = _enemiesInGrid[i];
            var sqrMag = Vector2.SqrMagnitude(e.Position - fromPos);
            if (sqrMag > min) continue;
            min = sqrMag;
            enemy = e;
        }

        return (enemy, min);
    }

    public List<FlyweightEnemy> FindEnemiesInRange(Vector2 point, float radius)
    {
        List<FlyweightEnemy> result = null;
        for (var i = 0; i < _enemiesInGrid.Count; i++)
        {
            var e = _enemiesInGrid[i];
            var sqrMag = Vector2.SqrMagnitude(e.Position - point);
            if (sqrMag > (e.Data.Size + radius) * (e.Data.Size + radius)) continue;
            result ??= new List<FlyweightEnemy>();
            result.Add(e);
        }

        return result;
    }

    public List<FlyweightEnemy> FindEnemyCollideToSegment(Vector2 a1, Vector2 a2)
    {
        if (_enemiesInGrid.IsNullOrEmpty()) return null;
        List<FlyweightEnemy> result = null;
        foreach (var e in _enemiesInGrid)
        {
            var c = Math2DUtils.ClosestPointOnSegment(e.Position, a1, a2);
            if (Vector2.SqrMagnitude(e.Position - c) > e.Data.Size * e.Data.Size) continue;
            result ??= new List<FlyweightEnemy>();
            result.Add(e);
        }

        return result;
    }

    public List<FlyweightEnemy> FindEnemyCollideToBox(Vector2 position, Vector2 dir, float width, float length)
    {
        if (_enemiesInGrid.IsNullOrEmpty()) return null;
        List<FlyweightEnemy> result = null;
        foreach (var e in _enemiesInGrid)
        {
            var c = Math2DUtils.ClosestPointOnSegment(e.Position, position, position + dir * length);
            if (Vector2.SqrMagnitude(e.Position - c) >
                (e.Data.Size + width / 2f) * (e.Data.Size + width / 2f)) continue;
            result ??= new List<FlyweightEnemy>();
            result.Add(e);
        }

        return result;
    }

    public bool IsIntersecWithSegment(Vector2 a1, Vector2 a2)
    {
        if (_manager.IsInsideSquare(a1, _localPosition) || _manager.IsInsideSquare(a2, _localPosition))
            return true;
        for (var i = 0; i < _vertices.Count; i++)
        {
            var j = (i + 1) % _vertices.Count;
            Math2DUtils.SegmentsIntersect(a1, a2, _vertices[i], _vertices[j], out var intersec, out var isOverlap);
            if (intersec != null)
                return true;
        }

        return false;
        // var intersec = Math2DUtils.SegmentPolyIntersect(a1, a2, _vertices);
        // return intersec != null;
    }

    public bool IsIntersecWithBox(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, float width, float length)
    {
        var c1 = Math2DUtils.ClosestPointOnSegment(LocalPosition, (a1 + b1) / 2f, (a2 + b2) / 2f);
        var c2 = Math2DUtils.ClosestPointOnSegment(LocalPosition, (a1 + a2) / 2f, (b1 + b2) / 2f);
        var dirToGrid1 = LocalPosition - c1;
        var dirToGrid2 = LocalPosition - c2;
        var d1 = Vector2.SqrMagnitude(dirToGrid1);
        var d2 = Vector2.SqrMagnitude(dirToGrid2);
        if (d1 <= width * width / 4f && d2 <= length * length / 4f)
        {
            return true;
        }

        return IsIntersecWithSegment(a1, a2) ||
               IsIntersecWithSegment(b1, b2) ||
               IsIntersecWithSegment(a1, b1) ||
               IsIntersecWithSegment(a2, b2);
    }

    public bool IsEnemyCollideWithCircle(Vector2 point, float radius)
    {
        return _enemiesInGrid.Any(e =>
            Vector2.SqrMagnitude(e.Position - point) <= (e.Data.Size + radius) * (e.Data.Size + radius));
    }
}