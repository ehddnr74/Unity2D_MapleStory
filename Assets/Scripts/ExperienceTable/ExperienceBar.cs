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
        // ������ �Ŵ����� ���� �÷��̾� ������ �ε�
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
        // ���� ������ ���� 
        playerData.level = currentlevel;
        playerData.experience = experience;
        int experienceForNextLevel = playerData.experienceTable[playerData.level];

        // ����ġ ���� ��� (0 ~ 1 ���� ��)
        float experienceRatio = (float)playerData.experience / experienceForNextLevel;
        experienceSlider.value = experienceRatio;
    }
}

