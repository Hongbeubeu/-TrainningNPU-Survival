using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(IAttackable attacker);
    public Transform GetTransform();
    public TeamType GetTeamType();
    public void Destroy();
}