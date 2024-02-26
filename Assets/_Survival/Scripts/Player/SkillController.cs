using UnityEngine;

public class SkillController : MonoBehaviour
{
    private Character Attacker;
    private Skill[] _skills;
    private int _currentIndex;

    public int CountOwnedSkill()
    {
        for (var i = 0; i < _skills.Length; i++)
        {
            if (_skills[i] == null)
                return i;
        }

        return _skills.Length;
    }

    public Skill GetSkillAtIndex(int index)
    {
        return _skills[index];
    }

    public void AddSkill(SkillType type)
    {
        GameController.Instance.WeaponSkillRandomizer.UpdateSkill(type);
        for (var i = 0; i < _skills.Length; i++)
        {
            if (_skills[i] == null) continue;
            if (_skills[i].Type != type) continue;
            _skills[i].LevelUp();
            return;
        }

        if (_currentIndex == GameManager.Instance.GameConfig.MaxSkillSlot)
            return;
        _skills[_currentIndex] = GenerateSkill(type);
        _currentIndex++;
    }

    private Skill GenerateSkill(SkillType type)
    {
        var skill = new Skill(Attacker, type);
        switch (type)
        {
            case SkillType.Magnet:
                skill = new Magnet(Attacker, type);
                break;
            case SkillType.Sprinter:
                skill = new Sprinter(Attacker, type);
                break;
            case SkillType.Specialist:
                skill = new Specialist(Attacker, type);
                break;
            case SkillType.Critical:
                skill = new Critical(Attacker, type);
                break;
            case SkillType.Dodger:
                skill = new Dodger(Attacker, type);
                break;
            case SkillType.Looter:
                skill = new Looter(Attacker, type);
                break;
        }

        return skill;
    }

    private void Start()
    {
        Attacker = GameController.Instance.Player;
        _skills = new Skill[GameManager.Instance.GameConfig.MaxSkillSlot];
    }

    public void ResetData()
    {
        _currentIndex = 0;
        for (var i = 0; i < _skills.Length; i++)
        {
            _skills[i] = null;
        }
    }

    public int GetCurrentLevelSkill(SkillType type)
    {
        if (_skills.Length <= 0)
            return 0;
        for (var i = 0; i < _skills.Length; i++)
        {
            if (_skills[i] == null) continue;
            if (_skills[i].Type == type)
                return _skills[i].CurrentLevel + 1;
        }

        return 0;
    }
}