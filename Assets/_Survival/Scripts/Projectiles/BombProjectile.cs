using System.Linq;
using DG.Tweening;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class BombProjectile : Projectile
{
    private Sequence _sequence;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Transform _explosion;

    private void CauseDamage()
    {
        if (_data.MaxTarget <= 0) return;
        var enemies = GameController.Instance.GridManager.FindTargetsInRange(_data.Target, _data.Range);
        if (enemies.IsNullOrEmpty()) return;

        foreach (var e in enemies.Take(_data.MaxTarget))
        {
            e.TakeDamage(_data.Attacker);
        }
    }

    public void ReleaseBomb()
    {
        var length = Vector2.Distance(transform.position, _data.Target);
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _explosion.localScale = Vector3.zero;
        _renderer.enabled = true;
        _sequence.Append(transform.DOJump(_data.Target, 3f, 1, length / _data.Speed));
        _sequence.AppendCallback(() => _renderer.enabled = false);
        _sequence.Append(_explosion.DOScale(new Vector3(_data.Range * 2f, _data.Range * 2f, 1f), 0.1f));
        _sequence.AppendCallback(() =>
        {
            CauseDamage();
            DoExtraEffect();
            Destroy();
        });
    }
}