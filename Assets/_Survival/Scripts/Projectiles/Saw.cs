using System;
using System.Linq;
using DG.Tweening;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class Saw : Projectile
{
    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        Init();
    }

    public void Init()
    {
        var scale = Vector2.zero;
        scale.x = _data.Size * 2;
        scale.y = _data.Size * 2;
        transform.localScale = scale;
        transform.DOKill();
        transform.localRotation = Quaternion.identity;
        transform.DOLocalRotate(-Vector3.forward * 180f, 0.6f).SetLoops(-1).SetEase(Ease.Linear);
    }

    public void UpdateData(ProjectileData data)
    {
        _data = data;
        Init();
    }

    private void Update()
    {
        if (_data.MaxTarget <= 0) return;
        var listEnemy = GameController.Instance.GridManager.CircleCast(transform.position, _data.Size);
        if (listEnemy.IsNullOrEmpty()) return;
        foreach (var e in listEnemy.Take(_data.MaxTarget))
        {
            e.TakeDamage(_data.Attacker);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _data.Size);
    }
}