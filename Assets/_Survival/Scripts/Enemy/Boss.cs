using Ultimate.Core.Runtime.EventManager;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] private EnemyWeaponType[] _weaponTypes;
    private readonly BossHPChangeEvent OnBossHPChangeEvent = new();

    public override void SetInfo(EnemyUnitData data)
    {
        base.SetInfo(data);
        UIController.Instance.InGamePanel.BossHpBarPanel.Show();
        UIController.Instance.InGamePanel.BossHpBarPanel.Reset();
    }

    private void Start()
    {
        WeaponController?.SetInfo(this);
        for (var i = 0; i < _weaponTypes.Length; i++)
        {
            WeaponController?.AddWeapon((int)_weaponTypes[i]);
        }

        WeaponController?.RandomWeapon();
    }

    public override void SetHP(float value)
    {
        base.SetHP(value);
        OnBossHPChangeEvent.CurrentHP = CurrentHP;
        OnBossHPChangeEvent.MaxHP = MaxHP;
        EventManager.Instance.Raise(OnBossHPChangeEvent);
    }

    public override void SelfDestroy()
    {
        base.SelfDestroy();
        UIController.Instance.InGamePanel.BossHpBarPanel.Hide();
    }
}