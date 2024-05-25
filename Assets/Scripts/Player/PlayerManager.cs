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

    private Image levelTensImage; //레벨 10의 자리 
    private Image levelUnitsImage; // 레벨 1의 자리 

    private Sprite[] numberSprites; //0부터 9까지 숫자 스프라이트 배열 


    private void Awake()
    {
    }
    private void Start()
    {
        // 데이터 매니저를 통해 플레이어 데이터 로드
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
                // 레벨이 10 미만인 경우, 오른쪽 이미지에만 숫자 표기
                levelTensImage.enabled = false;
                levelTensImage.sprite = null;
                levelUnitsImage.sprite = numberSprites[nowLevel];
            }
            else if (nowLevel >= 10 && nowLevel < 100)
            {
                // 레벨이 10 이상 100 미만인 경우, 왼쪽 이미지에 십의 자리 숫자, 오른쪽 이미지에 일의 자리 숫자 표기
                int tensDigit = nowLevel / 10; // 십의 자리 숫자
                int unitsDigit = nowLevel % 10; // 일의 자리 숫자

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
            string spriteName = "Level/" + i.ToString(); // 이미지 파일 이름 설정
            Sprite sprite = Resources.Load<Sprite>(spriteName); // 해당 이름의 이미지 로드
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