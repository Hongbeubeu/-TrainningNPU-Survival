using Ultimate.Core.Runtime.WeightedRandomization;
using UnityEngine;

[CreateAssetMenu(fileName = "BoxItemData", menuName = "Datas/BoxItemData", order = 0)]
public class BoxItemData : ScriptableObject
{
    public int WeaponRate;
    public int SkillRate;
    public int GoldRate;
    public int HPRate;
    public int RollRate;

    private readonly WeightedRandomizer<CollectableItemType> randomizer = new();

    private void Init()
    {
        randomizer.AddOrUpdateValue(CollectableItemType.Weapon, WeaponRate);
        randomizer.AddOrUpdateValue(CollectableItemType.Skill, SkillRate);
        randomizer.AddOrUpdateValue(CollectableItemType.Gold, GoldRate);
        randomizer.AddOrUpdateValue(CollectableItemType.HP, HPRate);
        randomizer.AddOrUpdateValue(CollectableItemType.Roll, RollRate);
    }

    public CollectableItemType GetRandomItem()
    {
        Init();
        return randomizer.GetRandom();
    }
}