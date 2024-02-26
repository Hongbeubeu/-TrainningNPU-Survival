using UnityEngine;

public interface IAttackable
{
    public void CauseDamage(IDamageable target);
    public float GetDamage();
    public Transform GetTranform();
    public TeamType GetTeamType();
    public Character GetAttacker();
}