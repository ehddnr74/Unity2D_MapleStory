using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    private PlayerData playerData;
    public Slider experienceSlider;

    private void Start()
    {
        // 데이터 매니저를 통해 플레이어 데이터 로드
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadData();
            playerData = DataManager.instance.nowPlayer;
        }

        UpdateExperienceBar(playerData.level, playerData.experience);
    }

    private void Update()
    {
       //UpdateExperienceBar();
    }

    public void UpdateExperienceBar(int currentlevel,int experience)
    {
        // 현재 레벨과 다음 
        playerData.level = currentlevel;
        playerData.experience = experience;
        int experienceForNextLevel = playerData.experienceTable[playerData.level];

        // 경험치 비율 계산 (0 ~ 1 사이 값)
        float experienceRatio = (float)playerData.experience / experienceForNextLevel;
        experienceSlider.value = experienceRatio;
    }
}

