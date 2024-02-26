using UnityEngine;

[CreateAssetMenu(fileName = "XPByLevelData", menuName = "Datas/XPBylevelData", order = 0)]
public class XPByLevelData : ScriptableObject
{
    public float[] XPByLevel;

    public float GetMaxXPByLevel(int level)
    {
        return XPByLevel[Mathf.Clamp(level - 1, 0, XPByLevel.Length - 1)];
    }
}