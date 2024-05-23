using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;


public class DataManager : MonoBehaviour
{
    // 싱글톤
    public static DataManager instance;

    public PlayerData nowPlayer;
    private PlayerManager playerManager;

    string path;
    string filename = "save";

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

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        LoadData();
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

    public void AddExperience(int amount)
    {
        nowPlayer.experience += amount;
        if (nowPlayer.experience >= nowPlayer.experienceTable[nowPlayer.level])
        {
            nowPlayer.level++;
            playerManager.UpdateLevelUI();
            //UpdateLevelUI();
            nowPlayer.experience = 0; // 레벨업 후 경험치 초기화 (또는 남은 경험치 계산)
        }

        //// 경험치바 업데이트
        //FindObjectOfType<ExperienceBar>().UpdateExperienceBar();

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
    //            nowPlayer = new PlayerData(); // 파일이 없을 경우 기본값으로 초기화
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
    //            nowPlayer.experienceTable = new Dictionary<int, int>(); // 파일이 없을 경우 기본값으로 초기화
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Failed to load data: " + e.Message);
    //        nowPlayer = new PlayerData();
    //    }
    //}
}
