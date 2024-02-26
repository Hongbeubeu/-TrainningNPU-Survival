using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Datas/SkillData", order = 0)]
public class SkillData : ScriptableObject
{
    public SkillLevelData[] SkillDatas;
}

[Serializable]
public class SkillUnitData
{
    public SkillType Type;
    public int Level;
    public float Range;
    public float Rate1;
    public float Rate2;
    public float Rate3;
    public float T1;
    public float T2;
}