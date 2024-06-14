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
    public Dictionary<int, int> experienceTable; // ������ ����ġ �䱸��
    public Dictionary<int, int> baseHPTable; // ������ �⺻ HP
    public Dictionary<int, int> baseMPTable; // ������ �⺻ MP
}


