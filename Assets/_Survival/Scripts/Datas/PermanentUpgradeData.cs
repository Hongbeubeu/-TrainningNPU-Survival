using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PermanentUpgradeData", menuName = "Datas/PermanentUpgradeData", order = 0)]
public class PermanentUpgradeData : ScriptableObject
{
    public PermanentUpgradeUnit[] Datas;
}

[Serializable]
public class PermanentUpgradeUnit
{
    public float Value;
    public float Price;
}