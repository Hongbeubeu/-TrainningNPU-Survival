using UnityEngine;

public class SelectWeaponSkillPanel : BaseUI
{
    [SerializeField] private SelectWeaponSkillItem[] _selectWeaponSkillItems;
    [SerializeField] private WeaponSkillSlot _weaponSkillSlot;

    public void RandomWeaponSkill()
    {
        _weaponSkillSlot.SetInfo();
        for (var i = 0; i < _selectWeaponSkillItems.Length; i++)
        {
            _selectWeaponSkillItems[i].gameObject.SetActive(false);
        }

        var num = GameController.Instance.WeaponSkillRandomizer.GetRandom(out var weaponTypes, out var skillTypes);
        if (num == 0)
            for (var i = 0; i < _selectWeaponSkillItems.Length; i++)
            {
                _selectWeaponSkillItems[i].gameObject.SetActive(true);
                SetSelectGold(_selectWeaponSkillItems[i]);
            }

        var count = 0;
        if (weaponTypes != null)
        {
            for (var i = 0; i < weaponTypes.Count; i++)
            {
                _selectWeaponSkillItems[i].gameObject.SetActive(true);
                SetWeapon(_selectWeaponSkillItems[i], weaponTypes[i]);
                count++;
            }
        }

        if (skillTypes != null)
        {
            for (var i = 0; i < skillTypes.Count; i++)
            {
                _selectWeaponSkillItems[i + count].gameObject.SetActive(true);
                SetSkill(_selectWeaponSkillItems[i + count], skillTypes[i]);
            }
        }

        Show();
        GameController.Instance.CurrentGameState = GameState.Pause;
    }

    private void SetWeapon(SelectWeaponSkillItem item, WeaponType type)
    {
        var id = (int)type;
        var level = GameController.Instance.Player.WeaponController.GetCurrentLevelWeapon((WeaponType)id);
        item.SetInfo(((WeaponType)id).ToString(), GameManager.Instance.GetWeaponSpriteIcon(id), $"{level + 1}",
            id,
            CollectableItemType.Weapon);
    }

    private void SetSkill(SelectWeaponSkillItem item, SkillType type)
    {
        var id = (int)type;
        var level = GameController.Instance.Player.SkillController.GetCurrentLevelSkill((SkillType)id);
        item.SetInfo(((SkillType)id).ToString(), GameManager.Instance.GetSkillSpriteIcon(id), $"{level + 1}",
            id,
            CollectableItemType.Skill);
    }

    private void SetSelectGold(SelectWeaponSkillItem item)
    {
        item.SetInfo("Gold", GameManager.Instance.GoldSprite, "", 0, CollectableItemType.Gold,
            GameManager.Instance.GameConfig.GoldBonusLevelUp);
    }
}