using UnityEngine;

public class Sword : Weapon
{
    public Sword(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (!(_coolDown >= _data.CoolDownTime)) return;
        GetSpecialistMulti(out var numberProjectile);
        for (var i = 0; i < numberProjectile; i++)
        {
            var randAngle = Random.Range(0, 360f);
            var dir = Quaternion.Euler(0f, 0f, randAngle) * Vector2.right;
            var projectileData = new ProjectileData
            {
                StartPosition = GameController.Instance.Player.ProjectPoint.position + dir * _data.Range,
                Range = _data.ProjectileRange,
                MaxTarget = _data.MaxTarget,
                Attacker = this,
                Speed = _data.ProjectileSpeed,
                ExtraEffectRate = _data.FireChance
            };
            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.SwordSlash);
            proj.SetInfo(projectileData);
        }

        _coolDown = 0;
    }
}