using DG.Tweening;
using Ultimate.Core.Runtime.EventManager;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public struct ProjectileData
{
    public Vector2 StartPosition;
    public TeamType TeamType;
    public float Speed;
    public float Range;
    public float Size;
    public Vector2 Direction;
    public Vector2 Target;
    public int MaxTarget;
    public int NumberProjectilePerHit;
    public float ExtraEffectRate;
    public IAttackable Attacker;
    public IDamageable Damageable;
    public int TargetMask;
}

public class Projectile : MonoBehaviour
{
    public ProjectileType Type;

    [FormerlySerializedAs("IsAutoDestroy")]
    public bool IsDestroyByRange;

    public ProjectileData _data;
    protected float _lifeTime;
    protected bool _isCalculate;
    protected float _distance;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<GameResetEvent>(OnGameReset);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<GameResetEvent>(OnGameReset);
    }

    public virtual void SetInfo(ProjectileData data)
    {
        _data = data;
        transform.position = data.StartPosition;
        _isCalculate = true;
        _distance = 0f;
        _lifeTime = 0f;
        gameObject.tag = data.TeamType == TeamType.Ally ? UnityConstants.Tags.Ally : UnityConstants.Tags.Enemy;
    }

    // protected virtual void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag(gameObject.tag))
    //         return;
    //     var damageable = other.GetComponent<IDamageable>();
    //     if (damageable == null)
    //         return;
    //     if (damageable.GetTeamType() == _data.Attacker.GetTeamType())
    //         return;
    //     damageable.TakeDamage(_data.Attacker);
    //     DoExtraEffect();
    //     Destroy();
    // }
    //
    // protected virtual void OnTriggerStay2D(Collider2D other)
    // {
    // }

    protected void DoExtraEffect()
    {
        if (_data.ExtraEffectRate < 0f) return;
        var rand = Random.Range(0f, 1f);
        if (!(rand <= _data.ExtraEffectRate)) return;
        var data = GameManager.Instance.ExtraEffectData.ExtraEffects[0];
        var numEffect = Random.Range(data.MinUnit, data.MaxUnit);
        for (var i = 0; i < numEffect; i++)
        {
            var extraEffect = GameManager.Instance.ObjectPooler.InstantiateExtraEffect(ExtraEffectType.FireEffect);
            var randRadius = Random.Range(0, data.Range);
            var randAngle = Random.Range(0f, 360f);
            var pos = transform.position + Quaternion.Euler(0f, 0f, randAngle) * Vector2.right * randRadius;
            extraEffect.transform.position = pos;
            extraEffect.SetInfo(data, _data.Attacker.GetAttacker());
        }
    }

    private void OnGameReset(GameResetEvent e)
    {
        Destroy();
    }

    public void Destroy()
    {
        transform.SetParent(null);
        transform.DOKill();
        GameManager.Instance.ObjectPooler.DestroyProjectile(gameObject, Type);
    }
}