using UnityEngine;

public class FlyweightEnemy : IAttackable, IDamageable
{
    public int ID;
    public int CurrentIndex;
    public EnemySharedData Data;
    public Vector2 Position;
    public float Scale;
    public float Rotation;
    public float Speed;
    public float CurrentHP;
    private float attackCooldown;
    private EnemyRenderController _controller;
    private bool _isDead;

    public void SetInfo(EnemySharedData data, EnemyRenderController controller)
    {
        _controller = controller;
        Data = data;
        CurrentHP = Data.MaxHP;
        Rotation = 0f;
        Scale = 1f;
        Speed = Random.Range(2f, 2.5f);
        CurrentIndex = GameController.Instance.GridManager.GetGridIndex(Position);
        attackCooldown = 1f / Data.AttackSpeed;
        _isDead = false;
    }

    public void DoUpdate(float dt, Vector2 dir)
    {
        // if (GameController.Instance.GridManager.CheckCollide(this, dir))
        // {
        //     dir = Vector2.zero;
        // }

        Position += dir * (Speed * dt);
        GameController.Instance.GridManager.UpdateEnemyInGrid(CurrentIndex, this);
        DoDamage(dt);
    }

    private void DoDamage(float dt)
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= dt;
            return;
        }

        if (Vector2.SqrMagnitude(Position - (Vector2)GameController.Instance.Player.transform.position) >
            Data.Range * Data.Range) return;
        if (!Data.IsRanger)
        {
            GameController.Instance.Player.Damageable.TakeDamage(this);
        }
        else
        {
            var projectileData = new ProjectileData
            {
                TeamType = TeamType.Enemy,
                StartPosition = Position,
                Size = 0.2f,
                Attacker = this,
                Speed = Data.AttackSpeed * 5,
                Direction = ((Vector2)_controller.Target.position - Position).normalized
            };
            var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(ProjectileType.EnemyBullet1);
            proj.SetInfo(projectileData);
        }

        attackCooldown = 1f / Data.AttackSpeed;
    }

    private void OnSpawnCollectableItem()
    {
        var (type, value) =
            GameManager.Instance.CollectableDropData.GetCollectableDropItem(Data.CollectableDropLevel);
        if (type == CollectableItemType.None)
            return;

        var item = GameManager.Instance.ObjectPooler.InstantiateCollectableItem();
        item.transform.position = Position;
        item.SetInfo(type, value);
    }

    public void Die()
    {
        if (_isDead)
            return;
        _isDead = true;
        OnSpawnCollectableItem();
        GameController.Instance.GridManager.Remove(this);
        GameController.Instance.EnemyRenderController.RemoveEnemy(this);
    }

    #region IAttackable Implement

    public void CauseDamage(IDamageable target)
    {
        target.TakeDamage(this);
    }

    public float GetDamage()
    {
        return Data.Damage;
    }

    public Transform GetTranform()
    {
        return null;
    }


    public TeamType GetTeamType()
    {
        return TeamType.Enemy;
    }


    public Character GetAttacker()
    {
        return null;
    }

    #endregion

    #region IDamageable Implement

    public void TakeDamage(IAttackable attacker)
    {
        if (_isDead)
            return;
        CurrentHP -= attacker.GetDamage();
        CurrentHP = CurrentHP < 0 ? 0 : CurrentHP;
        var hitEffect = GameManager.Instance.ObjectPooler.InstantiateEffect(EffectType.Hit);
        hitEffect.SetInfo();
        hitEffect.transform.position = Position;
        var effectText =
            GameManager.Instance.ObjectPooler.InstantiateEffect(EffectType.DamgeEffect);
        effectText.transform.position = Position;
        var textDamageEffect = effectText as DamageEffect;
        textDamageEffect?.SetInfo();
        textDamageEffect?.SetDamage(attacker.GetDamage());
        if (CurrentHP == 0)
            Die();
    }

    public Transform GetTransform()
    {
        return null;
    }

    public void Destroy()
    {
        GameController.Instance.GridManager.Remove(this);
    }

    #endregion
}