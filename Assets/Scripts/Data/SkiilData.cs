using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public class SkillData
{
    public string skillName; // ��ų �̸�
    public int skillLevel; // ��ų ����
}

[System.Serializable]
public class SkillCollection
{
    public List<SkillData> skills = new List<SkillData>(); // ��ų ���
    public int skillPoint;
}