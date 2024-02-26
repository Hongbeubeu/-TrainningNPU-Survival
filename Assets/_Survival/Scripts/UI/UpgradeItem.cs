using TMPro;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    [SerializeField] private PermanentUpgradeType Type;
    [SerializeField] private TextMeshProUGUI _value;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _price;

    public void Upgrade()
    {
        GameManager.Instance.PlayerData.UpgradePermanent(Type);
        UIController.Instance.MenuGamePanel.PlayerUpgradePanel.SetInfo();
    }

    public void Init()
    {
        switch (Type)
        {
            case PermanentUpgradeType.HP:
                _value.SetText($"{GameManager.Instance.PlayerData.PlayerStat.MaxHP}");
                break;
            case PermanentUpgradeType.DamageMultiplier:
                _value.SetText($"x{GameManager.Instance.PlayerData.PlayerStat.DamageMultiplier}");
                break;
            case PermanentUpgradeType.XPBoost:
                _value.SetText($"{GameManager.Instance.PlayerData.PlayerStat.XPBoost}%");
                break;
            case PermanentUpgradeType.GoldBoost:
                _value.SetText($"{GameManager.Instance.PlayerData.PlayerStat.GoldBoost}%");
                break;
            case PermanentUpgradeType.CriticalChance:
                _value.SetText($"{GameManager.Instance.PlayerData.PlayerStat.CriticalChance}%");
                break;
            case PermanentUpgradeType.CriticalDamageMultiplier:
                _value.SetText($"x{GameManager.Instance.PlayerData.PlayerStat.CriticalDamageMultiplier}");
                break;
        }

        var level = GameManager.Instance.PlayerData.LevelPermanent.LevelPermanentUpgraded[(int)Type];
        _level.SetText($"Lv{level}");
        _price.SetText(level < GameManager.Instance.PermanentUpgradeDatas.UpgradeDatas[(int)Type].Datas.Length
            ? $"{GameManager.Instance.PermanentUpgradeDatas.UpgradeDatas[(int)Type].Datas[level].Price}$"
            : "");
    }
}