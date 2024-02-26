using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private ProgressBarComponent _hpBar;

    public Character Character { get; private set; }

    public void Setup(Character character)
    {
        Character = character;
        Character.OnHPChanged -= OnHPChanged;
        Character.OnHPChanged += OnHPChanged;
    }

    public void CleanUp()
    {
        Character.OnHPChanged -= OnHPChanged;
    }

    private void OnHPChanged(Character obj)
    {
        UpdateHPBar();
    }

    void UpdateHPBar()
    {
        _hpBar.SetProgress(Character.CurrentHP / Character.MaxHP);
    }
}