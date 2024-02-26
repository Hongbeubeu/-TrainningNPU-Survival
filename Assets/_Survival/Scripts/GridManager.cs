using System.Collections.Generic;
using System.Linq;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _gridSize;
    [SerializeField] private int _gridUnitLength;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Dictionary<int, GridCell> _grids;
    private int[] _spriralTravelsalIndexs;
    private List<SpriteRenderer> _grounds;
    [SerializeField] private bool _isShowGridView;

    public int GridSize => _gridSize;
    public int GridUnitLength => _gridUnitLength;


    public void Start()
    {
        _grids = new Dictionary<int, GridCell>();
        _spriralTravelsalIndexs = new int[_gridSize * _gridSize];
        InitGrid();
        InitGridView();
    }

    private void InitGrid()
    {
        for (var i = 0; i < _gridSize * _gridSize; i++)
        {
            _grids.Add(i, new GridCell(i, this));
        }
    }

    private bool IsGridInRange(int gridIndex, float range)
    {
        var gridPos = GetWordPosition(gridIndex);
        var point = gridPos.normalized * range;
        return Vector2.SqrMagnitude(gridPos) <= range * range || IsInsideSquare(point, gridPos);
    }

    public bool IsInsideSquare(Vector2 point, Vector2 squarePos)
    {
        var haflSize = _gridUnitLength / 2f;
        return point.x >= squarePos.x - haflSize
               && point.x <= squarePos.x + haflSize
               && point.y >= squarePos.y - haflSize
               && point.y <= squarePos.y + haflSize;
    }

    // private void InitTravelsal()
    // {
    //     var row = _gridSize / 2;
    //     var col = row;
    //     int[] rowDir = { -1, 0, 1, 0 };
    //     int[] colDir = { 0, 1, 0, -1 };
    //     var dir = 0;
    //     for (var i = 0; i < _gridSize * _gridSize; i++)
    //     {
    //         _spriralTravelsalIndexs[i] = row * _gridSize + col;
    //         var nextRow = row + rowDir[dir];
    //         var nextCol = col + colDir[dir];
    //
    //         if (nextRow < 0 || nextRow >= _gridSize || nextCol < 0 || nextCol >= _gridSize || _spriralTravelsalIndexs[])
    //         {
    //             dir = (dir + 1) % 4;
    //             nextRow = row + rowDir[dir];
    //             nextCol = col + colDir[dir];
    //         }
    //
    //         row = nextRow;
    //         col = nextCol;
    //     }
    // }


    public int GetGridIndex(Vector2 worldPosition)
    {
        //cache lại position tại mỗi frame
        worldPosition -= (Vector2)transform.position;
        worldPosition.x += _gridUnitLength * _gridSize / 2f;
        worldPosition.y += _gridUnitLength * _gridSize / 2f;
        var pos = Vector2Int.zero;
        pos.x = (int)worldPosition.x / _gridUnitLength;
        pos.x = Mathf.Clamp(pos.x, 0, _gridSize - 1);
        pos.y = (int)worldPosition.y / _gridUnitLength;
        pos.y = Mathf.Clamp(pos.y, 0, _gridSize - 1);
        return GridPos2Index(pos);
    }

    public Vector2Int Index2GridPos(int index)
    {
        var res = Vector2Int.zero;
        res.x = index % _gridSize;
        res.y = index / _gridSize;
        return res;
    }

    public int GridPos2Index(int x, int y)
    {
        return y * _gridSize + x;
    }

    public int GridPos2Index(Vector2Int pos)
    {
        return pos.y * _gridSize + pos.x;
    }

    public Vector2 GetWordPosition(int index)
    {
        return GetLocalPosition(index) + (Vector2)transform.position;
    }

    public Vector2 GetLocalPosition(int index)
    {
        var gridPos = Index2GridPos(index);
        var result = Vector2.zero;
        result.x = gridPos.x * _gridUnitLength + _gridUnitLength / 2f - _gridUnitLength * _gridSize / 2f;
        result.y = gridPos.y * _gridUnitLength + _gridUnitLength / 2f - _gridUnitLength * _gridSize / 2f;
        return result;
    }

    public Vector2 ConvertToLocalPosition(Vector2 worldPosition)
    {
        return worldPosition - (Vector2)transform.position;
    }

    public void Add(FlyweightEnemy enemy)
    {
        var gridIndex = GetGridIndex(enemy.Position);
        if (!_grids.ContainsKey(gridIndex))
        {
            var grid = new GridCell(gridIndex, this);
            grid.AddEnemy(enemy);
            _grids[gridIndex] = grid;
        }

        _grids[gridIndex].AddEnemy(enemy);
    }

    public void Remove(FlyweightEnemy enemy)
    {
        var gridIndex = GetGridIndex(enemy.Position);
        _grids[gridIndex].RemoveEnemy(enemy);
    }

    public void UpdateEnemyInGrid(int preIndex, FlyweightEnemy enemy)
    {
        var index = GetGridIndex(enemy.Position);
        if (index == preIndex)
            return;
        if (_grids.ContainsKey(preIndex))
        {
            _grids[preIndex].RemoveEnemy(enemy);
        }

        Add(enemy);
    }

    public bool CheckCollide(FlyweightEnemy e, Vector2 dir)
    {
        var point = e.Position + dir * e.Data.Size + Vector2.Perpendicular(dir) * e.Data.Size;
        var index = GetGridIndex(point);
        var result1 = _grids.ContainsKey(index) && _grids[index].CheckCollideAtPosition(e, point);
        if (result1)
            return true;
        point = e.Position + dir * e.Data.Size + -Vector2.Perpendicular(dir) * e.Data.Size;
        index = GetGridIndex(point);
        var result2 = _grids.ContainsKey(index) && _grids[index].CheckCollideAtPosition(e, point);
        return result2;
    }

    public bool IsEnemyCollideWithPoint(Vector2 point, float radius)
    {
        var listGrid = FindGridInRange(point, radius);
        return !listGrid.IsNullOrEmpty() && listGrid.Any(t => _grids[t].IsEnemyCollideWithCircle(point, radius));
    }

    public (bool, IDamageable) CheckCollideBullet(Projectile projectile)
    {
        if (projectile._data.TeamType == TeamType.Ally)
        {
            var currentIndex = GetGridIndex(projectile.transform.position);
            if (!_grids.ContainsKey(currentIndex)) return (false, null);
            var e = _grids[currentIndex].FindEnemyCollideToPoint(projectile.transform.position);
            return (e != null, e);
        }

        var playerTrans = GameController.Instance.Player.transform;
        if (Vector2.SqrMagnitude(projectile.transform.position - playerTrans.position) <=
            (projectile._data.Size + playerTrans.localScale.x) * (projectile._data.Size + playerTrans.localScale.x))
        {
            return (true, GameController.Instance.Player.Damageable);
        }

        return (false, null);
    }

    private List<int> FindGridInRange(Vector2 position, float range)
    {
        List<int> result = null;
        var sqrRadius = range * range;
        foreach (var g in _grids)
        {
            if (!g.Value.HasEnemy()) continue;
            var point = GetWordPosition(g.Key) - position;
            if (point.SqrMagnitude() > sqrRadius &&
                !IsInsideSquare(point.normalized * range + position, GetWordPosition(g.Key))) continue;
            result ??= new List<int>();
            result.Add(g.Key);
        }

        return result;
    }

    public FlyweightEnemy FindNearestTargetInRange(float range)
    {
        var min = range * range;
        FlyweightEnemy enemy = null;
        for (var i = 0; i < _gridSize * _gridSize; i++)
        {
            if (!_grids.ContainsKey(i) || !IsGridInRange(i, range))
                continue;
            var (e, m) = _grids[i].FindNearestEnemy(transform.position, range);
            if (e == null || m > min)
                continue;
            enemy = e;
            min = m;
        }

        return enemy;
    }

    public List<FlyweightEnemy> FindTargetsInRange(Vector2 position, float range)
    {
        List<FlyweightEnemy> result = null;
        for (var i = 0; i < _gridSize * _gridSize; i++)
        {
            ChangeColorGrid(i, false);
        }

        var gridsInRange = FindGridInRange(position, range);
        if (gridsInRange.IsNullOrEmpty()) return null;
        for (var i = 0; i < _gridSize * _gridSize; i++)
        {
            ChangeColorGrid(i, gridsInRange.Contains(i));
        }

        foreach (var e in gridsInRange)
        {
            var enemies = _grids[e].FindEnemiesInRange(position, range);
            if (enemies.IsNullOrEmpty()) continue;
            result ??= new List<FlyweightEnemy>();
            result.AddRange(enemies);
        }

        return result;
    }


    #region Raycast

    public List<FlyweightEnemy> CircleCast(Vector2 position, float radius)
    {
        var listGridInRange = FindGridInRange(position, radius);
        List<FlyweightEnemy> result = null;
        if (listGridInRange.IsNullOrEmpty()) return null;
        for (var i = 0; i < listGridInRange.Count; i++)
        {
            var es = _grids[listGridInRange[i]].FindEnemiesInRange(position, radius);
            if (es.IsNullOrEmpty()) continue;
            result ??= new List<FlyweightEnemy>();
            result.AddRange(es);
        }

        return result;
    }

    public List<FlyweightEnemy> SegmentCast(Vector2 a1, Vector2 a2)
    {
        var listGridIntersec = FindGridIntersecSegment(a1, a2);
        if (listGridIntersec.IsNullOrEmpty()) return null;
        // for (var i = 0; i < _gridSize * _gridSize; i++)
        // {
        //     ChangeColorGrid(i, listGridIntersec.Contains(i));
        // }

        List<FlyweightEnemy> result = null;
        foreach (var g in listGridIntersec)
        {
            if (!_grids.ContainsKey(g)) continue;
            var es = _grids[g].FindEnemyCollideToSegment(a1, a2);
            if (es.IsNullOrEmpty()) continue;
            result ??= new List<FlyweightEnemy>();
            result.AddRange(es);
        }

        return result;
    }

    public List<FlyweightEnemy> BoxCast(Vector2 position, Vector2 dir, float width, float length)
    {
        var listGridIntersec = FindGridIntersecBox(position, dir, width, length);
        if (listGridIntersec.IsNullOrEmpty()) return null;
        // for (var i = 0; i < _gridSize * _gridSize; i++)
        // {
        //     ChangeColorGrid(i, listGridIntersec.Contains(i));
        // }

        List<FlyweightEnemy> result = null;
        foreach (var g in listGridIntersec)
        {
            if (!_grids.ContainsKey(g)) continue;
            var es = _grids[g].FindEnemyCollideToBox(position, dir, width, length);
            if (es.IsNullOrEmpty()) continue;
            result ??= new List<FlyweightEnemy>();
            result.AddRange(es);
        }

        return result;
    }

    private List<int> FindGridIntersecSegment(Vector2 a1, Vector2 a2)
    {
        List<int> result = null;
        a1 = ConvertToLocalPosition(a1);
        a2 = ConvertToLocalPosition(a2);
        for (var i = 0; i < _gridSize * _gridSize; i++)
        {
            if (!_grids[i].IsIntersecWithSegment(a1, a2)) continue;
            result ??= new List<int>();
            result.Add(i);
        }

        return result;
    }

    private List<int> FindGridIntersecBox(Vector2 position, Vector2 dir, float width, float length)
    {
        List<int> result = null;
        dir = dir.normalized;
        var a1 = position + Vector2.Perpendicular(dir) * width / 2f;
        a1 = ConvertToLocalPosition(a1);
        var b1 = position - Vector2.Perpendicular(dir) * width / 2f;
        b1 = ConvertToLocalPosition(b1);
        var a2 = a1 + dir * length;
        var b2 = b1 + dir * length;
        for (var i = 0; i < _gridSize * _gridSize; i++)
        {
            if (!_grids[i].IsIntersecWithBox(a1, a2, b1, b2, width, length)) continue;
            result ??= new List<int>();
            result.Add(i);
        }

        return result;
    }

    #endregion

    #region GridView Extention

    private void InitGridView()
    {
        if (!_isShowGridView) return;
        _grounds = new List<SpriteRenderer>(_gridSize * _gridSize);
        var c = _spriteRenderer.color;
        var alpha = 0.1f;
        var delta = 0.9f / (_gridSize * _gridSize);
        for (var i = 0; i < _gridSize * _gridSize; i++)
        {
            var pos = GetWordPosition(i);
            var g = Instantiate(_spriteRenderer, transform, false);
            g.transform.localPosition = pos;
            var scale = Vector3.zero;
            scale.x = _gridUnitLength - 0.1f;
            scale.y = _gridUnitLength - 0.1f;
            scale.z = 1;
            alpha += delta;
            c.a = alpha;
            g.transform.localScale = scale;
            g.color = c;
            _grounds.Add(g);
        }
    }

    private void ChangeColorGrid(int index, bool hasEnemy)
    {
        if (!_isShowGridView) return;
        var color = hasEnemy ? Color.blue : Color.gray;
        color.a = 0.3f;
        _grounds[index].color = color;
    }

    #endregion
}