using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _lockedMask;
    private int _mapId;
    private MapTabPanel _mapTabPanel;
    private bool _isUnlocked;

    public bool IsUnlocked
    {
        get => _isUnlocked;
        set
        {
            _button.enabled = value;
            _isUnlocked = value;
            _lockedMask?.SetActive(!_isUnlocked);
        }
    }


    private void OnValidate()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    public void SetInfo(MapTabPanel mapTabPanel, int index, bool isUnlocked)
    {
        _mapTabPanel = mapTabPanel;
        _mapId = index;
        IsUnlocked = isUnlocked;
    }

    private void OnButtonClick()
    {
        if (_mapTabPanel == null) return;
        _mapTabPanel.OnClickButton(_mapId);
    }
}