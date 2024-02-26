using DG.Tweening;

public class CrazyChase : EnemyWeapon
{
    public CrazyChase(int weaponId, TeamType teamType) : base(weaponId, teamType)
    {
        _coolDown = _data.CoolDownTime;
    }

    public override void OnUpdate(float dt)
    {
        if (!_isCaculate) return;
        if (_coolDown > 0)
        {
            _coolDown -= dt;
            return;
        }

        DoChase();
        _coolDown = _data.CoolDownTime;
    }

    private void DoChase()
    {
        Attacker.SetCanMove(false);
        DOVirtual.DelayedCall(_data.Range / _data.ProjectileSpeed, () =>
        {
            Attacker.SetCanMove(true);
            _controller.RandomWeapon();
        });
        var dir = (Attacker.Target.GetTransform().position - Attacker.transform.position).normalized;
        Attacker.transform.DOMove(Attacker.transform.position + dir * _data.Range, _data.Range / _data.ProjectileSpeed);
    }
}