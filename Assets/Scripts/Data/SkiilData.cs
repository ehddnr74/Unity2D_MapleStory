using System.Collections.Generic;

[System.Serializable]
public class SkillData
{
    public string skillName; // ��ų �̸�
    public int skillLevel; // ��ų ����
    public Dictionary<int, SkillEffect> levelEffects; // ��ų ������ ȿ��
}

[System.Serializable]
public class SkillCollection
{
    public List<SkillData> skills = new List<SkillData>(); // ��ų ���
    public int skillPoint;
}

[System.Serializable]
public class SkillEffect
{
    public int hpReduction; // HP �Ҹ�
    public int mpReduction; // MP �Ҹ�
    public float damageIncrease; // ������ ������
    public float coolDown; //��Ÿ��
    public float buffDuration; // ���� ���ӽð�
    public float cooldownReduction; // ��Ÿ�� ���ҷ�
    public float duration; // ���� �ð�
    public float speedIncrease; // �ӵ� ������
    public float jumpDistanceIncrease; // ���� ������
    public float attackSpeedIncrease; // ���� �ӵ� ������
    public float criticalChanceIncrease; // ũ��Ƽ�� Ȯ�� ������
    public string toolTipPath;
}