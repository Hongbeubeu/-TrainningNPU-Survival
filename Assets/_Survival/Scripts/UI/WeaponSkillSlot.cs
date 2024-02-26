using UnityEngine;

public class WeaponSkillSlot : MonoBehaviour
{
    [SerializeField] private WeaponSkillSlotItem[] _weaponItems;
    [SerializeField] private WeaponSkillSlotItem[] _skillItems;

    public void SetInfo()
    {
        for (var i = 0; i < _weaponItems.Length; i++)
        {
            _weaponItems[i].CleanUp();
            if (i >= GameController.Instance.Player.WeaponController.CountOwnedWeapon()) continue;
            _weaponItems[i].SetInfo(
                GameManager.Instance.GetWeaponSpriteIcon((int)GameController.Instance.Player.WeaponController
                    .GetWeaponAt(i).Type),
                GameController.Instance.Player.WeaponController.GetWeaponAt(i).CurrentLevel + 1);
        }

        for (var i = 0; i < _skillItems.Length; i++)
        {
            _skillItems[i].CleanUp();
            if (i >= GameController.Instance.Player.SkillController.CountOwnedSkill()) continue;
            _skillItems[i].SetInfo(
                GameManager.Instance.GetSkillSpriteIcon((int)GameController.Instance.Player.SkillController
                    .GetSkillAtIndex(i).Type),
                GameController.Instance.Player.SkillController.GetSkillAtIndex(i).CurrentLevel + 1);
        }
    }
}