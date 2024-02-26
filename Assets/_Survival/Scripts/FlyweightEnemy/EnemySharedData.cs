public struct EnemySharedData
{
    public EnemyTier Tier;
    public float MaxHP;
    public float Size;
    public bool IsRanger;
    public float AttackSpeed;
    public float Damage;
    public float Range;
    public int CollectableDropLevel;

    public EnemySharedData(EnemyTier tier, float maxHP, float size, bool isRanger, float attackSpeed, float damage,
        float range,
        int collectableDropLevel)
    {
        Tier = tier;
        MaxHP = maxHP;
        Size = size;
        IsRanger = isRanger;
        AttackSpeed = attackSpeed;
        Damage = damage;
        Range = range;
        CollectableDropLevel = collectableDropLevel;
    }

    public EnemySharedData(EnemyUnitData data)
    {
        Tier = data.Tier;
        MaxHP = data.HP;
        Size = data.Size;
        IsRanger = data.IsRanger;
        AttackSpeed = data.AttackSpeed;
        Damage = data.Damage;
        Range = data.Range;
        CollectableDropLevel = data.CollectableDropLevel;
    }
}