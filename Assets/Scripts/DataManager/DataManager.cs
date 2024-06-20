using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Data;
using static Player;
using Unity.VisualScripting;


public class DataManager : MonoBehaviour
{
    // 싱글톤
    public static DataManager instance;

    public PlayerData nowPlayer;
    public SkillCollection playerSkill;
    public StatData playerStat;
    public ItemData itemData; // ItemData 변수 추가

    private Player player;
    private Shop shop;
    private Inventory inv;
    private PlayerManager playerManager;
    private SkillManager skillManager;
    private StatManager statManager;
    private SkillEffectManager skillEffectManager;

    private AudioSource audioSource;
    public AudioClip levelUpSound;


    string path;
    string filename = "save"; // 플레이어 데이터 파일명 
    string StatDataFilename = "Stat"; // 스탯 데이터 파일명
   // string SkillDataFilename = "Skill"; // 스킬 데이터 파일명
    string itemDataFilename = "Item"; // 아이템 데이터 파일명

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

        path = Application.persistentDataPath + "/";
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        playerManager = FindObjectOfType<PlayerManager>();
        skillManager = FindObjectOfType<SkillManager>();
        statManager = FindObjectOfType<StatManager>();
        skillEffectManager = GameObject.Find("EffectPoolManager").GetComponent<SkillEffectManager>();
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        audioSource = gameObject.AddComponent<AudioSource>();
        //SaveSkill();
        LoadData();
        LoadStatData();
        LoadItemData(); // 아이템 데이터 로드
    }

    //public void SaveSkill()
    //{
    //    string data = JsonUtility.ToJson(playerSkill, true);
    //    File.WriteAllText(path + "Skill" + ".json", data);
    //}
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
        string HpLevelPath = path + "BaseHpTable.json";
        string MpLevelPath = path + "BaseMpTable.json";
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
        }
        else
        {
            Debug.LogWarning("Level experience data file not found");
        }

        if (File.Exists(HpLevelPath))
        {
            string HpLevelJson = File.ReadAllText(HpLevelPath);
            nowPlayer.baseHPTable = JsonConvert.DeserializeObject<Dictionary<int, int>>(HpLevelJson);
        }
        else
        {
            Debug.LogWarning("Level baseHPTable data file not found");
        }

        if (File.Exists(MpLevelPath))
        {
            string MpLevelJson = File.ReadAllText(MpLevelPath);
            nowPlayer.baseMPTable = JsonConvert.DeserializeObject<Dictionary<int, int>>(MpLevelJson);
        }
        else
        {
            Debug.LogWarning("Level baseHPTable data file not found");
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

    public void SaveCriticalProbability(float criticalProbability)
    {
        nowPlayer.criticalProbability = criticalProbability;
        SaveData();
    }

    public void AddExperience(int amount)
    {
        nowPlayer.experience += amount;
        if (nowPlayer.experience >= nowPlayer.experienceTable[nowPlayer.level])
        {
            PlaySound(levelUpSound);
            audioSource.volume = 0.2f;
            // 스킬 이펙트 생성
            skillEffectManager.ShowEffect("LevelUpEffect", player.transform, new Vector3(0f, 1.5f, 0f), Quaternion.identity, new Vector3(1.3f, 1.3f, 0f), 1.5f);
            nowPlayer.level++;
            LevelUpToStat(); //레벨업 시 스탯 성장
            playerManager.UpdateLevelUI(nowPlayer.level);
            nowPlayer.experience = 0; // 레벨업 후 경험치 초기화 (또는 남은 경험치 계산)

            skillManager.skillCollection.skillPoint += 3;
            skillManager.itemChanged = true;
        }

        // 경험치바 업데이트
        FindObjectOfType<ExperienceBar>().UpdateExperienceBar(nowPlayer.level, nowPlayer.experience);

        SaveData();
    }

    public void AddHP(int hpamount)
    {
        playerStat.hp += hpamount;

        CheckStatus();

        statManager.UpdateStatUI(nowPlayer, playerStat);
        SaveStat();
    }
    public void RemoveHP(int hpamount)
    {
        playerStat.hp -= hpamount;

        CheckStatus();

        statManager.UpdateStatUI(nowPlayer, playerStat);
        SaveStat();
    }

    public void AddMP(int mpamount)
    {
        playerStat.mp += mpamount;

        CheckStatus();

        statManager.UpdateStatUI(nowPlayer, playerStat);
        SaveStat();
    }
    public void RemoveMP(int mpamount)
    {
        playerStat.mp -= mpamount;

        CheckStatus();

        statManager.UpdateStatUI(nowPlayer, playerStat);
        SaveStat();
    }
    public void UseElixer()
    {
        playerStat.hp += (playerStat.maxHp / 2);
        playerStat.mp += (playerStat.maxMp / 2);

        CheckStatus();

        statManager.UpdateStatUI(nowPlayer, playerStat);
        SaveStat();
    }

    public void UsePowerElixer()
    {
        playerStat.hp += playerStat.maxHp;
        playerStat.mp += playerStat.maxMp;

        CheckStatus();

        statManager.UpdateStatUI(nowPlayer, playerStat);
        SaveStat();
    }

    public void LevelUpToStat()
    {
        playerStat.AbilityPoint += 5;
        playerStat.hp = playerStat.maxHp;
        playerStat.mp = playerStat.maxMp;

        statManager.UpdateStatUI(nowPlayer, playerStat);
        SaveStat();
    }

    public void AddMeso(int meso)
    {
        nowPlayer.meso += meso;
        inv.UpdateMesoUI(nowPlayer);

        if(shop.visibleShop)
        shop.UpdateShopMesoText(nowPlayer);

        SaveData();
    }
    public void LoseMeso(int meso)
    {
        if(nowPlayer.meso - meso < 0)
        {
            return;
        }
        nowPlayer.meso -= meso;
        inv.UpdateMesoUI(nowPlayer);

        if(shop.visibleShop)
        shop.UpdateShopMesoText(nowPlayer);

        SaveData();
    }

    private void CheckStatus() // HP , MP 초과 or 미만 체크 
    {
        if (playerStat.hp >= playerStat.maxHp)
            playerStat.hp = playerStat.maxHp;

        if (playerStat.mp >= playerStat.maxMp)
            playerStat.mp = playerStat.maxMp;

        if (playerStat.hp <= 0)
            playerStat.hp = 0;

        if (playerStat.mp <= 0)
            playerStat.mp = 0;
    }

    public int GetHP()
    {
        return playerStat.hp;
    }
    public int GetMP()
    {
        return playerStat.mp;
    }

    private void Update()
    {

    }


    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
