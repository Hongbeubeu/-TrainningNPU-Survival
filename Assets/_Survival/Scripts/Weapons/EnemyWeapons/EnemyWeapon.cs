using UnityEngine;

public class EnemyWeapon : IAttackable
{
    protected EnemyWeaponController _controller;
    public EnemyWeaponType Type;
    protected EnemyWeaponUnitData _data;
    public Enemy Attacker;
    protected bool _isCaculate;
    protected float _coolDown;
    protected TeamType _teamType;
    protected int _currentLevel;

    public EnemyWeapon(int weaponId, TeamType teamType)
    {
        _coolDown = 0f;
        _teamType = teamType;
        _isCaculate = true;
        _currentLevel = 0;
        _data = GameManager.Instance.EnemyWeaponData.EnemyWeaponUnitDatas[weaponId];
        Type = _data.Type;
    }

    public virtual void OnUpdate(float dt)
    {
    }


    public virtual void SetInfo(Enemy attacker, EnemyWeaponController controller)
    {
        _controller = controller;
        Attacker = attacker;
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

    public virtual void SpawnWeapon()
    {
    }

    public virtual void Destroy()
    {
    }
}