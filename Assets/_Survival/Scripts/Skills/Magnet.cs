public class Magnet : Skill
{
    public Magnet(Character attacker, SkillType type) : base(attacker, type)
    {
        SetData(_data);
    }

    protected sealed override void SetData(SkillUnitData data)
    {
        GameController.Instance.Player.CurrentData.MagnetRange =
            GameManager.Instance.PlayerData.BaseData.MagnetRange + data.Range;
    }
}