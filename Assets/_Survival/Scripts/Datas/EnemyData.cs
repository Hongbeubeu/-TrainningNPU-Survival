using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Datas/EnemyData", order = 0)]
public class EnemyData : ScriptableObject
{
    public List<EnemyLevelData> EnemyTierDataList;

    public EnemyUnitData GetEnemyUnitData(int level, EnemyTier tier)
    {
        return (int)tier >= EnemyTierDataList.Count
            ? null
            : EnemyTierDataList[(int)tier].EnemyList.Where(e => e.Level == level).ToList().First();
    }
}

[Serializable]
public class EnemyUnitData
{
    public EnemyTier Tier;
    public bool IsRanger;
    public int Level;
    public float HP;
    public float Speed; // Moving speed
    public float AttackSpeed; //NumberAttack per second
    public float Damage;
    public float Range;
    public float Size;
    public int CollectableDropLevel;
}