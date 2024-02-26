using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FlyingSaw : Weapon
{
    private readonly List<Saw> _saws = new();
    private Transform parentSaw;

    public FlyingSaw(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void SpawnWeapon()
    {
        parentSaw = new GameObject("parent").transform;
        parentSaw.SetParent(GameController.Instance.Player.transform, false);
        parentSaw.DOLocalRotate(-Vector3.forward * 180f, 1 / _data.ProjectileSpeed).SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
        var unit = 360f / _data.NumberOfWeapon;
        var dir = Vector2.right;
        for (var i = 0; i < _data.NumberOfWeapon; i++)
        {
            var projectileData = new ProjectileData
            {
                StartPosition = GameController.Instance.Player.transform.position,
                Range = _data.ProjectileRange,
                MaxTarget = _data.MaxTarget,
                Attacker = this,
                Speed = _data.ProjectileSpeed,
                Size = _data.TriggerRadius,
                ExtraEffectRate = _data.FireChance
            };
            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.Saw);
            proj.SetInfo(projectileData);
            proj.transform.SetParent(parentSaw);
            var newDir = Quaternion.Euler(0f, 0f, unit * i) * dir;
            proj.transform.localPosition = newDir * _data.Range;
            _saws.Add(proj as Saw);
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
        if (CurrentLevel == GameManager.Instance.WeaponData.WeaponDatas[(int)Type].BaseWeaponDatas.Length)
            return;
        if (_data.NumberOfWeapon <= _saws.Count) return;
        parentSaw.DOKill();
        parentSaw.localRotation = Quaternion.identity;
        parentSaw.DOLocalRotate(-Vector3.forward * 180f, 1 / _data.ProjectileSpeed).SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
        for (var i = 0; i < _data.NumberOfWeapon; i++)
        {
            var projectileData = new ProjectileData
            {
                Range = _data.ProjectileRange,
                MaxTarget = _data.MaxTarget,
                Attacker = this,
                Speed = _data.ProjectileSpeed,
                Size = _data.TriggerRadius,
                ExtraEffectRate = _data.FireChance
            };
            if (i < _saws.Count)
            {
                _saws[i].UpdateData(projectileData);
                continue;
            }

            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.Saw);
            proj.SetInfo(projectileData);
            proj.transform.SetParent(parentSaw);
            _saws.Add(proj as Saw);
        }

        for (var i = 0; i < _saws.Count; i++)
        {
            var unit = 360f / _data.NumberOfWeapon;
            var dir = Vector2.right;
            var newDir = Quaternion.Euler(0f, 0f, unit * i) * dir;
            _saws[i].transform.localPosition = newDir * _data.Range;
        }
    }

    public override void Destroy()
    {
        for (var i = 0; i < _saws.Count; i++)
        {
            _saws[i].Destroy();
        }

        _saws.Clear();
    }
}