using Ultimate.Core.Runtime.Pool;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPool", menuName = "Datas/ObjectPool")]
public class ObjectPooler : ScriptableObject
{
    public Projectile[] Projectiles;
    public ExtraEffect[] ExtraEffects;
    public Enemy[] Enemies;
    public CollectableItemComponent CollectableItem;
    public BoxItem BoxItem;
    public GameObject Drone;
    [SerializeField] private Effect[] _effects;

    public Projectile InstantiateProjectile(ProjectileType type)
    {
        return FastPoolManager.GetPool(Projectiles[(int)type]).FastInstantiate<Projectile>();
    }

    public void DestroyProjectile(GameObject go, ProjectileType type)
    {
        FastPoolManager.GetPool(Projectiles[(int)type]).FastDestroy(go);
    }

    public Enemy InstantiateEnemy(EnemyTier tier)
    {
        return FastPoolManager.GetPool(Enemies[(int)tier]).FastInstantiate<Enemy>();
    }

    public void DestroyEnemy(GameObject go, EnemyTier tier)
    {
        FastPoolManager.GetPool(Enemies[(int)tier]).FastDestroy(go);
    }

    public CollectableItemComponent InstantiateCollectableItem()
    {
        return FastPoolManager.GetPool(CollectableItem).FastInstantiate<CollectableItemComponent>();
    }

    public void DestroyCollectableItem(GameObject go)
    {
        FastPoolManager.GetPool(CollectableItem).FastDestroy(go);
    }

    public Transform InstantiateDrone()
    {
        return FastPoolManager.GetPool(Drone).FastInstantiate<Transform>();
    }

    public ExtraEffect InstantiateExtraEffect(ExtraEffectType type)
    {
        return FastPoolManager.GetPool(ExtraEffects[(int)type]).FastInstantiate<ExtraEffect>();
    }

    public void DestroyExtraEffect(GameObject go, ExtraEffectType type)
    {
        FastPoolManager.GetPool(ExtraEffects[(int)type]).FastDestroy(go);
    }

    public BoxItem InstantiateBoxItem()
    {
        return FastPoolManager.GetPool(BoxItem).FastInstantiate<BoxItem>();
    }

    public void DestroyBoxItem(GameObject go)
    {
        FastPoolManager.GetPool(BoxItem).FastDestroy(go);
    }

    public Effect InstantiateEffect(EffectType type)
    {
        return FastPoolManager.GetPool(_effects[(int)type]).FastInstantiate<Effect>();
    }

    public void DestroyEffect(EffectType type, GameObject go)
    {
        FastPoolManager.GetPool(_effects[(int)type]).FastDestroy(go);
    }
}