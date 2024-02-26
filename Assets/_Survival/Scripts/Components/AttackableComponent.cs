using UnityEngine;

public class AttackableComponent : MonoBehaviour, IAttackable
{
    private Character _character;
    public bool IsRanger;
    [SerializeField] private ProjectileType _projectileType;

    public void SetInfo(Character character)
    {
        _character = character;
    }

    public void CauseDamage(IDamageable target)
    {
        if (!IsRanger)
        {
            target.TakeDamage(this);
        }
        else
        {
            InstantiateProjectile();
        }
    }

    protected virtual void InstantiateProjectile()
    {
        var projectileData = new ProjectileData
        {
            StartPosition = transform.position,
            Range = _character.Range,
            Attacker = this,
            Speed = _character.CurrentData.AttackSpeed * 20,
            Direction = (_character.Target.GetTransform().position - transform.position).normalized
        };
        var proj = GameManager.Instance.ObjectPooler.InstantiateProjectile(_projectileType);
        proj.SetInfo(projectileData);
    }

    public float GetDamage()
    {
        return _character.Damage;
    }

    public Transform GetTranform()
    {
        return _character.transform;
    }

    public TeamType GetTeamType()
    {
        return _character.TeamType;
    }

    public Character GetAttacker()
    {
        return _character;
    }
}