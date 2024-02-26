using System.Collections;
using System.Linq;
using DG.Tweening;
using Ultimate.Core.Runtime.Extensions;
using UnityEngine;

public class Thunder : Projectile
{
    private int _numberLightning;
    [SerializeField] private Animator _animator;
    private static readonly int _thunder = Animator.StringToHash("Thunder");

    public override void SetInfo(ProjectileData data)
    {
        base.SetInfo(data);
        StartCoroutine(DoLightning());
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

    private IEnumerator DoLightning()
    {
        transform.localScale = new Vector3(_data.Range * 2f, _data.Range * 2f, 1f);
        for (var i = 0; i < _data.NumberProjectilePerHit; i++)
        {
            _animator.SetTrigger(_thunder);
            DOVirtual.Float(0, 1, 0.5f, _ => { })
                .OnComplete(
                    () =>
                    {
                        var effect =
                            GameManager.Instance.ObjectPooler.InstantiateEffect(EffectType.Lightning_Explosion);
                        effect.transform.localScale = transform.localScale;
                        effect.transform.position = transform.position;
                        effect.SetInfo();
                        CauseDamage();
                        Destroy();
                    });
            yield return new WaitForSeconds(0.5f);
        }
    }
}