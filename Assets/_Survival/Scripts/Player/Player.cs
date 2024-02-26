using UnityEngine;
using Random = UnityEngine.Random;

public class Player : Character
{
    public PlayerData Data;
    public DamageableComponent Damageable;
    public PlayerMovement PlayerMovement;
    public WeaponController WeaponController;
    public SkillController SkillController;
    public PlayerXPController PlayerXpController;
    public PlayerAnimatorController AnimatorController;
    public PlayerMotionDirection PlayerMotionDirection;
    private int _currentLevel;
    [SerializeField] private Transform _projectPoint;

    public Transform ProjectPoint => _projectPoint;

    private void Start()
    {
        Init();
        Damageable?.SetInfo(this);
        PlayerMovement?.SetInfo(this);
        PlayerXpController?.SetInfo(this);
        WeaponController?.SetInfo(this);
        CurrentData = GameManager.Instance.PlayerData.PlayerStat;
    }

    private void Init()
    {
        Data = GameManager.Instance.PlayerData;
        CurrentData = new PlayerStat(Data.PlayerStat);
        MaxHP = Data.PlayerStat.MaxHP;
        CurrentHP = MaxHP;
    }

    public void LevelUp()
    {
        _currentLevel++;
        UIController.Instance.InGamePanel.SelectWeaponSkillPanel.RandomWeaponSkill();
        // RandomWeaponSkill();
    }

    public void RandomWeaponSkill()
    {
        var number = GameManager.Instance.WeaponData.WeaponDatas.Length +
                     GameManager.Instance.SkillData.SkillDatas.Length;
        var rand = Random.Range(0, number);
        if (rand < GameManager.Instance.WeaponData.WeaponDatas.Length)
        {
            WeaponController.AddWeapon(rand);
        }
        else
        {
            rand = Mathf.Clamp(rand, 0, GameManager.Instance.SkillData.SkillDatas.Length - 1);
            SkillController.AddSkill((SkillType)rand);
        }
    }

    public void ResetData()
    {
        Data.ResetTempData();
        CurrentData = Data.PlayerStat;
        CurrentHP = CurrentData.MaxHP;
        PlayerMovement?.ResetData();
        PlayerXpController?.ResetData();
        SkillController?.ResetData();
        WeaponController?.ResetData();
    }

    public void CollectItem(ICollectable collectable)
    {
        switch (collectable.GetItemType())
        {
            case CollectableItemType.HP:
                CurrentHP += collectable.GetValue() + collectable.GetValue() * CurrentData.LooterHP;
                break;
            case CollectableItemType.XP:
                PlayerXpController.CurrentXP += collectable.GetValue() + collectable.GetValue() * CurrentData.LooterXP;
                break;
            case CollectableItemType.Gold:
                Data.Gold +=
                    (int)collectable.GetValue() + (int)(collectable.GetValue() * CurrentData.LooterGold);
                break;
            case CollectableItemType.Weapon:
                WeaponController.AddWeapon((int)GameController.Instance.RandomWeapon());
                break;
            case CollectableItemType.Skill:
                SkillController.AddSkill(GameController.Instance.RandomSkill());
                break;
            case CollectableItemType.Roll:
                RandomWeaponSkill();
                break;
        }
    }

    public override void Die()
    {
        UIController.Instance.InGamePanel.EndGamePanel.SetInfo(false);
        GameController.Instance.CurrentGameState = GameState.Pause;
        ResetData();
        GameController.Instance.ResetGame();
    }
}