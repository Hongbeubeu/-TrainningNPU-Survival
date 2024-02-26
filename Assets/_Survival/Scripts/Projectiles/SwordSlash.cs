using System.Linq;
using DG.Tweening;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class SwordSlash : Projectile
{
    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        DoSlash();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _data.Range);
    }

    private void DoSlash()
    {
        transform.DOKill();
        transform.localScale = new Vector3(_data.Range * 2f, _data.Range * 2f, 1f);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            CauseDamage();
            Destroy();
        });
        // transform.DOScale(new Vector3(_data.Range * 2f, _data.Range * 2f, 1), 0.2f).OnComplete(() => { });
    }

    private void CauseDamage()
    {
        if (_data.MaxTarget <= 0) return;

        var listEnemy = GameController.Instance.GridManager.CircleCast(transform.position, _data.Range);
        if (listEnemy.IsNullOrEmpty()) return;
        foreach (var e in listEnemy.Take(_data.MaxTarget))
        {
            e.TakeDamage(_data.Attacker);
        }

        // var raycastHits = new RaycastHit2D[_data.MaxTarget];
        // var hits = Physics2D.CircleCastNonAlloc(transform.position, _data.Range,
        //     Vector2.zero, raycastHits,
        //     UnityConstants.Layers.EnemyMask);
        // if (hits <= 0)
        //     return;
        // for (var i = 0; i < hits; i++)
        // {
        //     var damageable = raycastHits[i].collider.GetComponent<IDamageable>();
        //     if (damageable == null)
        //         continue;
        //     if (damageable.GetTeamType() == _data.Attacker.GetTeamType())
        //         continue;
        //     damageable.TakeDamage(_data.Attacker);
        // }
    }
}