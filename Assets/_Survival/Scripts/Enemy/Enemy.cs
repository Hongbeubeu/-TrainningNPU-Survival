using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    [SerializeField] private EnemyTier _tier;
    public AttackableComponent Attacker;
    public DamageableComponent Damageable;
    public EnemyWeaponController WeaponController;

    private float _coolDown;
    private float _maxCoolDown;
    private float _attackSpeed;
    private int _collectableItemDropLevel;
    private bool _canMove;
    [SerializeField] protected EnemyAnimatorController _animatorController;

    public virtual void SetInfo(EnemyUnitData data)
    {
        SetData(data);
        if (Attacker)
        {
            Attacker.SetInfo(this);
            Attacker.IsRanger = data.IsRanger;
        }

        if (Damageable)
        {
            Damageable.SetInfo(this);
            GameController.Instance.AddDamageable(Damageable);
        }

        Target = GameController.Instance.Player.Damageable;
        RandomPosition();
        _canMove = true;
    }

    private void SetData(EnemyUnitData data)
    {
        MaxHP = data.HP;
        CurrentHP = data.HP;
        Speed = data.Speed;
        Range = data.Range;
        Damage = data.Damage;
        _attackSpeed = data.AttackSpeed;
        _maxCoolDown = 1f / _attackSpeed;
        _collectableItemDropLevel = data.CollectableDropLevel;
        CurrentData.AttackSpeed = _attackSpeed;
        _coolDown = _maxCoolDown;
    }

    private void RandomPosition()
    {
        var randomPos = Vector3.zero;
        randomPos.x = Random.Range(-20, 20);
        randomPos.y = Random.Range(-20, 20);
        transform.position = randomPos;
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }

    protected virtual void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Pause) return;
        if (_canMove)
        {
            var dir = (Target.GetTransform().position - transform.position).normalized;
            _animatorController.SetDirection(dir);
            transform.Translate(dir * (Speed * Time.deltaTime));
        }

        _coolDown -= Time.deltaTime;
        if (!(_coolDown <= 0)) return;
        if (Vector2.Distance(transform.position, Target.GetTransform().position) > Range) return;
        Attacker?.CauseDamage(Target);
        _coolDown = _maxCoolDown;
    }

    private void OnSpawnCollectableItem()
    {
        var (type, value) =
            GameManager.Instance.CollectableDropData.GetCollectableDropItem(_collectableItemDropLevel);
        if (type == CollectableItemType.None)
            return;

        var item = GameManager.Instance.ObjectPooler.InstantiateCollectableItem();
        item.transform.position = transform.position;
        item.SetInfo(type, value);
    }

    public override void Die()
    {
        OnSpawnCollectableItem();
        SelfDestroy();
    }

    public virtual void SelfDestroy()
    {
        GameController.Instance.RemoveDamageable(Damageable);
        DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        WeaponController?.ResetData();
        GameManager.Instance.ObjectPooler.DestroyEnemy(gameObject, _tier);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}