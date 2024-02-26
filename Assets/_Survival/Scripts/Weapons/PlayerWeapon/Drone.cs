using System.Collections.Generic;
using System.Linq;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class Drone : Weapon
{
    private readonly List<Transform> _drones = new();
    private const float _radius = 2f;

    public Drone(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void SpawnWeapon()
    {
        var unit = 360f / _data.NumberOfWeapon;
        for (var i = 0; i < _data.NumberOfWeapon; i++)
        {
            var drone = GameManager.Instance.ObjectPooler.InstantiateDrone();
            drone.SetParent(GameController.Instance.Player.transform, false);
            drone.localPosition = Quaternion.Euler(0f, 0f, unit * i) * Vector2.right * _radius;
            _drones.Add(drone);
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
        if (CurrentLevel == GameManager.Instance.WeaponData.WeaponDatas[(int)Type].BaseWeaponDatas.Length)
            return;
        if (_data.NumberOfWeapon <= _drones.Count)
            return;
        for (var i = 0; i < _data.NumberOfWeapon - _drones.Count; i++)
        {
            var drone = GameManager.Instance.ObjectPooler.InstantiateDrone();
            drone.SetParent(GameController.Instance.Player.transform, false);
            _drones.Add(drone);
        }

        var unit = 360f / _data.NumberOfWeapon;
        for (var i = 0; i < _drones.Count; i++)
        {
            _drones[i].localPosition = Quaternion.Euler(0f, 0f, unit * i) * Vector2.right * _radius;
        }
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (!(_coolDown >= _data.CoolDownTime)) return;
        for (var i = 0; i < _drones.Count; i++)
        {
            var targets =
                GameController.Instance.GridManager.FindTargetsInRange(Attacker.transform.position, _data.Range);
            if (targets.IsNullOrEmpty()) return;
            foreach (var t in targets.Take(_data.MaxTarget))
            {
                var projectileData = new ProjectileData
                {
                    StartPosition = _drones[i].position,
                    Range = _data.Range,
                    MaxTarget = _data.MaxTarget,
                    Attacker = this,
                    Speed = _data.ProjectileSpeed,
                    Damageable = t,
                    ExtraEffectRate = _data.FireChance
                };
                var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.TargetedBullet);
                proj.SetInfo(projectileData);
            }
        }

        _coolDown = 0;
    }

    public override void Destroy()
    {
        for (var i = 0; i < _drones.Count; i++)
        {
            Object.Destroy(_drones[i].gameObject);
        }

        _drones.Clear();
    }
}