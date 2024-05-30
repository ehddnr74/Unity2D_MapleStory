using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Data;
using static Player;


public class DataManager : MonoBehaviour
{
    // 싱글톤
    public static DataManager instance;

    public PlayerData nowPlayer;
    public SkillData playerSkill;
    public StatData playerStat;
    public ItemData itemData; // ItemData 변수 추가

    private PlayerManager playerManager;
    private SkillManager skillManager;
    private StatManager statManager;

    string path;
    string filename = "save"; // 플레이어 데이터 파일명 
    string StatDataFilename = "Stat"; // 스탯 데이터 파일명
   // string SkillDataFilename = "Skill"; // 스킬 데이터 파일명
    string itemDataFilename = "Item"; // 아이템 데이터 파일명

    private void Awake()
    {
        #region 싱글톤
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        path = Application.persistentDataPath + "/";
    }

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        skillManager = FindObjectOfType<SkillManager>();
        statManager = FindObjectOfType<StatManager>();
        //SaveSkill();
        LoadData();
        LoadStatData();
        LoadItemData(); // 아이템 데이터 로드
    }

    public void SaveSkill()
    {
        string data = JsonUtility.ToJson(playerSkill, true);
        File.WriteAllText(path + "Skill" + ".json", data);
    }
    public void SaveStat()
    {
        string data = JsonUtility.ToJson(playerStat, true);
        File.WriteAllText(path + "Stat" + ".json", data);
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowPlayer, true);
        File.WriteAllText(path + filename + ".json", data);
    }

    public void LoadData()
    {
        string fullPath = path + filename + ".json";
        string levelExperiencePath = path + "ExperienceTable.json";

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            nowPlayer = JsonUtility.FromJson<PlayerData>(data);
        }
        else
        {
            Debug.LogWarning("Save file not found");
        }

        if (File.Exists(levelExperiencePath))
        {
            string levelExperienceJson = File.ReadAllText(levelExperiencePath);
            nowPlayer.experienceTable = JsonConvert.DeserializeObject<Dictionary<int, int>>(levelExperienceJson);

            //nowPlayer.experienceTable = JsonUtility.FromJson<Dictionary<int, int>>(levelExperienceJson);
        }
        else
        {
            Debug.LogWarning("Level experience data file not found");
        }
    }

    public void LoadStatData()
    {
        string StatDataPath = path + StatDataFilename + ".json";

        if (File.Exists(StatDataPath))
        {
            string StatdataJson = File.ReadAllText(StatDataPath);
            playerStat = JsonUtility.FromJson<StatData>(StatdataJson);
        }
    }

    public void LoadItemData()
    {
        string itemDataPath = path + itemDataFilename + ".json";

        if (File.Exists(itemDataPath))
        {
            string itemDataJson = File.ReadAllText(itemDataPath);
            itemData = JsonConvert.DeserializeObject<ItemData>(itemDataJson);
            Debug.Log("Item data loaded successfully");
        }
    }

    public void AddExperience(int amount)
    {
        nowPlayer.experience += amount;
        if (nowPlayer.experience >= nowPlayer.experienceTable[nowPlayer.level])
        {
            nowPlayer.level++;
            LevelUpToStat();
            playerManager.UpdateLevelUI(nowPlayer.level);
            nowPlayer.experience = 0; // 레벨업 후 경험치 초기화 (또는 남은 경험치 계산)
        }

        // 경험치바 업데이트
        FindObjectOfType<ExperienceBar>().UpdateExperienceBar(nowPlayer.level, nowPlayer.experience);

        SaveData();
    }

    public void LevelUpToStat()
    {
        playerStat.AbilityPoint += 5;
        playerStat.hp = playerStat.maxHp;
        playerStat.mp = playerStat.maxMp;
        //playerStat.maxHp += 50;
        //playerStat.hp = playerStat.maxHp;
        //playerStat.maxMp += 50;
        //playerStat.mp = playerStat.maxMp;

        statManager.UpdateStatUI(nowPlayer, playerStat);
        SaveStat();
    }

}
