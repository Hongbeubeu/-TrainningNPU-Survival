using UnityEngine;

public class Lightning : Weapon
{
    public Lightning(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (!(_coolDown >= _data.CoolDownTime)) return;
        GetSpecialistMulti(out var numberProjectile);
        for (var i = 0; i < _data.NumberOfWeapon * numberProjectile; i++)
        {
            var randAngle = Random.Range(0, 360f);
            var dir = Quaternion.Euler(0f, 0f, randAngle) * Vector2.right;
            var randRadius = Random.Range(0f, _data.Range);
            var projectileData = new ProjectileData
            {
                StartPosition = GameController.Instance.Player.transform.position + dir * randRadius,
                Range = _data.ProjectileRange,
                MaxTarget = _data.MaxTarget,
                Attacker = this,
                Speed = _data.ProjectileSpeed,
                NumberProjectilePerHit = _data.NumberOfProjectilePerHit,
                ExtraEffectRate = _data.FireChance
            };
            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.Lightning);
            proj.SetInfo(projectileData);
        }

        _coolDown = 0f;
    }
}