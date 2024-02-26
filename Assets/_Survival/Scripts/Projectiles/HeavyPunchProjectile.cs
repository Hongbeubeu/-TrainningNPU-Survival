using DG.Tweening;
using UnityEngine;

public class HeavyPunchProjectile : Projectile
{
    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        DoWave();
    }

    private void DoWave()
    {
        
        transform.localScale = Vector3.one;
        transform.DOScale(new Vector3(_data.Range * 2f, _data.Range * 2f, 1f), 1 / _data.Speed).OnComplete(() =>
        {
            CauseDamage();
            Destroy();
        });
    }

    private void CauseDamage()
    {
        if (_data.MaxTarget <= 0) return;
        var raycastHits = new RaycastHit2D[_data.MaxTarget];
        var hits = Physics2D.CircleCastNonAlloc(transform.position, _data.Range,
            Vector2.up, raycastHits, 0,
            _data.TargetMask);
        if (hits <= 0)
            return;
        for (var i = 0; i < hits; i++)
        {
            var damageable = raycastHits[i].collider.GetComponent<IDamageable>();
            if (damageable == null)
                continue;
            if (damageable.GetTeamType() == _data.Attacker.GetTeamType())
                continue;
            damageable.TakeDamage(_data.Attacker);
        }
    }
}