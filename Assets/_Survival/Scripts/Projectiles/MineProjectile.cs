using System.Linq;
using DG.Tweening;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class MineProjectile : Projectile
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Transform _explostion;

    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        _renderer.enabled = true;
        _explostion.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (!_isCalculate)
            return;

        _lifeTime += Time.deltaTime;
        if (GameController.Instance.GridManager.IsEnemyCollideWithPoint(transform.position, _data.Size))
        {
            DoExplosion();
            return;
        }

        if (_lifeTime >= GameManager.Instance.GameConfig.MaxLifeTimeProjectile)
        {
            Destroy();
        }
    }

    private void DoExplosion()
    {
        _renderer.enabled = false;
        _explostion.DOScale(new Vector3(_data.Range * 2f, _data.Range * 2f, 1f), 0.1f).OnComplete(() =>
        {
            CauseDamage();
            Destroy();
        });
    }

    private void CauseDamage()
    {
        if (_data.MaxTarget <= 0) return;

        var enemies = GameController.Instance.GridManager.FindTargetsInRange(transform.position, _data.Range);
        if (enemies.IsNullOrEmpty()) return;

        foreach (var e in enemies.Take(_data.MaxTarget))
        {
            e.TakeDamage(_data.Attacker);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _data.Range);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _data.Size);
    }
}