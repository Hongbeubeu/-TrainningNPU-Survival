public enum GameState : byte
{
    Pause,
    Play
}

public enum WeaponType : byte
{
    Uzi,
    ShortGun,
    Laser,
    Bomb,
    Mine,
    Boomerang,
    FlySaws,
    Sword,
    HeatWave,
    Lightning,
    Drone,
}

public enum EnemyWeaponType : byte
{
    Boss_FlowerGun,
    Boss_HeavyPunch,
    Boss_CrazyChase
}

public enum SkillType : byte
{
    Magnet,
    Sprinter,
    Specialist,
    Critical,
    Dodger,
    Looter
}

public enum PermanentUpgradeType : byte
{
    HP,
    DamageMultiplier,
    XPBoost,
    GoldBoost,
    CriticalChance,
    CriticalDamageMultiplier
}

public enum CollectableItemType : byte
{
    HP,
    XP,
    Gold,
    Weapon,
    Skill,
    Roll,
    None
}

public enum EnemyTier : byte
{
    Creep,
    Elite,
    Boss
}

public enum TeamType : byte
{
    Ally,
    Enemy,
    None
}

public enum ProjectileType : byte
{
    Bullet,
    LaserBeam,
    Bomb,
    Mine,
    Boomerang,
    Saw,
    SwordSlash,
    HeatWave,
    Lightning,
    TargetedBullet,
    EnemyBullet1,
    BossPunch,
}

public enum ExtraEffectType : byte
{
    FireEffect
}

public enum EffectType : byte
{
    Lightning_Explosion,
    Hit,
    HeatWave,
    FireField,
    DamgeEffect,
}

public enum PlayerAnimState : byte
{
    Idle,
    Run
}

public enum AIState : byte
{
    Idle,
    Move,
    Attack
}

public enum FSMTransition : byte
{
    ToIdle,
    ToMove,
    ToAttack
}