using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyLevelData", menuName = "Datas/EnemyLevelData", order = 0)]
public class EnemyLevelData : ScriptableObject
{
    public List<EnemyUnitData> EnemyList;
}