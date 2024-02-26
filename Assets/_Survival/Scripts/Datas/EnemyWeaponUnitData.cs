using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWeaponUnitData", menuName = "Datas/EnemyWeaponUnitData", order = 0)]
public class EnemyWeaponUnitData : ScriptableObject
{
    public EnemyWeaponType Type;
    public float CoolDownTime;
    public float DurationTime;
    public float AttackSpeed;
    public float Damage;
    public int NumberOfProjectilePerHit;
    public int MaxTarget;
    public float Angle;
    public float ProjectileSpeed;
    public float Range;
    public float ProjectileRange;
    public float TriggerRadius;
    public float FireChance;
    public int NumberOfWeapon;
}