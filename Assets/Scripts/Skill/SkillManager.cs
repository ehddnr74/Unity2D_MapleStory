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
    private Player player;

    private TextMeshProUGUI skillPointText;
    private TextMeshProUGUI luckySevenText;
    private TextMeshProUGUI luckySevenLevel;
    private TextMeshProUGUI heistText;
    private TextMeshProUGUI heistLevel;
    private TextMeshProUGUI flashJumpText;
    private TextMeshProUGUI flashJumpLevel;
    private TextMeshProUGUI windBoosterText;
    private TextMeshProUGUI windBoosterLevel;
    private TextMeshProUGUI criticalShotText;
    private TextMeshProUGUI criticalShotLevel;


    private Button luckySevenBtn;
    private Button heistBtn;
    private Button flashJumpBtn;
    private Button windBoosterBtn;
    private Button criticalShotBtn;

    private Sprite activityBtn;
    private Sprite deactivityBtn;

    public GameObject SkillUIPanel;
    public bool activeUI = false;

    public bool itemChanged = false;

    private float playerOriginCriticalProbability;

    public static SkillManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        path = Application.persistentDataPath + "/";

        // 스킬 데이터 로드
        skillCollection = LoadSkillData();

        if (skillCollection == null)
        {
            skillCollection = new SkillCollection();
        }

        player = GameObject.Find("Player").GetComponent<Player>();
        playerOriginCriticalProbability = player.criticalProbability;

        Sprite activityButton = Resources.Load<Sprite>("StatButton/ActivityButton");
        activityBtn = activityButton;
        Sprite deactivityButton = Resources.Load<Sprite>("StatButton/DeActivityButton");
        deactivityBtn = deactivityButton;

        luckySevenBtn = GameObject.Find("Skill/SkillPanel/Lucky Seven/LuckySevenButton").GetComponentInChildren<Button>();
        heistBtn = GameObject.Find("Skill/SkillPanel/Heist/HeistButton").GetComponentInChildren<Button>();
        flashJumpBtn = GameObject.Find("Skill/SkillPanel/FlashJump/FlashJumpButton").GetComponentInChildren<Button>();
        windBoosterBtn = GameObject.Find("Skill/SkillPanel/Wind Booster/WindBoosterButton").GetComponentInChildren<Button>();
        criticalShotBtn = GameObject.Find("Skill/SkillPanel/Critical Shot/CriticalShotButton").GetComponentInChildren<Button>();

        luckySevenBtn.onClick.AddListener(OnLuckySevenButtonClick);
        heistBtn.onClick.AddListener(OnHeistButtonClick);
        flashJumpBtn.onClick.AddListener(OnFlashJumpButtonClick);
        windBoosterBtn.onClick.AddListener(OnWindBoosterButtonClick);
        criticalShotBtn.onClick.AddListener(OnCriticalShotButtonClick);

        skillPointText = GameObject.Find("Skill/SkillPanel/Skill Point Text").GetComponentInChildren<TextMeshProUGUI>();
        luckySevenText = GameObject.Find("Skill/SkillPanel/Lucky Seven/LuckySevenText").GetComponentInChildren<TextMeshProUGUI>();
        luckySevenLevel = GameObject.Find("Skill/SkillPanel/Lucky Seven/LuckySevenLevel").GetComponentInChildren<TextMeshProUGUI>();
        heistText = GameObject.Find("Skill/SkillPanel/Heist/HeistText").GetComponentInChildren<TextMeshProUGUI>();
        heistLevel = GameObject.Find("Skill/SkillPanel/Heist/HeistLevel").GetComponentInChildren<TextMeshProUGUI>();
        flashJumpText = GameObject.Find("Skill/SkillPanel/FlashJump/FlashJumpText").GetComponentInChildren<TextMeshProUGUI>();
        flashJumpLevel = GameObject.Find("Skill/SkillPanel/FlashJump/FlashJumpLevel").GetComponentInChildren<TextMeshProUGUI>();
        windBoosterText = GameObject.Find("Skill/SkillPanel/Wind Booster/WindBoosterText").GetComponentInChildren<TextMeshProUGUI>();
        windBoosterLevel = GameObject.Find("Skill/SkillPanel/Wind Booster/WindBoosterLevel").GetComponentInChildren<TextMeshProUGUI>();
        criticalShotText = GameObject.Find("Skill/SkillPanel/Critical Shot/CriticalShotText").GetComponentInChildren<TextMeshProUGUI>();
        criticalShotLevel = GameObject.Find("Skill/SkillPanel/Critical Shot/CriticalShotLevel").GetComponentInChildren<TextMeshProUGUI>();


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
    private void OnWindBoosterButtonClick()
    {
        skillCollection.skillPoint--;
        skillCollection.skills[3].skillLevel++;
        itemChanged = true;
    }
    private void OnCriticalShotButtonClick()
    {
        skillCollection.skillPoint--;
        skillCollection.skills[4].skillLevel++;
        DataManager.instance.SaveCriticalProbability(playerOriginCriticalProbability + skillCollection.skills[4].levelEffects[skillCollection.skills[4].skillLevel].criticalChanceIncrease);
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
        if (windBoosterText != null && skillCollection.skills.Count > 0
            && windBoosterLevel != null)
        {
            windBoosterText.text = skillCollection.skills[3].skillName;
            windBoosterLevel.text = skillCollection.skills[3].skillLevel.ToString();
        }
        if (criticalShotText != null && skillCollection.skills.Count > 0
            && criticalShotLevel != null)
        {
            criticalShotText.text = skillCollection.skills[4].skillName;
            criticalShotLevel.text = skillCollection.skills[4].skillLevel.ToString();
        }

        if (skillCollection.skillPoint <= 0)
        {
            luckySevenBtn.interactable = false;
            luckySevenBtn.image.sprite = deactivityBtn;
            heistBtn.interactable = false;
            heistBtn.image.sprite = deactivityBtn;
            flashJumpBtn.interactable = false;
            flashJumpBtn.image.sprite = deactivityBtn;
            windBoosterBtn.interactable = false;
            windBoosterBtn.image.sprite = deactivityBtn;
            criticalShotBtn.interactable = false;
            criticalShotBtn.image.sprite = deactivityBtn;
        }
        else
        {
            if (skillCollection.skills[0].skillLevel < 15) 
            {
                luckySevenBtn.interactable = true;
                luckySevenBtn.image.sprite = activityBtn;
            }
            else
            {
                luckySevenBtn.interactable = false;
                luckySevenBtn.image.sprite = deactivityBtn;
            }

            if (skillCollection.skills[1].skillLevel < 10)
            {
                heistBtn.interactable = true;
                heistBtn.image.sprite = activityBtn;
            }
            else
            {
                heistBtn.interactable = false;
                heistBtn.image.sprite = deactivityBtn;
            }

            if (skillCollection.skills[2].skillLevel < 1)
            {
                flashJumpBtn.interactable = true;
                flashJumpBtn.image.sprite = activityBtn;
            }
            else
            {
                flashJumpBtn.interactable = false;
                flashJumpBtn.image.sprite = deactivityBtn;
            }
            if (skillCollection.skills[3].skillLevel < 1)
            {
                windBoosterBtn.interactable = true;
                windBoosterBtn.image.sprite = activityBtn;
            }
            else
            {
                windBoosterBtn.interactable = false;
                windBoosterBtn.image.sprite = deactivityBtn;
            }
            if (skillCollection.skills[4].skillLevel < 20)
            {
                criticalShotBtn.interactable = true;
                criticalShotBtn.image.sprite = activityBtn;
            }
            else
            {
                criticalShotBtn.interactable = false;
                criticalShotBtn.image.sprite = deactivityBtn;
            }
        }
    }


    public void SaveSkillData(SkillCollection skillCollection)
    {
        string json = JsonConvert.SerializeObject(skillCollection, Formatting.Indented);
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
