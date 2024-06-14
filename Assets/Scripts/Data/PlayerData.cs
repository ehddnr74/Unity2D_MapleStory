using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;

[System.Serializable]
public class PlayerData
{
    public string name;
    public int level;
    public int meso;
    public int experience;
    public float criticalProbability;
    public Dictionary<int, int> experienceTable; // 레벨별 경험치 요구량
    public Dictionary<int, int> baseHPTable; // 레벨별 기본 HP
    public Dictionary<int, int> baseMPTable; // 레벨별 기본 MP
}


