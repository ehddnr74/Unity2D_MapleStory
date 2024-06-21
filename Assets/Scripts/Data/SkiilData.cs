using System.Collections.Generic;

[System.Serializable]
public class SkillData
{
    public string skillName; // 스킬 이름
    public int skillLevel; // 스킬 레벨
    public Dictionary<int, SkillEffect> levelEffects; // 스킬 레벨별 효과
}

[System.Serializable]
public class SkillCollection
{
    public List<SkillData> skills = new List<SkillData>(); // 스킬 목록
    public int skillPoint;
}

[System.Serializable]
public class SkillEffect
{
    public int hpReduction; // HP 소모량
    public int mpReduction; // MP 소모량
    public float damageIncrease; // 데미지 증가량
    public float coolDown; //쿨타임
    public float buffDuration; // 버프 지속시간
    public float cooldownReduction; // 쿨타임 감소량
    public float duration; // 지속 시간
    public float speedIncrease; // 속도 증가량
    public float jumpDistanceIncrease; // 점프 증가량
    public float attackSpeedIncrease; // 공격 속도 증가량
    public float criticalChanceIncrease; // 크리티컬 확률 증가량
    public string toolTipPath;
}