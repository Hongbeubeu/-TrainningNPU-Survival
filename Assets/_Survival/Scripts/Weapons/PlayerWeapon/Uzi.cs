using UnityEngine;

public class Uzi : Weapon
{
    public Uzi(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (!(_coolDown >= _data.CoolDownTime)) return;
        var target = GameController.Instance.FindNearestEnemy(_data.Range);
        if (target == null)
            return;
        var projectileData = new ProjectileData
        {
            StartPosition = Attacker.transform.position,
            Range = _data.Range,
            MaxTarget = _data.MaxTarget,
            Attacker = this,
            Speed = _data.ProjectileSpeed,
            ExtraEffectRate = _data.FireChance,
            Direction = (target.Position - (Vector2)Attacker.transform.position).normalized
        };
        var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.Bullet);
        proj.SetInfo(projectileData);
        _coolDown = 0;
    }
}