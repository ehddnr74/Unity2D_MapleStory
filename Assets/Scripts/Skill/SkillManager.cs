using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEditor.Experimental.GraphView;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using System;

public class SkillManager : MonoBehaviour
{
    private string path;
    private string skillDataFilename = "Skill";  // 스킬 데이터 파일명
    public SkillCollection skillCollection;

    private TextMeshProUGUI skillPointText;
    private TextMeshProUGUI luckySevenText;
    private TextMeshProUGUI luckySevenLevel;
    private TextMeshProUGUI heistText;
    private TextMeshProUGUI heistLevel;
    private TextMeshProUGUI flashJumpText;
    private TextMeshProUGUI flashJumpLevel;


    private Button luckySevenBtn;
    private Button heistBtn;
    private Button flashJumpBtn;

    private Sprite activityBtn;
    private Sprite deactivityBtn;

    public GameObject SkillUIPanel;
    public bool activeUI = false;

    public bool itemChanged = false;

    private void Start()
    {
        path = Application.persistentDataPath + "/";

        // 스킬 데이터 로드
        skillCollection = LoadSkillData();

        if (skillCollection == null)
        {
            skillCollection = new SkillCollection();
        }

        Sprite activityButton = Resources.Load<Sprite>("StatButton/ActivityButton");
        activityBtn = activityButton;
        Sprite deactivityButton = Resources.Load<Sprite>("StatButton/DeActivityButton");
        deactivityBtn = deactivityButton;

        luckySevenBtn = GameObject.Find("Skill/SkillPanel/Lucky Seven/LuckySevenButton").GetComponentInChildren<Button>();
        heistBtn = GameObject.Find("Skill/SkillPanel/Heist/HeistButton").GetComponentInChildren<Button>();
        flashJumpBtn = GameObject.Find("Skill/SkillPanel/FlashJump/FlashJumpButton").GetComponentInChildren<Button>();

        luckySevenBtn.onClick.AddListener(OnLuckySevenButtonClick);
        heistBtn.onClick.AddListener(OnHeistButtonClick);
        flashJumpBtn.onClick.AddListener(OnFlashJumpButtonClick);

        skillPointText = GameObject.Find("Skill/SkillPanel/Skill Point Text").GetComponentInChildren<TextMeshProUGUI>();
        luckySevenText = GameObject.Find("Skill/SkillPanel/Lucky Seven/LuckySevenText").GetComponentInChildren<TextMeshProUGUI>();
        luckySevenLevel = GameObject.Find("Skill/SkillPanel/Lucky Seven/LuckySevenLevel").GetComponentInChildren<TextMeshProUGUI>();
        heistText = GameObject.Find("Skill/SkillPanel/Heist/HeistText").GetComponentInChildren<TextMeshProUGUI>();
        heistLevel = GameObject.Find("Skill/SkillPanel/Heist/HeistLevel").GetComponentInChildren<TextMeshProUGUI>();
        flashJumpText = GameObject.Find("Skill/SkillPanel/FlashJump/FlashJumpText").GetComponentInChildren<TextMeshProUGUI>();
        flashJumpLevel = GameObject.Find("Skill/SkillPanel/FlashJump/FlashJumpLevel").GetComponentInChildren<TextMeshProUGUI>();


        UpdateSkillUI();

        // 스킬 데이터 저장
        SaveSkillData(skillCollection);
        SkillUIPanel = GameObject.Find("SkillPanel");
        SkillUIPanel.SetActive(activeUI);
    }


    private void OnLuckySevenButtonClick()
    {
        skillCollection.skillPoint--;
        skillCollection.skills[0].skillLevel++;
        itemChanged = true;
    }
    private void OnHeistButtonClick()
    {
        skillCollection.skillPoint--;
        skillCollection.skills[1].skillLevel++;
        itemChanged = true;
    }
    private void OnFlashJumpButtonClick()
    {
        skillCollection.skillPoint--;
        skillCollection.skills[2].skillLevel++;
        itemChanged = true;
    }


    private void Update()
    {
        if(itemChanged)
        {
            itemChanged = false;
            SaveSkillData(skillCollection);
            UpdateSkillUI();
        }
    }

    private void AddSkill(string name, int level)
    {
        SkillData newSkill = new SkillData
        {
            skillName = name,
            skillLevel = level
        };
        skillCollection.skills.Add(newSkill);
    }

    public void UpdateSkillUI()
    {
        if (skillPointText != null)
        {
            skillPointText.text = skillCollection.skillPoint.ToString();
        }
        if (luckySevenText != null && skillCollection.skills.Count > 0
            && luckySevenLevel != null)
        {
            luckySevenText.text = skillCollection.skills[0].skillName;
            luckySevenLevel.text = skillCollection.skills[0].skillLevel.ToString();
        }
        if (heistText != null && skillCollection.skills.Count > 0
            && heistLevel != null)
        {
            heistText.text = skillCollection.skills[1].skillName;
            heistLevel.text = skillCollection.skills[1].skillLevel.ToString();
        }

        if (flashJumpText != null && skillCollection.skills.Count > 0
            && flashJumpLevel != null)
        {
            flashJumpText.text = skillCollection.skills[2].skillName;
            flashJumpLevel.text = skillCollection.skills[2].skillLevel.ToString();
        }

        if (skillCollection.skillPoint <= 0)
        {
            luckySevenBtn.interactable = false;
            luckySevenBtn.image.sprite = deactivityBtn;
            heistBtn.interactable = false;
            heistBtn.image.sprite = deactivityBtn;
            flashJumpBtn.interactable = false;
            flashJumpBtn.image.sprite = deactivityBtn;
        }
        else
        {
            luckySevenBtn.interactable = true;
            luckySevenBtn.image.sprite = activityBtn;
            heistBtn.interactable = true;
            heistBtn.image.sprite = activityBtn;
            flashJumpBtn.interactable = true;
            flashJumpBtn.image.sprite = activityBtn;
        }
    }


    public void SaveSkillData(SkillCollection skillCollection)
    {
        string json = JsonUtility.ToJson(skillCollection, true);
        File.WriteAllText(path + "Skill" + ".json", json);
    }

    public SkillCollection LoadSkillData()
    {
        string skillDataPath = path + skillDataFilename + ".json";

        if (File.Exists(skillDataPath))
        {
            string skillDatajson = File.ReadAllText(skillDataPath);
            SkillCollection loadedData = JsonConvert.DeserializeObject<SkillCollection>(skillDatajson);
            return loadedData;
        }
        else
        {
            return new SkillCollection(); // 데이터가 없으면 빈 SkillCollection 반환
        }
    }
}
