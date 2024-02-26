using System;
using System.Collections.Generic;
using System.Linq;
using Ultimate.Core.Runtime.EventManager;
using Ultimate.Core.Runtime.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : Singleton<GameController>
{
    public Player Player;
    private GameState _currentGameState;
    private readonly ChangeGameStateEvent OnChangeGameState = new();
    private readonly GameResetEvent OnGameReset = new();
    private readonly List<IDamageable> _damageables = new();
    private CollectableController _collectableController;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private BoxSpawner _boxSpawner;
    public EnemyRenderController EnemyRenderController;
    public InfiniteBackgroundComponent InfiniteBackground;
    public bool IsDoneSpawning;
    private int _currentLevel;
    public WeaponSkillRandomizer WeaponSkillRandomizer;
    private bool _isEndGame;
    public GridManager GridManager;

    public GameState CurrentGameState
    {
        get => _currentGameState;
        set
        {
            _currentGameState = value;
            OnChangeGameState.CurrentState = _currentGameState;
            EventManager.Instance.Raise(OnChangeGameState);
            Time.timeScale = _currentGameState == GameState.Play ? 1 : 0;
        }
    }

    public bool IsEndGame
    {
        get => _isEndGame;
        set => _isEndGame = value;
    }

    public override void Init()
    {
    }

    public void StartGame(int level)
    {
        _currentLevel = level;
        CurrentGameState = GameState.Play;
        _enemySpawner?.StartSpawnEnemies(level);
        EnemyRenderController.SetTarget(Player.transform);
        IsDoneSpawning = false;
        if (WeaponSkillRandomizer == null)
        {
            WeaponSkillRandomizer = new WeaponSkillRandomizer();
        }
        else
        {
            WeaponSkillRandomizer.CleanUp();
            WeaponSkillRandomizer.Init();
        }

        Player.WeaponController.Init();
        _isEndGame = false;
        _collectableController ??= new CollectableController();
        _collectableController.Init();
        // _boxSpawner?.SetInfo();
    }

    private void Update()
    {
        _collectableController?.Update();
    }

    public bool CheckWin()
    {
        if (!IsDoneSpawning)
            return false;
        if (!EnemyRenderController.HasEnemy()) return false;
        CurrentGameState = GameState.Pause;
        _isEndGame = true;
        return true;
    }

    public void ResetGame()
    {
        CurrentGameState = GameState.Pause;
        _enemySpawner.StopSpawnEnemies();
        Player.ResetData();
        _collectableController.Reset();

        // for (var i = 0; i < _damageables.Count; i++)
        // {
        //     _damageables[i]?.Destroy();
        // }
        //
        // _damageables.Clear();
        EnemyRenderController.Reset();
        IsDoneSpawning = false;
        EventManager.Instance.Raise(OnGameReset);
    }

    public void AddDamageable(IDamageable enemy)
    {
        if (!_damageables.Contains(enemy))
        {
            _damageables.Add(enemy);
        }
    }

    public void RemoveDamageable(IDamageable enemy)
    {
        if (!_damageables.Contains(enemy)) return;
        _damageables.Remove(enemy);
        if (!_isEndGame && CheckWin())
        {
            UIController.Instance.InGamePanel.EndGamePanel.SetInfo(true);
            if (Player.Data.LevelUnlocked == _currentLevel)
            {
                Player.Data.LevelUnlocked++;
            }

            Player.ResetData();
            ResetGame();
        }
    }

    public WeaponType RandomWeapon()
    {
        var number = GameManager.Instance.WeaponData.WeaponDatas.Length;
        var rand = Random.Range(0, number);
        return (WeaponType)rand;
    }

    public SkillType RandomSkill()
    {
        var number = GameManager.Instance.WeaponData.WeaponDatas.Length;
        var rand = Random.Range(0, number);
        return (SkillType)rand;
    }

    public void AddCollectable(ICollectable collectable)
    {
        _collectableController.Add(collectable);
    }

    public void RemoveCollectable(ICollectable collectable)
    {
        _collectableController.Remove(collectable);
    }

    // public IDamageable GetNearestTarget()
    // {
    //     if (!_enemies.Any()) return null;
    //     var playerPos = Player.transform.position;
    //     return _enemies.OrderBy(e => Vector2.Distance(e.GetTransform().position, playerPos)).First();
    // }

    public IDamageable GetNearestTarget(float range)
    {
        if (!_damageables.Any()) return null;
        var playerPos = Player.transform.position;
        var target = _damageables.OrderBy(e => Vector2.Distance(e.GetTransform().position, playerPos)).First();
        return Vector2.Distance(target.GetTransform().position, playerPos) <= range ? target : null;
    }

    public FlyweightEnemy FindNearestEnemy(float range)
    {
        return GridManager.FindNearestTargetInRange(range);
    }

    public List<IDamageable> GetTargetsInRange(float range, int maxTarget)
    {
        if (!_damageables.Any()) return null;
        var playerPos = Player.transform.position;
        var target = _damageables.OrderBy(e => Vector2.Distance(e.GetTransform().position, playerPos)).Take(maxTarget);
        var res = target.Where(t => Vector2.Distance(t.GetTransform().position, playerPos) <= range)
            .ToList();
        return res;
    }

    public IDamageable GetTargetInRange(float range)
    {
        if (!_damageables.Any()) return null;
        var playerPos = Player.transform.position;
        var target = _damageables.Where(t => Vector2.Distance(t.GetTransform().position, playerPos) <= range).Take(1)
            .DefaultIfEmpty().First();
        return target;
    }
}