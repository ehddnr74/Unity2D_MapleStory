using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatManager : MonoBehaviour
{
    private PlayerData playerData; // 캐릭터 이름 가져오기 
    private StatData statData;


    public Slider hpBar;
    public Slider mpBar;
    public TextMeshProUGUI hpBarText;
    public TextMeshProUGUI mpBarText;

    private Button strBtn; 
    private Button dexBtn;
    private Button intBtn;
    private Button lukBtn;

    private Sprite activityBtn;
    private Sprite deactivityBtn;

    private TextMeshProUGUI nameText;
    private TextMeshProUGUI hpText;
    private TextMeshProUGUI mpText;
    private TextMeshProUGUI attackPowerText;
    private TextMeshProUGUI strText;
    private TextMeshProUGUI dexText;
    private TextMeshProUGUI intText;
    private TextMeshProUGUI lukText;
    
    private TextMeshProUGUI abilityPointText;

    public GameObject StatUIPanel;
    public bool activeUI = false;

    private AudioSource audioSource;
    public AudioClip BtnClickSound;

    private void Start()
    {
        // 데이터 매니저를 통해 플레이어 데이터 로드
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadData();
            DataManager.instance.LoadStatData();
            playerData = DataManager.instance.nowPlayer;
            statData = DataManager.instance.playerStat;
            audioSource = gameObject.AddComponent<AudioSource>();


            Sprite activityButton = Resources.Load<Sprite>("StatButton/ActivityButton");
            activityBtn = activityButton;
            Sprite deactivityButton = Resources.Load<Sprite>("StatButton/DeActivityButton");
            deactivityBtn = deactivityButton;

            //hpBar = GameObject.Find("MainBar/Panel/HpBar").GetComponentInChildren<Slider>();
            //mpBar  = GameObject.Find("MainBar/Panel/MpBar").GetComponentInChildren<Slider>();

            strBtn = GameObject.Find("Stats/StatsPanel/Stat/Image/strbtn").GetComponentInChildren<Button>();
            dexBtn = GameObject.Find("Stats/StatsPanel/Stat/Image/dexbtn").GetComponentInChildren<Button>();
            intBtn = GameObject.Find("Stats/StatsPanel/Stat/Image/intbtn").GetComponentInChildren<Button>();
            lukBtn = GameObject.Find("Stats/StatsPanel/Stat/Image/lukbtn").GetComponentInChildren<Button>();

            strBtn.onClick.AddListener(OnStrengthButtonClick);
            dexBtn.onClick.AddListener(OnDexterityButtonClick);
            intBtn.onClick.AddListener(OnIntelligenceButtonClick);
            lukBtn.onClick.AddListener(OnLuckButtonClick);

            nameText = GameObject.Find("Stats/StatsPanel/Stat/Name Text").GetComponentInChildren<TextMeshProUGUI>();
            hpText = GameObject.Find("Stats/StatsPanel/Stat/HP Text").GetComponentInChildren<TextMeshProUGUI>();
            mpText = GameObject.Find("Stats/StatsPanel/Stat/MP Text").GetComponentInChildren<TextMeshProUGUI>();
            attackPowerText = GameObject.Find("Stats/StatsPanel/Stat/Attack Power Text").GetComponentInChildren<TextMeshProUGUI>();
            strText = GameObject.Find("Stats/StatsPanel/Stat/STRText").GetComponentInChildren<TextMeshProUGUI>();
            dexText = GameObject.Find("Stats/StatsPanel/Stat/DEXText").GetComponentInChildren<TextMeshProUGUI>();
            intText = GameObject.Find("Stats/StatsPanel/Stat/INTText").GetComponentInChildren<TextMeshProUGUI>();
            lukText = GameObject.Find("Stats/StatsPanel/Stat/LUKText").GetComponentInChildren<TextMeshProUGUI>();

            abilityPointText = GameObject.Find("Stats/StatsPanel/Stat/Image/Ability Point Text").GetComponentInChildren<TextMeshProUGUI>();

            UpdateStatUI(playerData, statData);

            StatUIPanel = GameObject.Find("StatsPanel");
            StatUIPanel.SetActive(activeUI);
        }
    }

    public void UpdateStatUI(PlayerData pd, StatData sd)
    {
        //DataManager에서 받아서 저장
        playerData = pd;
        statData = sd;

        statData.maxHp = pd.baseHPTable[pd.level];
        statData.maxMp = pd.baseMPTable[pd.level];

        if (hpBar != null)
            hpBar.value = (float)statData.hp / pd.baseHPTable[pd.level];

        if (mpBar != null)
            mpBar.value = (float)statData.mp / pd.baseMPTable[pd.level];
        
        if(hpBarText != null)
            hpBarText.text = $"{statData.hp} / {pd.baseHPTable[pd.level]}";

        if (mpBarText != null)
            mpBarText.text = $"{statData.mp} / {pd.baseMPTable[pd.level]}";

        //UI 띄우기 작업
        if (nameText != null)
            nameText.text = pd.name;

        if (hpText != null)
            hpText.text = $"{sd.hp} / {pd.baseHPTable[pd.level]}";

        if (mpText != null)
            mpText.text = $"{sd.mp} / {pd.baseMPTable[pd.level]}";

        if (attackPowerText != null)
        {
            sd.minAttackPower = (int)Math.Round(sd.luck * 2.3);
            sd.maxAttackPower = (int)Math.Round(sd.luck * 3.6);   
            attackPowerText.text = $"{sd.minAttackPower} ~ {sd.maxAttackPower}";
            DataManager.instance.SaveStat();
        }

        if (strText != null)
            strText.text = sd.strength.ToString();

        if (dexText != null)
            dexText.text = sd.dexterity.ToString();

        if (intText != null)
            intText.text = sd.intelligence.ToString();

        if (lukText != null)
            lukText.text = sd.luck.ToString();

        if (abilityPointText != null) 
            abilityPointText.text = sd.AbilityPoint.ToString(); 
        
        if(sd.AbilityPoint <=0)
        {
            strBtn.interactable = false;
            strBtn.image.sprite = deactivityBtn;
            dexBtn.interactable = false;
            dexBtn.image.sprite = deactivityBtn;
            intBtn.interactable = false;
            intBtn.image.sprite = deactivityBtn;
            lukBtn.interactable = false;
            lukBtn.image.sprite = deactivityBtn;
        }
        else
        {
            strBtn.interactable = true;
            strBtn.image.sprite = activityBtn;
            dexBtn.interactable = true;
            dexBtn.image.sprite = activityBtn;
            intBtn.interactable = true;
            intBtn.image.sprite = activityBtn;
            lukBtn.interactable = true;
            lukBtn.image.sprite = activityBtn;
        }
    }


    private void OnStrengthButtonClick()
    {
        PlaySound(BtnClickSound);
        audioSource.volume = 0.2f;
        DataManager.instance.playerStat.AbilityPoint--;
        DataManager.instance.playerStat.strength++;
        UpdateStatUI(playerData, DataManager.instance.playerStat);
        DataManager.instance.SaveStat();
    }

    private void OnDexterityButtonClick()
    {
        PlaySound(BtnClickSound);
        audioSource.volume = 0.2f;
        DataManager.instance.playerStat.AbilityPoint--;
        DataManager.instance.playerStat.dexterity++;
        UpdateStatUI(playerData, DataManager.instance.playerStat);
        DataManager.instance.SaveStat();
    }

    private void OnIntelligenceButtonClick()
    {
        PlaySound(BtnClickSound);
        audioSource.volume = 0.2f;
        DataManager.instance.playerStat.AbilityPoint--;
        DataManager.instance.playerStat.intelligence++;
        UpdateStatUI(playerData, DataManager.instance.playerStat);
        DataManager.instance.SaveStat();
    }

    private void OnLuckButtonClick()
    {
        PlaySound(BtnClickSound);
        audioSource.volume = 0.2f;
        DataManager.instance.playerStat.AbilityPoint--;
        DataManager.instance.playerStat.luck++;
        DataManager.instance.playerStat.minAttackPower = (int)Math.Round(DataManager.instance.playerStat.luck * 2.3);
        DataManager.instance.playerStat.maxAttackPower = (int)Math.Round(DataManager.instance.playerStat.luck * 3.6);
        UpdateStatUI(playerData, DataManager.instance.playerStat);
        DataManager.instance.SaveStat();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
