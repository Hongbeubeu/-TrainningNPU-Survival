using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectWeaponSkillItem : MonoBehaviour
{
    [SerializeField] private Color[] _color;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _BG;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private SelectWeaponSkillPanel _panel;
    private int _id;
    private CollectableItemType _type;
    private int _value;

    public void SetInfo(string itemName, Sprite icon, string level, int id, CollectableItemType type, int value = 0)
    {
        _type = type;
        _id = id;
        _value = value;
        _BG.color = _type switch
        {
            CollectableItemType.Gold => _color[0],
            CollectableItemType.Weapon => _color[1],
            CollectableItemType.Skill => _color[2],
            _ => _BG.color
        };
        _icon.sprite = icon;
        _nameText.SetText(itemName);
        if (level != "")
        {
            _levelText.SetText($"Level {level}");
        }
        else
        {
            _levelText.SetText($"{_value} $");
        }
    }

    public void OnButtonClick()
    {
        switch (_type)
        {
            case CollectableItemType.Gold:
                GameManager.Instance.PlayerData.Gold += _value;
                break;
            case CollectableItemType.Skill:
                GameController.Instance.Player.SkillController.AddSkill((SkillType)_id);
                break;
            case CollectableItemType.Weapon:
                GameController.Instance.Player.WeaponController.AddWeapon(_id);
                break;
        }

        GameController.Instance.CurrentGameState = GameState.Play;
        _panel.Hide();
    }
}