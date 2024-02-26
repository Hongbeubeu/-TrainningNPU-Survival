using System.Collections;
using DG.Tweening;
using UnityEngine;

public class FlowerGun : EnemyWeapon
{
    private bool _isAttacking;

    public FlowerGun(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        if (_isAttacking)
        {
            SpawnProjectiles((int)(_data.DurationTime * _data.AttackSpeed), 1f / _data.AttackSpeed);
            _isAttacking = false;
            return;
        }

        _coolDown -= dt;
        if (_coolDown > 0) return;
        _coolDown = _data.CoolDownTime + _data.DurationTime;
        _isAttacking = true;
    }

    private void SpawnProjectiles(int number, float deltaTime)
    {
        Attacker.SetCanMove(false);
        DOVirtual.DelayedCall(number * deltaTime, () =>
        {
            Attacker.SetCanMove(true);
            _controller.RandomWeapon();
        });
        for (var j = 0; j < number; j++)
        {
            DOVirtual.DelayedCall(deltaTime * j, () =>
            {
                var dir = Random.insideUnitCircle.normalized;
                var unit = _data.Angle / _data.NumberOfProjectilePerHit;
                for (var i = 0; i < _data.NumberOfProjectilePerHit; i++)
                {
                    var newDir = Quaternion.Euler(0, 0, -unit * i) * dir;
                    var projectileData = new ProjectileData
                    {
                        StartPosition = Attacker.transform.position,
                        Range = _data.Range,
                        Attacker = this,
                        Speed = _data.ProjectileSpeed,
                        Direction = newDir.normalized
                    };
                    var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.EnemyBullet1);
                    proj.SetInfo(projectileData);
                }
            });
        }
    }
}