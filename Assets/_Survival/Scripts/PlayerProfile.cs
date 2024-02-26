using System;
using UnityEngine;

public sealed class PlayerProfile : MonoBehaviour
{
    public PlayerData data;

    private void Save()
    {
        var dataJsonPermanentUpgrade = JsonUtility.ToJson(data.LevelPermanent);
        PlayerPrefs.SetString(PlayerPrefParameter.Permanent_Upgrade, dataJsonPermanentUpgrade);
        PlayerPrefs.SetInt(PlayerPrefParameter.Gold, data.Gold);
        PlayerPrefs.SetInt(PlayerPrefParameter.Level_Unlocked, data.LevelUnlocked);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        data.ResetData();

        var jsonUpgradeLevel = PlayerPrefs.GetString(PlayerPrefParameter.Permanent_Upgrade, null);
        if (!string.IsNullOrEmpty(jsonUpgradeLevel))
        {
            data.LevelPermanent = JsonUtility.FromJson<LevelPermanentUpgrade>(jsonUpgradeLevel);
        }
        else
        {
            data.LevelPermanent.LevelPermanentUpgraded.Clear();
            for (var i = 0; i < Enum.GetNames(typeof(PermanentUpgradeType)).Length; i++)
            {
                data.LevelPermanent.LevelPermanentUpgraded.Add(0);
            }
        }

        data.Gold = PlayerPrefs.GetInt(PlayerPrefParameter.Gold, 0);
        data.LevelUnlocked = PlayerPrefs.GetInt(PlayerPrefParameter.Level_Unlocked, 0);
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}