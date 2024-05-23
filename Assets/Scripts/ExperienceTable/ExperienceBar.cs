using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    private PlayerData playerData;
    public Scrollbar experienceBar;

    private void Start()
    {
        // 데이터 매니저를 통해 플레이어 데이터 로드
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadData();
            playerData = DataManager.instance.nowPlayer;
        }
    }

    private void Update()
    {
        UpdateExperienceBar();
    }

    public void UpdateExperienceBar()
    {
        // 현재 레벨과 다음 
        int currentLevel = playerData.level;
        int currentExperience = playerData.experience;
        int experienceForNextLevel = playerData.experienceTable[currentLevel];

        // 경험치 비율 계산 (0 ~ 1 사이 값)
        float experienceRatio = (float)currentExperience / experienceForNextLevel;
        experienceBar.size = experienceRatio;
    }
}

