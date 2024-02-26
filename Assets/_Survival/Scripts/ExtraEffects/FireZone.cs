public class FireZone : ExtraEffect
{
    private Effect _fireEffect;

    public override void SetInfo(EffectUnitData data, Character attacker)
    {
        base.SetInfo(data, attacker);
        if (!_fireEffect)
            _fireEffect = GameManager.Instance.ObjectPooler.InstantiateEffect(EffectType.FireField);
        _fireEffect.SetInfo(data.Duration, data.Size);
        _fireEffect.transform.position = transform.position;
    }

    public override void Destroy()
    {
        _fireEffect = null;
        base.Destroy();
    }
}