using System;
using Ultimate.Core.Runtime.WeightedRandomization;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    private Enemy _attacker;
    private EnemyWeapon[] _weapons;
    private int _currentIndex;
    private int _currentWeaponIndex;
    private WeightedRandomizer<int> _randomizer = new();

    public void SetInfo(Enemy attacker)
    {
        _attacker = attacker;
        _weapons = new EnemyWeapon[GameManager.Instance.GameConfig.BossMaxWeaponSlot];
    }

    public void AddWeapon(int weaponId)
    {
        if (_currentIndex == GameManager.Instance.GameConfig.BossMaxWeaponSlot)
            return;
        for (var i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == null) continue;
            if (weaponId != (int)_weapons[i].Type) continue;
            return;
        }

        _weapons[_currentIndex] = GenerateWeapon(weaponId);
        _weapons[_currentIndex].SetInfo(_attacker, this);
        _weapons[_currentIndex].SpawnWeapon();
        _currentIndex++;
    }

    public void RandomWeapon()
    {
        _randomizer.ClearElementList();
        for (var i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == null) continue;
            _randomizer.AddOrUpdateValue(i, 1);
        }

        _currentWeaponIndex = _randomizer.GetRandom();
    }

    private EnemyWeapon GenerateWeapon(int weaponId)
    {
        Debug.LogError($"boss add weapon - {(EnemyWeaponType)weaponId}");
        return (EnemyWeaponType)weaponId switch
        {
            EnemyWeaponType.Boss_FlowerGun => new FlowerGun(weaponId, TeamType.Enemy),
            EnemyWeaponType.Boss_HeavyPunch => new HeavyPunch(weaponId, TeamType.Enemy),
            EnemyWeaponType.Boss_CrazyChase => new CrazyChase(weaponId, TeamType.Enemy),
            _ => throw new ArgumentOutOfRangeException(nameof(weaponId), weaponId, null)
        };
    }


    private void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Pause)
            return;
        _weapons[_currentWeaponIndex]?.OnUpdate(Time.deltaTime);
        // for (var i = 0; i < _weapons.Length; i++)
        // {
        //     if (_weapons[i] != null)
        //     {
        //         _weapons[i].OnUpdate(Time.deltaTime);
        //     }
        // }
    }

    public void ResetData()
    {
        _currentIndex = 0;
        for (var i = 0; i < _weapons.Length; i++)
        {
            _weapons[i]?.Destroy();
            _weapons[i] = null;
        }
    }
}