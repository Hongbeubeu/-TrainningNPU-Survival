using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveData", menuName = "Datas/EnemyWaveData", order = 0)]
public class EnemyWaveData : ScriptableObject
{
    public float DelayBeforeSpawnWave;
    public List<WaveUnitData> WaveData;
}