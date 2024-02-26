using Ultimate.Core.Runtime.EventManager;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBarPanel : BaseUI
{
    public Scrollbar Scrollbar;

    public override void Show()
    {
        base.Show();
        Scrollbar.size = 1;
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BossHPChangeEvent>(OnBossChangeHP);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<BossHPChangeEvent>(OnBossChangeHP);
    }

    public void Reset()
    {
        Scrollbar.size = 1f;
    }

    private void OnBossChangeHP(BossHPChangeEvent e)
    {
        var value = e.CurrentHP / e.MaxHP;
        value = Mathf.Clamp(value, 0f, 1f);
        Scrollbar.size = value;
    }
}