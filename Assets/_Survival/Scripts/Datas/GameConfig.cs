using UnityEngine;


[CreateAssetMenu(fileName = "GameConfig", menuName = "Datas/GameConfig", order = 0)]
public class GameConfig : ScriptableObject
{
    public int TargetFamerate;
    public float MaxLifeTimeProjectile;
    public int MaxWeaponSlot;
    public int MaxSkillSlot;
    public float TimeBoxSpawn;
    public float TimeBoxAlive;

    public int BossMaxWeaponSlot;
    public int GoldBonusLevelUp;

    public float MinSpawnRange;
    public float MaxSpawnRange;
}