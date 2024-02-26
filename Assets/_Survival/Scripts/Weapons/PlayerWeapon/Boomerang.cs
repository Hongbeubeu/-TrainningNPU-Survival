using UnityEngine;

public class Boomerang : Weapon
{
    public Boomerang(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (!(_coolDown >= _data.CoolDownTime)) return;
        var target = GameController.Instance.GridManager.FindNearestTargetInRange(_data.Range);
        if (target == null) return;
        var projectileData = new ProjectileData
        {
            StartPosition = GameController.Instance.Player.ProjectPoint.position,
            Range = _data.Range,
            MaxTarget = _data.MaxTarget,
            Attacker = this,
            Speed = _data.ProjectileSpeed,
            Direction = (target.Position - (Vector2)GameController.Instance.Player.transform.position).normalized,
            ExtraEffectRate = _data.FireChance
        };
        var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.Boomerang);
        proj.SetInfo(projectileData);
        _coolDown = 0;
    }
}