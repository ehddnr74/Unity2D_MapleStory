using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public class SkillData
{
    public string skillName; // 스킬 이름
    public int skillLevel; // 스킬 레벨
}

[System.Serializable]
public class SkillCollection
{
    public List<SkillData> skills = new List<SkillData>(); // 스킬 목록
    public int skillPoint;
}