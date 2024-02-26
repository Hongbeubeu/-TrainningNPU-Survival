public class Sprinter : Skill
{
    public Sprinter(Character attacker, SkillType type) : base(attacker, type)
    {
        SetData(_data);
    }

    protected sealed override void SetData(SkillUnitData data)
    {
        GameController.Instance.Player.CurrentData.SpeedUp =
            GameManager.Instance.PlayerData.BaseData.SpeedUp + data.Rate1;
        GameController.Instance.Player.CurrentData.SpeedBurst =
            GameManager.Instance.PlayerData.BaseData.SpeedUp + data.Rate2;
        GameController.Instance.Player.CurrentData.SpeedBurstCoolDown =
            GameManager.Instance.PlayerData.BaseData.SpeedUp + data.T1;
        GameController.Instance.Player.CurrentData.SpeedBurstDuration =
            GameManager.Instance.PlayerData.BaseData.SpeedUp + data.T2;
    }
}