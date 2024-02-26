using UnityEngine;

public class Character : MonoBehaviour
{
    private float _currentHP;
    public TeamType TeamType;
    public float Damage;
    public float Speed;
    public float MaxHP;
    public PlayerStat CurrentData;
    public IDamageable Target;
    public WeaponType DefaultWeapon;
    public float Range;

    [SerializeField] private CharacterView _characterView;

    public float CurrentHP
    {
        get => _currentHP;
        set
        {
            if (value > MaxHP)
                value = MaxHP;
            _currentHP = value;
            OnHPChanged?.Invoke(this);
            if (value <= 0)
                Die();
        }
    }

    public event System.Action<Character> OnHPChanged;

    private void Awake()
    {
        _characterView?.Setup(this);
    }

    private void OnDisable()
    {
        _characterView?.CleanUp();
    }

    public virtual void SetHP(float value)
    {
        CurrentHP = value;
    }

    public virtual void Die()
    {
    }
}