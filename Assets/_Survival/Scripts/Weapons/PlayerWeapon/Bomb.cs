using UnityEngine;

public class Bomb : Weapon
{
    public Bomb(int weaponId, TeamType teamType) : base(weaponId, teamType)
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
            var target = GameController.Instance.GridManager.FindNearestTargetInRange(_data.Range);
            if (target == null) return;
            var dir = (target.Position - (Vector2)GameController.Instance.Player.transform.position).normalized;
            var projectileData = new ProjectileData
            {
                StartPosition = GameController.Instance.Player.transform.position,
                Range = _data.ProjectileRange,
                MaxTarget = _data.MaxTarget,
                Attacker = this,
                Speed = _data.ProjectileSpeed,
                Direction = dir,
                Target = target.Position,
                ExtraEffectRate = _data.FireChance
            };
            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.Bomb);
            proj.SetInfo(projectileData);
            var bomb = proj as BombProjectile;
            bomb?.ReleaseBomb();
        }

        _coolDown = 0f;
    }
}