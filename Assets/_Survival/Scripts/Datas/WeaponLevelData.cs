using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponLevelData", menuName = "Datas/WeaponLevelData", order = 0)]
public class WeaponLevelData : ScriptableObject
{
    public WeaponUnitData[] BaseWeaponDatas;
}

[Serializable]
public class WeaponUnitData
{
    public WeaponType Type;
    public int Level;
    public float CoolDownTime;
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