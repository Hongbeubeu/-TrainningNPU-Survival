public class Looter : Skill
{
    public Looter(Character attacker, SkillType type) : base(attacker, type)
    {
        SetData(_data);
    }

    protected sealed override void SetData(SkillUnitData data)
    {
        GameController.Instance.Player.CurrentData.LooterXP =
            GameManager.Instance.PlayerData.BaseData.LooterXP + data.Rate1;
        GameController.Instance.Player.CurrentData.LooterHP =
            GameManager.Instance.PlayerData.BaseData.LooterHP + data.Rate2;
        GameController.Instance.Player.CurrentData.LooterGold =
            GameManager.Instance.PlayerData.BaseData.LooterGold + data.Rate3;
    }
}