using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageEffect : Effect
{
    [SerializeField] private TextMeshPro _text;

    public override void SetInfo()
    {
        base.SetInfo();
        DoEffect();
    }

    public void SetDamage(float damage)
    {
        _text.SetText($"{damage}");
    }

    private void DoEffect()
    {
        _text.transform.DOKill();
        _text.transform.localPosition = Vector3.zero;
        _text.transform.localScale = new Vector3(1, 1, 1);
        _text.transform.DOLocalMoveY(3f, 0.3f).SetEase(Ease.OutBack, 2f);
        _text.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.3f).SetEase(Ease.OutBack);
    }
}