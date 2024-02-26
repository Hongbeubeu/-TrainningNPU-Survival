using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ExtraEffectData", menuName = "Datas/ExtraEffectData", order = 0)]
public class ExtraEffectData : ScriptableObject
{
    public EffectUnitData[] ExtraEffects;
}

[Serializable]
public class EffectUnitData
{
    public ExtraEffectType Type;
    public float Damage;
    public float CoolDown;
    public float Duration;
    public float Range;
    public int MaxTarget;
    public float Size;
    public int MinUnit;
    public int MaxUnit;
}