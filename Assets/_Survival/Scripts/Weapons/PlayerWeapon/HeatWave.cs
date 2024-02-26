using UnityEngine;

public class HeatWave : Weapon
{
    private Effect _heatWave;

    public HeatWave(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
    }

    public override void LevelUp()
    {
        base.LevelUp();
        _heatWave.SetSize(_data.ProjectileRange * 2f);
    }

    public override void SpawnWeapon()
    {
        _heatWave = GameManager.Instance.ObjectPooler.InstantiateEffect(EffectType.HeatWave);
        _heatWave.SetInfo();
        _heatWave.SetSize(_data.ProjectileRange * 2f);
        _heatWave.transform.SetParent(Attacker.transform, false);
        _heatWave.transform.localPosition = Vector3.zero;
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        _coolDown += dt;
        if (!(_coolDown >= _data.CoolDownTime)) return;
        var baseRange = _data.ProjectileRange / _data.NumberOfProjectilePerHit;
        for (var i = 0; i < _data.NumberOfProjectilePerHit; i++)
        {
            var projectileData = new ProjectileData
            {
                StartPosition = GameController.Instance.Player.transform.position,
                Range = baseRange + i * baseRange,
                MaxTarget = _data.MaxTarget,
                Attacker = this,
                Speed = _data.ProjectileSpeed,
                ExtraEffectRate = _data.FireChance
            };
            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.HeatWave);
            proj.SetInfo(projectileData);
            proj.transform.SetParent(GameController.Instance.Player.transform);
        }

        _coolDown = 0f;
    }

    public override void Destroy()
    {
        base.Destroy();
        if (!_heatWave)
            _heatWave.Destroy();
    }
}