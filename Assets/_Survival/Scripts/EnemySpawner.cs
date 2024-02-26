using System.Collections;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    public void StartSpawnEnemies(int level)
    {
        StartCoroutine(SpawnWave(level));
    }

    public void StopSpawnEnemies()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnWave(int mapIndex)
    {
        var enemyInMapData = GameManager.Instance.EnemyMapData.EnemyByMapData[mapIndex];
        int index = 0;
        foreach (var item in enemyInMapData.WaveData)
        {
            yield return new WaitForSeconds(enemyInMapData.DelayBeforeSpawnWave);
            for (var i = 0; i < item.Count; i++)
            {
                yield return new WaitForSeconds(item.SpawnDeltaTime);
                GameController.Instance.EnemyRenderController.CreateEnemy(item.Tier, item.Level, index);
                index++;
                // var enemy = GameManager.Instance.ObjectPooler.InstantiateEnemy(item.Tier);
                // enemy.SetInfo(GameManager.Instance.EnemyData.GetEnemyUnitData(item.Level, item.Tier));
            }
        }

        GameController.Instance.IsDoneSpawning = true;
    }
}