public class Critical : Skill
{
    public Critical(Character attacker, SkillType type) : base(attacker, type)
    {
        SetData(_data);
    }

    protected sealed override void SetData(SkillUnitData data)
    {
        GameController.Instance.Player.CurrentData.CriticalChance =
            GameManager.Instance.PlayerData.BaseData.CriticalChance + data.Rate1;
        GameController.Instance.Player.CurrentData.CriticalDamageMultiplier =
            GameManager.Instance.PlayerData.BaseData.CriticalDamageMultiplier + data.Rate2;
    }
}