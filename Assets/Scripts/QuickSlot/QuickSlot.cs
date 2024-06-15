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
using TMPro;

public class QuickSlot : MonoBehaviour
{
    private static QuickSlot instance;

    private SkillManager skillManager;
    private StatManager statManager;
    private Player player;

    int slotAmount;
    GameObject quickSlotPanel;
    private Inventory inv;
    ItemDataBase itemdataBase;

    public GameObject quickSlot;
    public GameObject quickSlotItem;
    public bool itemsChanged = false;

    public bool ExistSuriken; // 표창이 인벤토리 내에 없는경우 표창을 발사 못하게 하기 위한 플래그 

    public List<InventoryItem> iti = new List<InventoryItem>();
    public List<GameObject> slots = new List<GameObject>();

    // 슬롯과 키 매핑
    private Dictionary<KeyCode, int> keyToSlotMap = new Dictionary<KeyCode, int>();

    public float playerOriginMoveSpeed;
    public float playerOriginJumpForce;
    public float playerOriginAttackSpeed;

    private bool playerJumping; //더블점프를 위함

    public BuffManager buffManager;

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
        skillManager = FindObjectOfType<SkillManager>();
        statManager = FindObjectOfType<StatManager>();
        player = FindObjectOfType<Player>();

        buffManager = FindObjectOfType<BuffManager>(); // BuffManager 인스턴스 찾기

        playerOriginMoveSpeed = player.moveSpeed;
        playerOriginJumpForce = player.jumpForce;
        playerOriginAttackSpeed = player.attackCoolDown;

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


            //Amount가 0이라면 텍스트 투명처리
            if(slotItem.GetComponent<QuickSlotDT>().itemAmount <=0)
            {
                TextMeshProUGUI amountText = slotItem.GetComponent<QuickSlotDT>().GetComponentInChildren<TextMeshProUGUI>();
                if(amountText != null)
                    amountText.text = string.Empty;
            }

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
                quickSlotItems.Add(new QuickSlotItem(quickSlotDT.slotNum, quickSlotDT.iconPath, quickSlotDT.itemAmount));
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
                    AddItemToQuickSlot(icon, item.slotNum, item.itemAmount);
                }
            }
        }
    }

    //인벤토리 아이템(ItemDT)로 부터 퀵슬롯에 Icon이 옮겨지는 과정 
    // 추후 icon이 표창일 경우 return 필요
    public void AddItemToQuickSlot(Sprite icon, int quickSlotID, int amount)
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
                        quickSlotDT.itemAmount = amount;

                        if(quickSlotDT.itemAmount > 0)
                        quickSlotDT.GetComponentInChildren<TextMeshProUGUI>().text = quickSlotDT.itemAmount.ToString();
                        // 아이템 추가가 완료된 후 데이터 저장
                        itemsChanged = true;
                    }
                }
            }
        }
    }

    public void RemoveQuicktSlotItem(string iconPath, int slotIndex, int amount)
    {
       QuickSlotDT slotDT =  slots[slotIndex].GetComponentInChildren<QuickSlotDT>();

        slotDT.itemAmount -= amount;
        slotDT.GetComponentInChildren<TextMeshProUGUI>().text = slotDT.itemAmount.ToString();

        if(slotDT.itemAmount <= 0)
        {
            // 마우스 오른쪽 클릭 시 아이템 아이콘을 지우고, 해당 슬롯의 정보를 초기화
            slotDT.itemIcon = null;
            slotDT.GetComponent<Image>().sprite = null;
            slotDT.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // 투명하게 설정
            slotDT.itemAmount = 0;
            slotDT.GetComponentInChildren<TextMeshProUGUI>().text = String.Empty;
        }

        itemsChanged = true;
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
                // 아이콘 교환
                Sprite tempIcon = slot1.itemIcon;
                slot1.itemIcon = slot2.itemIcon;
                slot2.itemIcon = tempIcon;

                // 아이콘 경로 교환
                string tempIconPath = slot1.iconPath;
                slot1.iconPath = slot2.iconPath;
                slot2.iconPath = tempIconPath;

                // 아이템 수량 교환
                int tempAmount = slot1.itemAmount;
                slot1.itemAmount = slot2.itemAmount;
                slot2.itemAmount = tempAmount;

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

                TextMeshProUGUI slot1Text = slot1.GetComponentInChildren<TextMeshProUGUI>();
                TextMeshProUGUI slot2Text = slot2.GetComponentInChildren<TextMeshProUGUI>();

                if (slot1Text != null)
                {
                    slot1Text.text = slot1.itemAmount > 0 ? slot1.itemAmount.ToString() : string.Empty;
                }

                if (slot2Text != null)
                {
                    slot2Text.text = slot2.itemAmount > 0 ? slot2.itemAmount.ToString() : string.Empty;
                }
            }
        }
    }

    private void InitializeKeyMappings()
    {
        KeyCode[] keys = { KeyCode.LeftAlt, KeyCode.LeftShift, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J,
                           KeyCode.K, KeyCode.L, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.LeftControl, KeyCode.Z, KeyCode.X,KeyCode.C,
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
                if (quickSlotDT.itemAmount > 0)
                {
                    Item redPotion = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                    DataManager.instance.AddHP(50);
                    inv.RemoveItem(redPotion.ID);
                    RemoveQuicktSlotItem(quickSlotDT.iconPath, slotIndex, 1);
                }

            }

            if (quickSlotDT.iconPath == "Orange Potion")
            {
                if (quickSlotDT.itemAmount > 0)
                {
                    Item orangePotion = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                    DataManager.instance.AddHP(150);
                    inv.RemoveItem(orangePotion.ID);
                    RemoveQuicktSlotItem(quickSlotDT.iconPath, slotIndex, 1);
                }
            }

            if (quickSlotDT.iconPath == "White Potion")
            {
                if (quickSlotDT.itemAmount > 0)
                {
                    Item whitePotion = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                    DataManager.instance.AddHP(300);
                    inv.RemoveItem(whitePotion.ID);
                    RemoveQuicktSlotItem(quickSlotDT.iconPath, slotIndex, 1);
                }
            }

            if (quickSlotDT.iconPath == "Mana Elixir")
            {
                if (quickSlotDT.itemAmount > 0)
                {
                    Item manaElixir = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                    DataManager.instance.AddMP(300);
                    inv.RemoveItem(manaElixir.ID);
                    RemoveQuicktSlotItem(quickSlotDT.iconPath, slotIndex, 1);
                }
            }

            if (quickSlotDT.iconPath == "Elixir")
            {
                if (quickSlotDT.itemAmount > 0)
                {
                    Item elixir = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                    DataManager.instance.UseElixer();
                    inv.RemoveItem(elixir.ID);
                    RemoveQuicktSlotItem(quickSlotDT.iconPath, slotIndex, 1);
                }
            }

            if (quickSlotDT.iconPath == "Power Elixir")
            {
                if (quickSlotDT.itemAmount > 0)
                {
                    Item powerElixir = itemdataBase.FetchItemByIconPath(quickSlotDT.iconPath);
                    DataManager.instance.UsePowerElixer();
                    inv.RemoveItem(powerElixir.ID);
                    RemoveQuicktSlotItem(quickSlotDT.iconPath, slotIndex, 1);
                }
            }

            // 스킬 사용 로직 
            if (quickSlotDT.iconPath == "LuckySeven" && player.canAttack)
            {
                int level = skillManager.skillCollection.skills[0].skillLevel;

                if (level > 0 && DataManager.instance.GetMP() >= skillManager.skillCollection.skills[0].levelEffects[level].mpReduction)
                {
                    DataManager.instance.RemoveMP(skillManager.skillCollection.skills[0].levelEffects[level].mpReduction);

                    if (!player.doubleJumping)
                    {
                        player.toAttack = true;
                        player.luckySeven = true;
                    }

                    if (ExistSuriken)
                    {
                        inv.UpdatesurikenAmountText(2);// 표창 개수 감소 (인자 = 감소시킬 수량)   
                    }
                }
            }

            if (quickSlotDT.iconPath == "Heist")
            {
                int level = skillManager.skillCollection.skills[1].skillLevel;

                if (level > 0)
                {
                    player.moveSpeed = playerOriginMoveSpeed + skillManager.skillCollection.skills[1].levelEffects[level].speedIncrease;
                    player.jumpForce = playerOriginJumpForce + skillManager.skillCollection.skills[1].levelEffects[level].jumpDistanceIncrease;

                    // 버프 활성화
                    buffManager.ActivateBuff("Heist", quickSlotDT.itemIcon, skillManager.skillCollection.skills[1].levelEffects[level].duration);
                }
            }

            if (quickSlotDT.iconPath == "FlashJump" && !player.isGround) 
            {
                int level = skillManager.skillCollection.skills[2].skillLevel;
                if (level > 0 && DataManager.instance.GetMP() >= skillManager.skillCollection.skills[2].levelEffects[level].mpReduction)
                {
                    DataManager.instance.RemoveMP(skillManager.skillCollection.skills[2].levelEffects[level].mpReduction);
                    player.canDoubleJump = true;
                }
            }

            if (quickSlotDT.iconPath == "WindBooster")
            {
                int level = skillManager.skillCollection.skills[3].skillLevel;

                if (level > 0 && DataManager.instance.GetMP() >= skillManager.skillCollection.skills[3].levelEffects[level].mpReduction
                    && level > 0 && DataManager.instance.GetHP() >= skillManager.skillCollection.skills[3].levelEffects[level].mpReduction)
                {
                    DataManager.instance.RemoveHP(skillManager.skillCollection.skills[3].levelEffects[level].mpReduction);
                    DataManager.instance.RemoveMP(skillManager.skillCollection.skills[3].levelEffects[level].mpReduction);
                    player.attackCoolDown = playerOriginAttackSpeed / skillManager.skillCollection.skills[3].levelEffects[level].attackSpeedIncrease;

                    // 버프 활성화
                    buffManager.ActivateBuff("WindBooster", quickSlotDT.itemIcon, skillManager.skillCollection.skills[3].levelEffects[level].duration);
                }
            }

            // 퀵슬롯 인벤토리,스탯창 등 Icon 사용 로직
            if (quickSlotDT.iconPath == "Key.Item")
            {
                inv.activeInventory = !inv.activeInventory;
                inv.inventoryPanel.SetActive(inv.activeInventory);
            }
            if (quickSlotDT.iconPath == "Key.Stat")
            {
                statManager.activeUI = !statManager.activeUI;
                statManager.StatUIPanel.SetActive(statManager.activeUI);
            }
            if (quickSlotDT.iconPath == "Key.Skill")
            {
                skillManager.activeUI = !skillManager.activeUI;
                skillManager.SkillUIPanel.SetActive(skillManager.activeUI);
            }
            if (!player.isAttacking && quickSlotDT.iconPath == "Key.Jump" && player.isGround)
            {
                playerJumping = true;
                player.toJump = true;
            }
            if(playerJumping && !player.isGround && quickSlotDT.iconPath == "Key.Jump")
            {
                int level = skillManager.skillCollection.skills[2].skillLevel;
                if (level > 0 && DataManager.instance.GetMP() >= skillManager.skillCollection.skills[2].levelEffects[level].mpReduction)
                {
                    DataManager.instance.RemoveMP(skillManager.skillCollection.skills[2].levelEffects[level].mpReduction);

                    player.doubleJumping = true;
                    player.canDoubleJump = true;
                    playerJumping = false;
                }
            }

            if (quickSlotDT.iconPath == "Key.Attack" && player.canAttack )
            { 
                if(!player.doubleJumping)
                player.toAttack = true;

                if(ExistSuriken)
                inv.UpdatesurikenAmountText(1); //표창 개수 1개 감소 
            }
        }
    }
}




