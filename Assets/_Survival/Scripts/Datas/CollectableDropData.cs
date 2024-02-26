using System;
using Ultimate.Core.Runtime.WeightedRandomization;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CollectableDropData", menuName = "Datas/CollectableDropData", order = 0)]
public class CollectableDropData : ScriptableObject
{
    [SerializeField] private CollectableUnitData[] _collectableUnitDatas;

    private readonly WeightedRandomizer<CollectableItemType> _randomizer = new();

    public (CollectableItemType, float) GetCollectableDropItem(int level)
    {
        level = Mathf.Clamp(level, 0, _collectableUnitDatas.Length - 1);
        InitRandomizer(level);
        var itemType = _randomizer.GetRandom();
        float value;
        switch (itemType)
        {
            case CollectableItemType.HP:
                value = _collectableUnitDatas[level].HP;
                break;
            case CollectableItemType.XP:
                value = _collectableUnitDatas[level].XP;
                break;
            case CollectableItemType.Gold:
                value = _collectableUnitDatas[level].Gold;
                break;
            case CollectableItemType.None:
                value = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return (itemType, value);
    }

    private void InitRandomizer(int level)
    {
        _randomizer.AddOrUpdateValue(CollectableItemType.XP, _collectableUnitDatas[level].XPRate);
        _randomizer.AddOrUpdateValue(CollectableItemType.HP, _collectableUnitDatas[level].HPRate);
        _randomizer.AddOrUpdateValue(CollectableItemType.Gold, _collectableUnitDatas[level].GoldRate);
        _randomizer.AddOrUpdateValue(CollectableItemType.None, _collectableUnitDatas[level].NoneRate);
    }
}

[Serializable]
public class CollectableUnitData
{
    public int XPRate;
    public int HPRate;
    public int GoldRate;
    public int NoneRate;
    public float XP;
    public float HP;
    public float Gold;
}