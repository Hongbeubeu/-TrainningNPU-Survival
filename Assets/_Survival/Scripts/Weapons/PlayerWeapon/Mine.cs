using UnityEngine;

public class Mine : Weapon
{
    public Mine(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (!(_coolDown >= _data.CoolDownTime)) return;
        var target = FindNearestTarget(_data.Range);
        if (target == null) return;
        GetSpecialistMulti(out var numberProjectile);
        for (var i = 0; i < numberProjectile; i++)
        {
            var dir = (target.GetTransform().position - GameController.Instance.Player.transform.position).normalized;
            var projectileData = new ProjectileData
            {
                StartPosition = GameController.Instance.Player.transform.position + i * Vector3.up,
                Range = _data.ProjectileRange,
                MaxTarget = _data.MaxTarget,
                Attacker = this,
                Speed = _data.ProjectileSpeed,
                Direction = dir,
                Size = _data.TriggerRadius,
                ExtraEffectRate = _data.FireChance
            };
            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.Mine);
            proj.SetInfo(projectileData);
        }

        _coolDown = 0f;
    }
}