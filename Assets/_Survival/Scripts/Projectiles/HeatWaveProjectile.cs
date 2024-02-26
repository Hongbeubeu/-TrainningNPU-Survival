using System.Linq;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class HeatWaveProjectile : Projectile
{
    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        CauseDamage();
    }

    private void CauseDamage()
    {
        if (_data.MaxTarget <= 0)
        {
            Destroy();
            return;
        }

        var enemies = GameController.Instance.GridManager.FindTargetsInRange(transform.position, _data.Range);
        if (enemies.IsNullOrEmpty())
        {
            Destroy();
            return;
        }

        foreach (var e in enemies.Take(_data.MaxTarget))
        {
            e.TakeDamage(_data.Attacker);
        }

        Destroy();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _data.Range);
    }
}