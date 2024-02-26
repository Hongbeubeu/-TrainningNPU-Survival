using UnityEngine;

public class TargetedBullet : Projectile
{
    private Transform _target;
    private FlyweightEnemy _enemyTarget;

    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        _enemyTarget = (FlyweightEnemy)data.Damageable;
    }

    private void Update()
    {
        if (!_isCalculate)
            return;
        var dir = (_enemyTarget.Position - (Vector2)transform.position).normalized;
        var distance = dir * (_data.Speed * Time.deltaTime);
        transform.Translate(distance);

        if (_enemyTarget == null)
        {
            Destroy();
        }

        if (Vector2.SqrMagnitude(_enemyTarget.Position - (Vector2)transform.position) <= 0.1f)
        {
            _data.Damageable.TakeDamage(_data.Attacker);
            Destroy();
        }

        _lifeTime += Time.deltaTime;
        if (_lifeTime >= GameManager.Instance.GameConfig.MaxLifeTimeProjectile)
        {
            Destroy();
        }
    }
}