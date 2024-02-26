using System.Linq;
using DG.Tweening;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class LaserBeam : Projectile
{
    [SerializeField] private Transform _beam;

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
    }

    public void CauseDamage()
    {
        DoLaser();
        if (_data.MaxTarget <= 0) return;

        var enemies =
            GameController.Instance.GridManager.BoxCast(_data.StartPosition, _data.Direction, 1f, _data.Range);
        if (enemies.IsNullOrEmpty()) return;
        var length = Mathf.Min(_data.MaxTarget, enemies.Count);
        for (var i = 0; i < length; i++)
        {
            enemies[i].TakeDamage(_data.Attacker);
        }
    }

    private void DoLaser()
    {
        var scale = _beam.localScale;
        scale.y = 0f;
        scale.x = _data.Range;
        _beam.right = _data.Direction;
        DOVirtual.Float(0f, 1f, 0.2f, value =>
        {
            scale.y = value;
            _beam.localScale = scale;
        }).SetEase(Ease.OutBack).OnComplete(Destroy);
    }
}