using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using static UnityEditor.Progress;
using System;

public class QuickSlot : MonoBehaviour
{
    private static QuickSlot instance;

    int slotAmount;
    GameObject quickSlotPanel;
    private Inventory inv;
    ItemDataBase itemdataBase;

    public GameObject quickSlot;
    public GameObject quickSlotItem;
    public bool itemsChanged = false;

    public List<GameObject> slots = new List<GameObject>();

    // 슬롯과 키 매핑
    private Dictionary<KeyCode, int> keyToSlotMap = new Dictionary<KeyCode, int>();


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
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        itemdataBase = inv.GetComponent<ItemDataBase>();
        slotAmount = 32;
        quickSlotPanel = GameObject.Find("QuickSlotPanel");
        for (int i = 0; i < slotAmount; i++)
        {
            slots.Add(Instantiate(quickSlot));
            slots[i].GetComponent<QuickSlotSlot>().SetID(i);
            slots[i].transform.SetParent(quickSlotPanel.transform, false);
            GameObject slotItem = Instantiate(quickSlotItem);
            slotItem.transform.SetParent(slots[i].transform, false);
            slotItem.GetComponent<QuickSlotDT>().slotNum = i;

            //아이템 이미지 투명하게 설정
            Image slotItemImage = slotItem.GetComponent<Image>();
            if(slotItemImage != null)
            {
                Color tempColor = slotItemImage.color;
                tempColor.a = 0f; // 투명하게 설정
                slotItemImage.color = tempColor;
            }

        }

        // 키 매핑 초기화
        InitializeKeyMappings();

        LoadQuickSlot();
    }
    private void Update()
    {
        // 매핑된 키를 검사하여 해당 슬롯을 활성화
        foreach (var kvp in keyToSlotMap)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                ActivateSlot(kvp.Value);
            }
        }
        if(itemsChanged)
        {
            itemsChanged = false;
            SaveQuickSlot();
        }
    }

    public void SaveQuickSlot()
    {
        List<QuickSlotItem> quickSlotItems = new List<QuickSlotItem>();

        foreach (var slot in slots)
        {
            QuickSlotDT quickSlotDT = slot.GetComponentInChildren<QuickSlotDT>();

            if (quickSlotDT != null && quickSlotDT.itemIcon != null)
            {
                quickSlotItems.Add(new QuickSlotItem(quickSlotDT.slotNum, quickSlotDT.iconPath));
            }
        }

        string quickSlotDataJson = JsonConvert.SerializeObject(quickSlotItems, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/QuickSlot.json", quickSlotDataJson);
    }

    public void LoadQuickSlot()
    {
        string quickSlotDataPath = Application.persistentDataPath + "/QuickSlot.json";
        if (File.Exists(quickSlotDataPath))
        {
            string quickSlotDataJson = File.ReadAllText(quickSlotDataPath);
            List<QuickSlotItem> quickSlotItem = JsonConvert.DeserializeObject<List<QuickSlotItem>>(quickSlotDataJson);

            // 인벤토리 정보를 기반으로 슬롯에 아이템 배치
            foreach (QuickSlotItem item in quickSlotItem)
            {
                if (!string.IsNullOrEmpty(item.iconPath))
                {
                    Sprite icon = Resources.Load<Sprite>("QuickSlotIcons/" + item.iconPath);
                    AddItemToQuickSlot(icon, item.slotNum);
                }
            }
        }
    }

    //인벤토리 아이템(ItemDT)로 부터 퀵슬롯에 Icon이 옮겨지는 과정 
    // 추후 icon이 표창일 경우 return 필요
    public void AddItemToQuickSlot(Sprite icon, int quickSlotID)
    {
        foreach(var slot in slots)
        {
            QuickSlotDT quickSlotDT = slot.GetComponentInChildren<QuickSlotDT>();
            if (quickSlotDT != null)
            {
                Image slotImage = quickSlotDT.GetComponent<Image>();
                if (slotImage != null && slotImage.sprite != null && slotImage.sprite.name == icon.name)
                {
                    // 이미 해당 아이콘이 있는 경우 리턴
                    return;
                }
            }
        }
        if (quickSlotID >= 0 && quickSlotID < slots.Count)
        { 
            QuickSlotSlot quickSlotSlot = slots[quickSlotID].GetComponent<QuickSlotSlot>();
            if (quickSlotSlot != null)
            {
                QuickSlotDT quickSlotDT = quickSlotSlot.GetComponentInChildren<QuickSlotDT>();
                
                if (quickSlotDT != null)
                {
                    Image slotImage = quickSlotDT.GetComponent<Image>();
                    if (slotImage != null)
                    {
                        slotImage.sprite = icon;
                        Color tempColor = slotImage.color;
                        tempColor.a = 1f; // 불투명하게 설정
                        slotImage.color = tempColor;
                        quickSlotDT.itemIcon = icon;
                        quickSlotDT.iconPath = icon.name; // 아이콘의 경로를 저장

                        // 아이템 추가가 완료된 후 데이터 저장
                        itemsChanged = true;
                    }
                }
            }
        }
    }

    // 두 퀵슬롯 간의 아이템 정보를 교환하는 메서드
    public void SwapItems(int slotNum1, int slotNum2)
    {
        if (slotNum1 >= 0 && slotNum1 < slots.Count && slotNum2 >= 0 && slotNum2 < slots.Count)
        {
            QuickSlotDT slot1 = slots[slotNum1].GetComponentInChildren<QuickSlotDT>();
            QuickSlotDT slot2 = slots[slotNum2].GetComponentInChildren<QuickSlotDT>();

            if (slot1 != null && slot2 != null)
            {
                Sprite tempIcon = slot1.itemIcon;
                slot1.itemIcon = slot2.itemIcon;
                slot2.itemIcon = tempIcon;

                // 아이콘 경로 교환
                string tempIconPath = slot1.iconPath;
                slot1.iconPath = slot2.iconPath;
                slot2.iconPath = tempIconPath;

                Image slot1Image = slot1.GetComponent<Image>();
                Image slot2Image = slot2.GetComponent<Image>();

                if (slot1Image != null && slot2Image != null)
                {
                    slot1Image.sprite = slot1.itemIcon;
                    slot2Image.sprite = slot2.itemIcon;

                    Color tempColor1 = slot1Image.color;
                    tempColor1.a = slot1.itemIcon != null ? 1f : 0f;
                    slot1Image.color = tempColor1;

                    Color tempColor2 = slot2Image.color;
                    tempColor2.a = slot2.itemIcon != null ? 1f : 0f;
                    slot2Image.color = tempColor2;            
                }
            }
        }
    }

    private void InitializeKeyMappings()
    {
        KeyCode[] keys = { KeyCode.LeftAlt, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
                           KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.LeftControl, KeyCode.Z, KeyCode.X,KeyCode.C,
                           KeyCode.V, KeyCode.B, KeyCode.N,KeyCode.M,KeyCode.Y,KeyCode.U,KeyCode.I,KeyCode.O,KeyCode.P,KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };

        for (int i = 0; i < slots.Count && i < keys.Length; i++)
        {
            keyToSlotMap[keys[i]] = i;
        }
    }

    // 슬롯 활성화
    private void ActivateSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            QuickSlotDT quickSlotDT = slots[slotIndex].GetComponentInChildren<QuickSlotDT>();


            // 아이템 사용 로직 
            if(quickSlotDT.iconPath == "Red Potion")
            {
                Item redPotion = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                DataManager.instance.AddHP(50);
                inv.RemoveItem(redPotion.ID);

                Debug.Log("Use Red Potion");

            }

            if (quickSlotDT.iconPath == "Orange Potion")
            {
                Item orangePotion = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                DataManager.instance.AddHP(150);
                inv.RemoveItem(orangePotion.ID);

                Debug.Log("Use Orange Potion");
            }

            if (quickSlotDT.iconPath == "White Potion")
            {
                Item whitePotion = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                DataManager.instance.AddHP(300);
                inv.RemoveItem(whitePotion.ID);

                Debug.Log("Use White Potion Potion");
            }

            if (quickSlotDT.iconPath == "Mana Elixir")
            {
                Item manaElixir = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                DataManager.instance.AddMP(300);
                inv.RemoveItem(manaElixir.ID);

                Debug.Log("Use Mana Elixir Potion");
            }

            if (quickSlotDT.iconPath == "Elixir")
            {
                Item elixir = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                DataManager.instance.UseElixer();
                inv.RemoveItem(elixir.ID);

                Debug.Log("Use Elixir");
            }

            if (quickSlotDT.iconPath == "Power Elixir")
            {
                Item powerElixir = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                DataManager.instance.UsePowerElixer();
                inv.RemoveItem(powerElixir.ID);

                Debug.Log("Use Power Elixir");
            }

            // 스킬 사용 로직 
            if (quickSlotDT.iconPath == "LuckySeven")
            {
                Debug.Log("Use LuckySeven");
            }

            if (quickSlotDT.iconPath == "Heist")
            {
                Debug.Log("Use Heist");
            }
        }
    }
}




