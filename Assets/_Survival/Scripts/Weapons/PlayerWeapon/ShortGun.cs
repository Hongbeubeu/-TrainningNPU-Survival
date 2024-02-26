using UnityEngine;

public class ShortGun : Weapon
{
    public ShortGun(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (!(_coolDown >= _data.CoolDownTime)) return;
        var target = GameController.Instance.GridManager.FindNearestTargetInRange(_data.Range);
        if (target == null) return;
        var dir = (target.Position - (Vector2)GameController.Instance.Player.transform.position).normalized;
        dir = Quaternion.Euler(0, 0, _data.Angle / 2f) * dir;
        GetSpecialistMulti(out var numberProjectile);
        var unit = _data.Angle / (_data.NumberOfProjectilePerHit * numberProjectile);
        for (int i = 0; i < _data.NumberOfProjectilePerHit * numberProjectile; i++)
        {
            var newDir = Quaternion.Euler(0, 0, -unit * i) * dir;
            var projectileData = new ProjectileData
            {
                StartPosition = GameController.Instance.Player.transform.position,
                Range = _data.Range,
                Attacker = this,
                Speed = _data.ProjectileSpeed,
                Direction = newDir.normalized,
                ExtraEffectRate = _data.FireChance
            };
            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.Bullet);
            proj.SetInfo(projectileData);
        }

        _coolDown = 0;
    }
}