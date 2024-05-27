using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;


public class DataManager : MonoBehaviour
{
    // �̱���
    public static DataManager instance;

    public PlayerData nowPlayer;
    public ItemData itemData; // ItemData ���� �߰�

    private PlayerManager playerManager;

    string path;
    string filename = "save";
    string itemDataFilename = "Item"; // ������ ������ ���ϸ�

    private void Awake()
    {
        #region �̱���
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

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        LoadData();
        LoadItemData(); // ������ ������ �ε�
    }

    public void SaveData()
    {
         string data = JsonUtility.ToJson(nowPlayer, true);
         File.WriteAllText(path + filename + ".json", data);
        //try
        //{
        //    string data = JsonConvert.SerializeObject(nowPlayer, Formatting.Indented);
        //    Debug.Log("Serialized data: " + data);
        //    string filePath = path + filename + ".json";
        //    File.WriteAllText(filePath, data);
        //    Debug.Log("Data saved to " + filePath);

        //}
        //catch (Exception e)
        //{
        //    Debug.LogError("Failed to save data: " + e.Message);
        //}
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
            playerManager.UpdateLevelUI(nowPlayer.level);
            nowPlayer.experience = 0; // ������ �� ����ġ �ʱ�ȭ (�Ǵ� ���� ����ġ ���)
        }

        // ����ġ�� ������Ʈ
        FindObjectOfType<ExperienceBar>().UpdateExperienceBar(nowPlayer.level,nowPlayer.experience);

        SaveData();
    }



    //    try
    //    {
    //        string fullPath = path + filename + ".json";
    //        string levelExperiencePath = path + "ExperienceTable.json";

    //        if (File.Exists(fullPath))
    //        {
    //            string data = File.ReadAllText(fullPath);
    //            nowPlayer = JsonConvert.DeserializeObject<PlayerData>(data);
    //            Debug.Log("Player data loaded from " + fullPath);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Save file not found at " + fullPath);
    //            nowPlayer = new PlayerData(); // ������ ���� ��� �⺻������ �ʱ�ȭ
    //        }

    //        if (File.Exists(levelExperiencePath))
    //        {
    //            string levelExperienceJson = File.ReadAllText(levelExperiencePath);
    //            nowPlayer.experienceTable = JsonConvert.DeserializeObject<Dictionary<int, int>>(levelExperienceJson);
    //            Debug.Log("Experience Table loaded successfully from " + levelExperiencePath);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Level experience data file not found at " + levelExperiencePath);
    //            nowPlayer.experienceTable = new Dictionary<int, int>(); // ������ ���� ��� �⺻������ �ʱ�ȭ
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Failed to load data: " + e.Message);
    //        nowPlayer = new PlayerData();
    //    }
    //}
}
