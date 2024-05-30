using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEditor.Experimental.GraphView;
using Unity.Burst.CompilerServices;

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

    GameObject SkillUIPanel;
    bool activeUI = false;

    private void Start()
    {
        path = Application.persistentDataPath + "/";

        // 스킬 데이터 로드
        skillCollection = LoadSkillData();

        if (skillCollection == null)
        {
            skillCollection = new SkillCollection();
        }

        // 예시 스킬 추가
        //AddSkill("럭키 세븐", 1);
        //AddSkill("헤이스트", 1);


        skillPointText = GameObject.Find("Skill/SkillPanel/Skill Point Text").GetComponentInChildren<TextMeshProUGUI>();
        luckySevenText = GameObject.Find("Skill/SkillPanel/Lucky Seven/LuckySevenText").GetComponentInChildren<TextMeshProUGUI>();
        luckySevenLevel = GameObject.Find("Skill/SkillPanel/Lucky Seven/LuckySevenLevel").GetComponentInChildren<TextMeshProUGUI>();
        heistText = GameObject.Find("Skill/SkillPanel/Heist/HeistText").GetComponentInChildren<TextMeshProUGUI>();
        heistLevel = GameObject.Find("Skill/SkillPanel/Heist/HeistLevel").GetComponentInChildren<TextMeshProUGUI>();

        UpdateSkillUI();

        // 스킬 데이터 저장
        SaveSkillData(skillCollection);
        SkillUIPanel = GameObject.Find("SkillPanel");
        SkillUIPanel.SetActive(activeUI);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            activeUI = !activeUI;
            SkillUIPanel.SetActive(activeUI);
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
