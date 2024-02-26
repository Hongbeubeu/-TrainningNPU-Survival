public class Specialist : Skill
{
    public Specialist(Character attacker, SkillType type) : base(attacker, type)
    {
        SetData(_data);
    }

    protected sealed override void SetData(SkillUnitData data)
    {
        GameController.Instance.Player.CurrentData.SpecialistChance =
            GameManager.Instance.PlayerData.BaseData.SpecialistChance + data.Rate1;
    }
}