using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : IAttackable
{
    public WeaponType Type;
    protected WeaponUnitData _data;
    public Character Attacker;
    protected bool _isCaculate;
    protected float _coolDown;
    protected TeamType _teamType;
    protected int _currentLevel;

    public Weapon(int weaponId, TeamType teamType)
    {
        _coolDown = 0f;
        _teamType = teamType;
        _isCaculate = true;
        _currentLevel = 0;
        _data = GameManager.Instance.WeaponData.WeaponDatas[weaponId].BaseWeaponDatas[_currentLevel];
        Type = _data.Type;
    }

    public int CurrentLevel => _currentLevel;

    public virtual void SetInfo(Character attacker)
    {
        Attacker = attacker;
    }

    public virtual void LevelUp()
    {
        _currentLevel++;
        _currentLevel = Mathf.Clamp(_currentLevel, 0,
            GameManager.Instance.WeaponData.WeaponDatas[(int)Type].BaseWeaponDatas.Length - 1);
        _data = GameManager.Instance.WeaponData.WeaponDatas[(int)Type].BaseWeaponDatas[_currentLevel];
    }

    public virtual void SpawnWeapon()
    {
    }

    public virtual void Destroy()
    {
    }

    protected void GetSpecialistMulti(out int multi)
    {
        multi = 1;
        if (Attacker.CurrentData.SpecialistChance <= 0)
        {
            return;
        }

        var rand = Random.Range(0f, 1f);
        if (!(rand <= Attacker.CurrentData.SpecialistChance)) return;
        multi = 2;
    }

    public virtual void OnUpdate(float dt)
    {
    }

    public IDamageable FindNearestTarget(float range)
    {
        return GameController.Instance.GetNearestTarget(range);
    }

    public IDamageable FindTargetInRange(float range)
    {
        return GameController.Instance.GetTargetInRange(range);
    }

    public List<IDamageable> FindTargetsInRange(float range, int maxTarget)
    {
        return GameController.Instance.GetTargetsInRange(range, maxTarget);
    }

    public void CauseDamage(IDamageable target)
    {
        target.TakeDamage(this);
    }

    public float GetDamage()
    {
        return _data.Damage;
    }

    public Transform GetTranform()
    {
        return Attacker.transform;
    }

    public TeamType GetTeamType()
    {
        return _teamType;
    }

    public Character GetAttacker()
    {
        return Attacker;
    }
}