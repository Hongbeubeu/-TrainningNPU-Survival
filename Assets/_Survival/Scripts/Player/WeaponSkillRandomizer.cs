using System.Collections.Generic;
using Ultimate.Core.Runtime.WeightedRandomization;
using UnityEngine;

public class WeaponSkillRandomizer
{
    private List<(int, int, bool)> _itemList;
    private WeightedRandomizer<int> _randomizer;
    private int _ownedWeaponCount;
    private int _ownedSkillCount;

    public WeaponSkillRandomizer()
    {
        Init();
    }

    public void Init()
    {
        _itemList ??= new List<(int, int, bool)>();
        _randomizer ??= new WeightedRandomizer<int>();
        _ownedSkillCount = 0;
        _ownedWeaponCount = 0;
        InnitItemList();
        InitRandomize();
    }

    private void InnitItemList()
    {
        var count = GameManager.Instance.WeaponData.WeaponDatas.Length;
        for (var i = 0; i < count; i++)
        {
            var maxLevel = GameManager.Instance.WeaponData.WeaponDatas[i].BaseWeaponDatas.Length;
            _itemList.Add((i, maxLevel, false));
        }

        var weaponLength = count;
        count = GameManager.Instance.SkillData.SkillDatas.Length;
        for (var i = 0; i < count; i++)
        {
            var maxLevel = GameManager.Instance.SkillData.SkillDatas[i].SkillLevelDatas.Length;
            _itemList.Add((i + weaponLength, maxLevel, false));
        }
    }

    private void InitRandomize()
    {
        for (var i = 0; i < _itemList.Count; i++)
        {
            _randomizer.AddOrUpdateValue(i, 1);
        }
    }

    public int GetRandom(out List<WeaponType> weaponTypes, out List<SkillType> skillTypes)
    {
        weaponTypes = null;
        skillTypes = null;

        var maxItem = CountAvailableItem();
        if (maxItem == 0)
            return maxItem;
        maxItem = Mathf.Clamp(maxItem, 0, 3);
        var tempItemIds = new int[maxItem];
        for (var i = 0; i < maxItem; i++)
        {
            var value = _randomizer.GetRandom();
            tempItemIds[i] = value;
            _randomizer.AddOrUpdateValue(value, 0);
            if (value < GameManager.Instance.WeaponData.WeaponDatas.Length)
            {
                weaponTypes ??= new List<WeaponType>();
                weaponTypes.Add((WeaponType)value);
            }
            else
            {
                skillTypes ??= new List<SkillType>();
                skillTypes.Add((SkillType)(value - GameManager.Instance.WeaponData.WeaponDatas.Length));
            }
        }

        for (var i = 0; i < tempItemIds.Length; i++)
        {
            _randomizer.AddOrUpdateValue(tempItemIds[i], 1);
        }

        return maxItem;
    }

    private int CountAvailableItem()
    {
        var count = 0;
        count = CountAvailableWeapon(count);
        count = CountAvailableSkill(count);
        return count;
    }

    private int CountAvailableWeapon(int count)
    {
        if (_ownedWeaponCount < GameManager.Instance.GameConfig.MaxWeaponSlot)
        {
            for (var i = 0; i < GameManager.Instance.WeaponData.WeaponDatas.Length; i++)
            {
                if (_itemList[i].Item2 > 0)
                    count++;
            }
        }
        else
        {
            for (var i = 0; i < GameManager.Instance.WeaponData.WeaponDatas.Length; i++)
            {
                if (_itemList[i].Item3 && _itemList[i].Item2 > 0)
                    count++;
            }
        }

        return count;
    }

    private int CountAvailableSkill(int count)
    {
        if (_ownedSkillCount < GameManager.Instance.GameConfig.MaxSkillSlot)
        {
            for (var i = GameManager.Instance.WeaponData.WeaponDatas.Length; i < _itemList.Count; i++)
            {
                if (_itemList[i].Item2 > 0)
                    count++;
            }
        }
        else
        {
            for (var i = GameManager.Instance.WeaponData.WeaponDatas.Length; i < _itemList.Count; i++)
            {
                if (_itemList[i].Item3 && _itemList[i].Item2 > 0)
                    count++;
            }
        }

        return count;
    }

    public void UpdateWeapon(WeaponType weaponType)
    {
        var valueTuple = _itemList[(int)weaponType];
        if (!valueTuple.Item3)
        {
            _ownedWeaponCount++;
            valueTuple.Item3 = true;
            _itemList[(int)weaponType] = valueTuple;
            if (_ownedWeaponCount == GameManager.Instance.GameConfig.MaxWeaponSlot)
            {
                UpdateWeaponRandomizer();
            }
        }

        valueTuple.Item2--;
        _itemList[(int)weaponType] = valueTuple;
        if (valueTuple.Item2 > 0) return;
        valueTuple.Item3 = false;
        _itemList[(int)weaponType] = valueTuple;
        UpdateWeaponRandomizer();
    }

    private void UpdateWeaponRandomizer()
    {
        for (var i = 0; i < GameManager.Instance.WeaponData.WeaponDatas.Length; i++)
        {
            if (_itemList[i].Item2 == 0)
            {
                _randomizer.AddOrUpdateValue(i, 0);
            }
            else if (_ownedWeaponCount == GameManager.Instance.GameConfig.MaxWeaponSlot && !_itemList[i].Item3)
            {
                _randomizer.AddOrUpdateValue(i, 0);
            }
        }
    }

    public void UpdateSkill(SkillType skillType)
    {
        var valueTuple = _itemList[(int)skillType + GameManager.Instance.WeaponData.WeaponDatas.Length];
        if (!valueTuple.Item3)
        {
            _ownedSkillCount++;
            valueTuple.Item3 = true;
            _itemList[(int)skillType + GameManager.Instance.WeaponData.WeaponDatas.Length] = valueTuple;
            if (_ownedSkillCount == GameManager.Instance.GameConfig.MaxSkillSlot)
            {
                UpdateSkillRandomizer();
            }
        }

        valueTuple.Item2--;
        _itemList[(int)skillType + GameManager.Instance.WeaponData.WeaponDatas.Length] = valueTuple;
        if (valueTuple.Item2 > 0) return;
        valueTuple.Item3 = false;
        _itemList[(int)skillType + GameManager.Instance.WeaponData.WeaponDatas.Length] = valueTuple;
        UpdateSkillRandomizer();
    }

    private void UpdateSkillRandomizer()
    {
        for (var i = GameManager.Instance.WeaponData.WeaponDatas.Length; i < _itemList.Count; i++)
        {
            if (_itemList[i].Item2 == 0)
            {
                _randomizer.AddOrUpdateValue(i, 0);
            }
            else if (_ownedSkillCount == GameManager.Instance.GameConfig.MaxSkillSlot && !_itemList[i].Item3)
            {
                _randomizer.AddOrUpdateValue(i, 0);
            }
        }
    }

    public void CleanUp()
    {
        _itemList.Clear();
        _randomizer.ClearElementList();
    }
}