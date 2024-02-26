using Ultimate.Core.Runtime.EventManager;
using UnityEngine;

public class PlayerXPController : MonoBehaviour
{
    private Player _player;
    private float _currentXP;
    private float _maxXP;
    private int _currentLevel;
    [SerializeField] private XPByLevelData _data;

    private readonly PlayerChangeXPEvent OnPlayerChangeXP = new();

    public float CurrentXP
    {
        get => _currentXP;
        set
        {
            _currentXP = value;
            if (_currentXP >= _maxXP)
            {
                _currentXP -= _maxXP;
                CurrentLevel++;
                _maxXP = _data.GetMaxXPByLevel(CurrentLevel);
            }

            OnPlayerChangeXP.CurrentXP = _currentXP;
            OnPlayerChangeXP.MaxXP = _maxXP;
            EventManager.Instance.Raise(OnPlayerChangeXP);
        }
    }

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            if (value > _currentLevel && value > 1)
            {
                _player.LevelUp();
            }

            _currentLevel = value;
        }
    }

    public void SetInfo(Player player)
    {
        _player = player;
        ResetData();
    }

    public void ResetData()
    {
        CurrentLevel = 1;
        _maxXP = _data.GetMaxXPByLevel(_currentLevel);
        CurrentXP = 0f;
    }
}