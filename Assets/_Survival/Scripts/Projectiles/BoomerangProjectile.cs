using DG.Tweening;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class BoomerangProjectile : Projectile
{
    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        ThrowBoomerang();
    }

    private void ThrowBoomerang()
    {
        transform.DOKill();
        var sequence = DOTween.Sequence();
        var dir = _data.Direction;
        transform.position = _data.Attacker.GetTranform().position;
        sequence.Append(transform.DOMove((Vector2)transform.position + dir * _data.Range, 0.5f));
        sequence.AppendCallback(() =>
        {
            dir = (_data.Attacker.GetTranform().position - transform.position).normalized;
            transform.DOMove((Vector2)transform.position + dir * (_data.Range * 2f), 1f);
        });
        sequence.AppendInterval(1f);
        sequence.AppendCallback(Destroy);
        transform.DORotate(new Vector3(0f, 0f, 180f), 0.1f).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void Update()
    {
        var enemies = GameController.Instance.GridManager.FindTargetsInRange(transform.position, _data.Size);
        if (enemies.IsNullOrEmpty()) return;
        for (var i = 0; i < enemies.Count; i++)
        {
            enemies[i].TakeDamage(_data.Attacker);
        }
    }
}