using System;
using System.Collections.Generic;
using Ultimate.Core.Runtime.EventManager;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Datas/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private PlayerStat _baseData;
    public PlayerStat PlayerStat;
    public int LevelUnlocked;
    [SerializeField] private int _gold;
    public LevelPermanentUpgrade LevelPermanent;
    private readonly GoldChangeEvent OnGoldChange = new();

    public PlayerStat BaseData => _baseData;

    public int Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            OnGoldChange.CurrentGold = _gold;
            EventManager.Instance.Raise(OnGoldChange);
        }
    }

    public void ResetData()
    {
        PlayerStat = new PlayerStat(BaseData);
        UpdatePermanentUpgrade();
    }

    public void ResetTempData()
    {
        BaseData.CopyTo(PlayerStat);
        UpdatePermanentUpgrade();
    }

    private void UpdatePermanentUpgrade()
    {
        for (var i = 0; i < LevelPermanent.LevelPermanentUpgraded.Count; i++)
        {
            var levelUpgraded = LevelPermanent.LevelPermanentUpgraded[i] - 1;
            if (levelUpgraded < 0) continue;
            var value = GameManager.Instance.PermanentUpgradeDatas.UpgradeDatas[i]
                .Datas[levelUpgraded].Value;
            ApplyPermanentData((PermanentUpgradeType)i, value);
        }
    }

    public void UpgradePermanent(PermanentUpgradeType type)
    {
        var currentLevel = LevelPermanent.LevelPermanentUpgraded[(int)type];
        if (currentLevel >= GameManager.Instance.PermanentUpgradeDatas.UpgradeDatas[(int)type].Datas.Length)
            return;
        var value = GameManager.Instance.PermanentUpgradeDatas.UpgradeDatas[(int)type].Datas[currentLevel].Value;
        var cost = GameManager.Instance.PermanentUpgradeDatas.UpgradeDatas[(int)type].Datas[currentLevel].Price;
        if (cost < _gold)
        {
            _gold -= (int)cost;
        }
        else
        {
            return;
        }

        currentLevel++;
        LevelPermanent.LevelPermanentUpgraded[(int)type] = currentLevel;
        ApplyPermanentData(type, value);
    }

    private void ApplyPermanentData(PermanentUpgradeType type, float value)
    {
        switch (type)
        {
            case PermanentUpgradeType.HP:
                PlayerStat.MaxHP += value;
                break;
            case PermanentUpgradeType.DamageMultiplier:
                PlayerStat.DamageMultiplier = value;
                break;
            case PermanentUpgradeType.XPBoost:
                PlayerStat.XPBoost = value;
                break;
            case PermanentUpgradeType.GoldBoost:
                PlayerStat.GoldBoost = value;
                break;
            case PermanentUpgradeType.CriticalChance:
                PlayerStat.CriticalChance = value;
                break;
            case PermanentUpgradeType.CriticalDamageMultiplier:
                PlayerStat.CriticalDamageMultiplier = value;
                break;
        }
    }
}

[Serializable]
public class LevelPermanentUpgrade
{
    public List<int> LevelPermanentUpgraded;
}

[Serializable]
public class PlayerStat
{
    public float Speed;
    public float MaxHP;
    public float AttackSpeed;
    public float MagnetRange;
    public float DamageMultiplier;
    public float XPBoost;
    public float GoldBoost;
    public float CriticalChance;
    public float CriticalDamageMultiplier;
    public float SpeedUp;
    public float SpeedBurst;
    public float SpeedBurstCoolDown;
    public float SpeedBurstDuration;
    public float SpecialistChance;
    public float DodgerChance;
    public float LooterXP;
    public float LooterHP;
    public float LooterGold;

    public PlayerStat()
    {
    }

    public PlayerStat(PlayerStat stat)
    {
        Speed = stat.Speed;
        MaxHP = stat.MaxHP;
        MagnetRange = stat.MagnetRange;
        DamageMultiplier = stat.DamageMultiplier;
        XPBoost = stat.XPBoost;
        GoldBoost = stat.GoldBoost;
        CriticalChance = stat.CriticalChance;
        CriticalDamageMultiplier = stat.CriticalDamageMultiplier;
        SpeedUp = stat.SpeedUp;
        SpeedBurst = stat.SpeedBurst;
        SpeedBurstCoolDown = stat.SpeedBurstCoolDown;
        SpeedBurstDuration = stat.SpeedBurstDuration;
        SpecialistChance = stat.SpecialistChance;
        DodgerChance = stat.DodgerChance;
        LooterXP = stat.LooterXP;
        LooterHP = stat.LooterHP;
        LooterGold = stat.LooterGold;
    }

    public PlayerStat Clone()
    {
        var other = new PlayerStat();
        CopyTo(other);
        return other;
    }

    public void CopyTo(PlayerStat other)
    {
        if (other == null)
        {
            throw new NullReferenceException();
        }

        other.Speed = Speed;
        other.MaxHP = MaxHP;
        other.MagnetRange = MagnetRange;
        other.DamageMultiplier = DamageMultiplier;
        other.XPBoost = XPBoost;
        other.GoldBoost = GoldBoost;
        other.CriticalChance = CriticalChance;
        other.CriticalDamageMultiplier = CriticalDamageMultiplier;
        other.SpeedUp = SpeedUp;
        other.SpeedBurst = SpeedBurst;
        other.SpeedBurstCoolDown = SpeedBurstCoolDown;
        other.SpeedBurstDuration = SpeedBurstDuration;
        other.SpecialistChance = SpecialistChance;
        other.DodgerChance = DodgerChance;
        other.LooterXP = LooterXP;
        other.LooterHP = LooterHP;
        other.LooterGold = LooterGold;
    }
}