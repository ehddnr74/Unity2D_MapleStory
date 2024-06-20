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
    // �̱���
    public static DataManager instance;

    public PlayerData nowPlayer;
    public SkillCollection playerSkill;
    public StatData playerStat;
    public ItemData itemData; // ItemData ���� �߰�

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
    string filename = "save"; // �÷��̾� ������ ���ϸ� 
    string StatDataFilename = "Stat"; // ���� ������ ���ϸ�
   // string SkillDataFilename = "Skill"; // ��ų ������ ���ϸ�
    string itemDataFilename = "Item"; // ������ ������ ���ϸ�

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
        LoadItemData(); // ������ ������ �ε�
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
            // ��ų ����Ʈ ����
            skillEffectManager.ShowEffect("LevelUpEffect", player.transform, new Vector3(0f, 1.5f, 0f), Quaternion.identity, new Vector3(1.3f, 1.3f, 0f), 1.5f);
            nowPlayer.level++;
            LevelUpToStat(); //������ �� ���� ����
            playerManager.UpdateLevelUI(nowPlayer.level);
            nowPlayer.experience = 0; // ������ �� ����ġ �ʱ�ȭ (�Ǵ� ���� ����ġ ���)

            skillManager.skillCollection.skillPoint += 3;
            skillManager.itemChanged = true;
        }

        // ����ġ�� ������Ʈ
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

    private void CheckStatus() // HP , MP �ʰ� or �̸� üũ 
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
