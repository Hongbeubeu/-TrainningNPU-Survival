using DG.Tweening;

public class HeavyPunch : EnemyWeapon
{
    public HeavyPunch(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (_coolDown < _data.CoolDownTime) return;
        Attacker.SetCanMove(false);
        DOVirtual.DelayedCall(1f / _data.ProjectileSpeed, () =>
        {
            Attacker.SetCanMove(true);
            _controller.RandomWeapon();
        });
        var projectileData = new ProjectileData
        {
            StartPosition = Attacker.transform.position,
            Range = _data.ProjectileRange,
            MaxTarget = _data.MaxTarget,
            Attacker = this,
            Speed = _data.ProjectileSpeed,
            TargetMask = UnityConstants.Layers.AllyMask
        };
        var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.BossPunch);
        proj.SetInfo(projectileData);
        _coolDown = 0;
    }
}