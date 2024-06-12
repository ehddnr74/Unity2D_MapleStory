using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public class SkillData
{
    public string skillName; // ��ų �̸�
    public int skillLevel; // ��ų ����
    //public Dictionary<int, SkillEffect> levelEffects; // ��ų ������ ȿ��
}

[System.Serializable]
public class SkillCollection
{
    public List<SkillData> skills = new List<SkillData>(); // ��ų ���
    public int skillPoint;
}

//[System.Serializable]
//public class SkillEffect
//{
//    public float hpReduction; // HP �Ҹ�
//    public float mpReduction; // MP �Ҹ�
//    public float damageIncrease; // ������ ������
//    public float cooldownReduction; // ��Ÿ�� ���ҷ�
//    public float duration; // ���� �ð�
//    public float speedIncrease; // �ӵ� ������
//    public float jumpDistanceIncrease; // ���� ������
//    public float attackSpeedIncrease; // ���� �ӵ� ������
//    public float criticalChanceIncrease; // ũ��Ƽ�� Ȯ�� ������
//}