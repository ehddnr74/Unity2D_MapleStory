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
        // ������ �Ŵ����� ���� �÷��̾� ������ �ε�
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
        // ���� ������ ���� 
        int currentLevel = playerData.level;
        int currentExperience = playerData.experience;
        int experienceForNextLevel = playerData.experienceTable[currentLevel];

        // ����ġ ���� ��� (0 ~ 1 ���� ��)
        float experienceRatio = (float)currentExperience / experienceForNextLevel;
        experienceBar.size = experienceRatio;
    }
}

