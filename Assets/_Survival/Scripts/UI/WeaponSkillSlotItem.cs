using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSkillSlotItem : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        CleanUp();
    }

    public void SetInfo(Sprite icon, int level)
    {
        _icon.enabled = true;
        _text.enabled = true;
        _icon.sprite = icon;
        _text.SetText($"Level {level}");
    }

    public void CleanUp()
    {
        _icon.enabled = false;
        _text.enabled = false;
    }
}