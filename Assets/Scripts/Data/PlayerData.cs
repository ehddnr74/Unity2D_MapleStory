using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;

[System.Serializable]
public class PlayerData
{
    public string name;
    public int level;
    public int meso;
    public int experience;
    public Dictionary<int, int> experienceTable; // ������ ����ġ �䱸��


    //public PlayerData()
    //{
    //    name = "DefaultName";
    //    level = 1;
    //    experience = 0;
    //    experienceTable = new Dictionary<int, int>();
    //}
}

    //public StatData stats;
    //public SkillData skills;
    //public List<ItemData> items;
