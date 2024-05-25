using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class PlayerManager : MonoBehaviour
{
    private PlayerData playerData;
    private TextMeshProUGUI nameText;

    private Image levelTensImage; //���� 10�� �ڸ� 
    private Image levelUnitsImage; // ���� 1�� �ڸ� 

    private Sprite[] numberSprites; //0���� 9���� ���� ��������Ʈ �迭 


    private void Awake()
    {
    }
    private void Start()
    {
        // ������ �Ŵ����� ���� �÷��̾� ������ �ε�
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadData();
            playerData = DataManager.instance.nowPlayer;

            numberSprites = LoadNumberSprites();

            levelTensImage = GameObject.Find("MainBar/Panel/Tens").GetComponentInChildren<Image>();
            levelUnitsImage = GameObject.Find("MainBar/Panel/Units").GetComponentInChildren<Image>();
            nameText = GameObject.Find("MainBar").GetComponentInChildren<TextMeshProUGUI>();

            UpdateLevelUI(playerData.level);
            UpdateNameUI();
        }
        else
        {
            Debug.LogError("DataManager instance is not initialized.");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
           DataManager.instance.AddExperience(50);
        }
    }

    public void UpdateLevelUI(int nowLevel)
    {
        if (levelTensImage != null && levelUnitsImage != null && playerData != null)
        {
            playerData.level = nowLevel;
            if (nowLevel < 10)
            {
                // ������ 10 �̸��� ���, ������ �̹������� ���� ǥ��
                levelTensImage.enabled = false;
                levelTensImage.sprite = null;
                levelUnitsImage.sprite = numberSprites[nowLevel];
            }
            else if (nowLevel >= 10 && nowLevel < 100)
            {
                // ������ 10 �̻� 100 �̸��� ���, ���� �̹����� ���� �ڸ� ����, ������ �̹����� ���� �ڸ� ���� ǥ��
                int tensDigit = nowLevel / 10; // ���� �ڸ� ����
                int unitsDigit = nowLevel % 10; // ���� �ڸ� ����

                levelTensImage.enabled = true;
                levelTensImage.sprite = numberSprites[tensDigit];
                levelUnitsImage.sprite = numberSprites[unitsDigit];            
            }
            else
            {
                Debug.LogWarning("Level is out of range for the numberSprites array.");
            }
        }
        else
        {
            Debug.LogWarning("Level UI elements or PlayerData is not initialized.");
        }
    }

    private void UpdateNameUI()
    {
        if(nameText != null && playerData != null)
        {
            nameText.text = playerData.name;
        }
        else
        {
            Debug.LogWarning("LevelText or PlayerData is not initialized.");
        }
    }

    private Sprite[] LoadNumberSprites()
    {
        Sprite[] numberSprites = new Sprite[10];
        for (int i = 0; i < 10; i++)
        {
            string spriteName = "Level/" + i.ToString(); // �̹��� ���� �̸� ����
            Sprite sprite = Resources.Load<Sprite>(spriteName); // �ش� �̸��� �̹��� �ε�
            if (sprite != null)
            {
                numberSprites[i] = sprite;
            }
            else
            {
                Debug.LogError("Failed to load sprite: " + spriteName);
            }
        }
        return numberSprites;
    }



    [ContextMenu("Save Player Data")]
    void SavePlayerDataToJson()
    {
        if (DataManager.instance != null)
        {
            DataManager.instance.SaveData();
        }
        else
        {
            Debug.LogError("DataManager instance is not initialized.");
        }
    }

    [ContextMenu("Load Player Data")]
    void LoadPlayerDataFromJson()
    {
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadData();
            playerData = DataManager.instance.nowPlayer;
        }
        else
        {
            Debug.LogError("DataManager instance is not initialized.");
        }
    }
}