using Ultimate.Core.Runtime.Singleton;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerProfile PlayerProfile;
    public PlayerData PlayerData;
    public ObjectPooler ObjectPooler;
    public EnemyMapData EnemyMapData;
    public EnemyData EnemyData;
    public WeaponData WeaponData;
    public SkillData SkillData;
    public EnemyWeaponData EnemyWeaponData;
    public ExtraEffectData ExtraEffectData;
    public PermanentUpgradeDatas PermanentUpgradeDatas;
    public BoxItemData BoxItemData;
    public CollectableDropData CollectableDropData;
    public GameConfig GameConfig;

    public Sprite[] CollectableSprites;
    public Sprite[] WeaponSprites;
    public Sprite GoldSprite;
    [SerializeField] private Sprite[] _skillSprites;

    public Sprite GetSkillSpriteIcon(int index)
    {
        index = Mathf.Clamp(index, 0, _skillSprites.Length - 1);
        return _skillSprites[index];
    }

    public Sprite GetWeaponSpriteIcon(int index)
    {
        index = Mathf.Clamp(index, 0, WeaponSprites.Length - 1);
        return WeaponSprites[index];
    }

    public override void Init()
    {
        PlayerProfile.Load();
        Application.targetFrameRate = GameConfig.TargetFamerate;
    }
}