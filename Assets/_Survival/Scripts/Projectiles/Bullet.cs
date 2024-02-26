using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] private Transform _bulletSprite;

    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        _bulletSprite.right = _data.Direction;
    }

    private void Update()
    {
        if (!_isCalculate)
            return;
        var distance = _data.Direction * (_data.Speed * Time.deltaTime);
        transform.Translate(distance);
        if (IsDestroyByRange)
        {
            _distance += distance.magnitude;
            if (_distance >= _data.Range)
            {
                Destroy();
            }
        }

        _lifeTime += Time.deltaTime;

        if (_lifeTime >= GameManager.Instance.GameConfig.MaxLifeTimeProjectile)
        {
            Destroy();
        }

        var (isCollide, damageable) = GameController.Instance.GridManager.CheckCollideBullet(this);
        if (!isCollide) return;
        damageable.TakeDamage(_data.Attacker);
        Destroy();
    }
}