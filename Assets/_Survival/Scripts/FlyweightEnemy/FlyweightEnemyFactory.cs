using System.Collections.Generic;

public struct ShareEnemyDataKey
{
    public EnemyTier Tier;
    public int Level;

    public ShareEnemyDataKey(EnemyTier tier, int level)
    {
        Tier = tier;
        Level = level;
    }
}

public class FlyweightEnemyFactory
{
    private readonly Dictionary<ShareEnemyDataKey, EnemySharedData> EnemySharedDatas = new();
    private readonly EnemyFlyweightPooler<FlyweightEnemy> _pooler = new(200);

    public FlyweightEnemy GetEnemy()
    {
        var e = _pooler.GetEnemy();
        return e;
    }

    public void ReturnEnemy(FlyweightEnemy e)
    {
        _pooler.ReturnEnemy(e);
    }

    public EnemySharedData GetEnemySharedData(EnemyTier tier, int level)
    {
        var key = new ShareEnemyDataKey(tier, level);
        if (EnemySharedDatas.ContainsKey(key))
        {
            return EnemySharedDatas[key];
        }

        var data = new EnemySharedData(GameManager.Instance.EnemyData.GetEnemyUnitData(level, tier));
        EnemySharedDatas[key] = data;
        return data;
    }
}