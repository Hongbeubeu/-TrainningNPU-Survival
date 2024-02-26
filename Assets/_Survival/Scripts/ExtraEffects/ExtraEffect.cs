using UnityEngine;

public class ExtraEffect : MonoBehaviour, IAttackable
{
    public ExtraEffectType Type;
    protected TeamType _teamType;
    protected EffectUnitData _data;
    protected Character _attacker;
    private float _timeAlive;
    private float _coolDown;

    public virtual void SetInfo(EffectUnitData data, Character attacker)
    {
        _data = data;
        _attacker = attacker;
        _teamType = attacker.TeamType;
        _timeAlive = data.Duration;
        _coolDown = data.CoolDown;
        gameObject.tag = _teamType == TeamType.Ally ? UnityConstants.Tags.Ally : UnityConstants.Tags.Enemy;
        gameObject.layer = _teamType == TeamType.Ally ? UnityConstants.Layers.Ally : UnityConstants.Layers.Enemy;
    }

    protected virtual void Update()
    {
        if (_timeAlive <= 0)
        {
            Destroy();
            return;
        }

        _timeAlive -= Time.deltaTime;
        if (_coolDown > 0)
        {
            _coolDown -= Time.deltaTime;
            return;
        }

        CauseDamage();
        _coolDown = _data.CoolDown;
    }

    private void CauseDamage()
    {
        var raycastHits = new RaycastHit2D[_data.MaxTarget];
        var hits = Physics2D.CircleCastNonAlloc(transform.position, _data.Size,
            Vector2.one, raycastHits, 0,
            _teamType == TeamType.Ally ? UnityConstants.Layers.EnemyMask : UnityConstants.Layers.AllyMask);
        if (hits <= 0)
            return;
        for (var i = 0; i < hits; i++)
        {
            var damageable = raycastHits[i].collider.GetComponent<IDamageable>();
            if (damageable == null)
                continue;
            if (damageable.GetTeamType() == GetTeamType())
                continue;
            damageable.TakeDamage(this);
        }
    }

    public virtual void Destroy()
    {
        GameManager.Instance.ObjectPooler.DestroyExtraEffect(gameObject, Type);
    }

    public void CauseDamage(IDamageable target)
    {
        target.TakeDamage(this);
    }

    public float GetDamage()
    {
        return _data.Damage;
    }

    public Transform GetTranform()
    {
        return transform;
    }

    public TeamType GetTeamType()
    {
        return _teamType;
    }

    public Character GetAttacker()
    {
        return _attacker;
    }
}