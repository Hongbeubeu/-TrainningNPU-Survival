public class Dodger : Skill
{
    public Dodger(Character attacker, SkillType type) : base(attacker, type)
    {
        SetData(_data);
    }

    protected sealed override void SetData(SkillUnitData data)
    {
        GameController.Instance.Player.CurrentData.DodgerChance =
            GameManager.Instance.PlayerData.BaseData.DodgerChance + data.Rate1;
    }
}