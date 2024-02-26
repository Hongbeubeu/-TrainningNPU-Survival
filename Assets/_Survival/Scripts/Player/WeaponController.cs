using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Character _attacker;
    private Weapon[] _weapons;
    private int _currentIndex;

    public void SetInfo(Character attacker)
    {
        _weapons = new Weapon[GameManager.Instance.GameConfig.MaxWeaponSlot];
        _attacker = attacker;
        _currentIndex = 0;
    }

    public Weapon GetWeaponAt(int index)
    {
        return _weapons[index];
    }

    public int CountOwnedWeapon()
    {
        for (var i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == null)
                return i;
        }

        return _weapons.Length;
    }

    public void Init()
    {
        AddWeapon((int)_attacker.DefaultWeapon);
    }

    private Weapon GenerateWeapon(int weaponId)
    {
        return (WeaponType)weaponId switch
        {
            WeaponType.Bomb => new Bomb(weaponId, TeamType.Ally),
            WeaponType.Uzi => new Uzi(weaponId, TeamType.Ally),
            WeaponType.ShortGun => new ShortGun(weaponId, TeamType.Ally),
            WeaponType.Laser => new Laser(weaponId, TeamType.Ally),
            WeaponType.Mine => new Mine(weaponId, TeamType.Ally),
            WeaponType.Boomerang => new Boomerang(weaponId, TeamType.Ally),
            WeaponType.FlySaws => new FlyingSaw(weaponId, TeamType.Ally),
            WeaponType.Sword => new Sword(weaponId, TeamType.Ally),
            WeaponType.HeatWave => new HeatWave(weaponId, TeamType.Ally),
            WeaponType.Lightning => new Lightning(weaponId, TeamType.Ally),
            WeaponType.Drone => new Drone(weaponId, TeamType.Ally),
            _ => throw new ArgumentOutOfRangeException(nameof(weaponId), weaponId, null)
        };
    }

    public void AddWeapon(int weaponId)
    {
        GameController.Instance.WeaponSkillRandomizer.UpdateWeapon((WeaponType)weaponId);
        for (var i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == null) continue;
            if (weaponId != (int)_weapons[i].Type) continue;
            _weapons[i].LevelUp();
            return;
        }

        if (_currentIndex == GameManager.Instance.GameConfig.MaxWeaponSlot)
            return;
        _weapons[_currentIndex] = GenerateWeapon(weaponId);
        _weapons[_currentIndex].SetInfo(_attacker);
        _weapons[_currentIndex].SpawnWeapon();
        _currentIndex++;
    }

    private void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Pause)
            return;
        for (var i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null)
            {
                _weapons[i].OnUpdate(Time.deltaTime);
            }
        }
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

    public int GetCurrentLevelWeapon(WeaponType type)
    {
        if (_weapons.Length <= 0)
            return 0;
        for (var i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == null) continue;
            if (_weapons[i].Type == type)
                return _weapons[i].CurrentLevel + 1;
        }

        return 0;
    }
}