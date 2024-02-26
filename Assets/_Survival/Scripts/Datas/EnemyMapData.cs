using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyMapData", menuName = "Datas/EnemyMapData", order = 0)]
public class EnemyMapData : ScriptableObject
{
    public List<EnemyWaveData> EnemyByMapData;
}

[Serializable]
public class WaveUnitData
{
    public EnemyTier Tier;
    public int Level;
    public int Count;
    public float SpawnDeltaTime;
}