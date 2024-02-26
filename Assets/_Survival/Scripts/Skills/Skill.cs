using UnityEngine;

public class Skill
{
    public SkillType Type;
    protected SkillUnitData _data;
    protected Character _attacker;
    protected int _currentLevel;

    public Skill(Character attacker, SkillType type)
    {
        _attacker = attacker;
        Type = type;
        _currentLevel = 0;
        _data = GameManager.Instance.SkillData.SkillDatas[(int)Type].SkillLevelDatas[_currentLevel];
    }

    public int CurrentLevel => _currentLevel;

    protected virtual void ResetData()
    {
    }

    protected virtual void SetData(SkillUnitData data)
    {
    }

    public void LevelUp()
    {
        _currentLevel = CurrentLevel + 1;
        _currentLevel = Mathf.Clamp(_currentLevel, 0,
            GameManager.Instance.SkillData.SkillDatas[(int)Type].SkillLevelDatas.Length - 1);
        var data = GameManager.Instance.SkillData.SkillDatas[(int)Type].SkillLevelDatas[CurrentLevel];
        SetData(data);
    }
}