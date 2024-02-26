using UnityEngine;

public class Laser : Weapon
{
    public Laser(int weaponId, TeamType teamType) : base(weaponId, teamType)
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
        var projectileData = new ProjectileData
        {
            StartPosition = GameController.Instance.Player.transform.position,
            Range = _data.Range,
            MaxTarget = _data.MaxTarget,
            Attacker = this,
            Speed = _data.ProjectileSpeed,
            Direction = dir,
            ExtraEffectRate = _data.FireChance
        };
        var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.LaserBeam);
        proj.SetInfo(projectileData);
        var laserBeam = proj as LaserBeam;
        laserBeam?.SetParent(Attacker.transform);
        laserBeam?.CauseDamage();
        _coolDown = 0f;
    }
}