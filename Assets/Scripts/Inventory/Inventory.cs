using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using static UnityEditor.Progress;
using UnityEditor.U2D.Aseprite;
using Unity.VisualScripting;


public class Inventory : MonoBehaviour
{
    GameObject inventoryPanel;
    GameObject Content;
    ItemDataBase itemdataBase;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    private PlayerData playerData;

    private TextMeshProUGUI mesoText;

    bool activeInventory = false;

    int slotAmount;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    // 아이템 변경 플래그
    public bool itemsChanged = false;
    private Shop shop;

    #region 싱글톤
    public static Inventory instance;
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
    #endregion
    private void Start()
    {
        // 데이터 매니저를 통해 플레이어 데이터 로드
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadData();
            playerData = DataManager.instance.nowPlayer;
        }

        shop = GameObject.Find("Shop").GetComponent<Shop>();

        mesoText = GameObject.Find("InventoryUI/Inventory Panel/Slot Panel/MesoText").GetComponentInChildren<TextMeshProUGUI>();
        itemdataBase = GetComponent<ItemDataBase>();
        slotAmount = 24;
        inventoryPanel = GameObject.Find("Inventory Panel");
        inventoryPanel.SetActive(activeInventory);

        string contentPath = "Slot Panel/Scroll View/Viewport/Content";
        Transform contentTransform = inventoryPanel.transform.Find(contentPath);

        Content = contentTransform.gameObject;

        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(Content.transform, false);
        }
        UpdateMesoUI(playerData);
        LoadInventory();
    }

    public void AddItem(int id)
    {
        Item itemToAdd = itemdataBase.FetchItemByID(id);
        if (itemToAdd.Stackable && CheckIfItemIsInInventory(itemToAdd))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemDT data = slots[i].transform.GetChild(0).GetComponent<ItemDT>();
                    data.amount++;
                    data.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.amount.ToString();
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == -1)
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    ItemDT itemDT = itemObj.GetComponent<ItemDT>();
                    itemDT.item = itemToAdd;
                    itemDT.amount = 1; // 새로운 아이템의 개수는 1로 설정
                    itemDT.slot = i; // 추가된 아이템의 슬롯 인덱스를 설정
                    itemDT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemDT.amount.ToString();
                    itemDT.transform.SetParent(slots[i].transform, false);
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Icon;
                    itemObj.name = itemToAdd.Name;
                    itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // 슬롯 중앙에 배치
                    break;
                }
            }
        }
        itemsChanged = true; // 아이템이 추가되었을 때 플래그 설정
    }

    public void RemoveItem(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == id)
            {
                ItemDT data = slots[i].transform.GetChild(0).GetComponent<ItemDT>();

                if (data.amount > 1)
                {
                    // 스택 가능한 아이템의 경우 수량을 감소시킴
                    data.amount--;
                    data.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.amount.ToString();
                }
                else
                {
                    // 수량이 1인 경우 아이템을 제거
                    items[i] = new Item();
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }

                itemsChanged = true; // 아이템이 제거되었을 때 플래그 설정
                break;
            }
        }
    }

    bool CheckIfItemIsInInventory(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == item.ID)
                return true;
        }
        return false;
    }


    public void SaveInventory()
    {
        List<InventoryItem> inventoryItems = new List<InventoryItem>();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0 && items[i].ID != -1) 
            {
                ItemDT data = slots[i].transform.GetChild(0).GetComponent<ItemDT>();
                inventoryItems.Add(new InventoryItem(items[i].ID, data.amount, data.slot));
            }
        }

        string inventoryDataJson = JsonConvert.SerializeObject(inventoryItems, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/Inventory.json", inventoryDataJson);
    }

    public void LoadInventory()
    {
        string inventoryDataPath = Application.persistentDataPath + "/Inventory.json";
        if (File.Exists(inventoryDataPath))
        {
            string inventoryDataJson = File.ReadAllText(inventoryDataPath);
            List<InventoryItem> inventoryItems = JsonConvert.DeserializeObject<List<InventoryItem>>(inventoryDataJson);

            items.Clear();

            for (int i = 0; i < slotAmount; i++)
            {
                items.Add(new Item());
                slots[i].GetComponent<Slot>().ClearSlot();
            }
            // 인벤토리 정보를 기반으로 슬롯에 아이템 배치
            foreach (InventoryItem inventoryItem in inventoryItems)
            {
                Item item = itemdataBase.FetchItemByID(inventoryItem.ID);
                items[inventoryItem.slotnum] = item;

                // 슬롯에 아이템 배치
                Slot slot = slots[inventoryItem.slotnum].GetComponent<Slot>();
                slot.UpdateSlot(item, inventoryItem.amount);
            }
        }
    }

    public void UpdateMesoUI(PlayerData pd)
    {
        playerData = pd;

        if (mesoText != null)
        {
            mesoText.text = playerData.meso.ToString();
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }

        // 아이템이 변경된 경우 인벤토리를 저장
        if (itemsChanged)
        {
            SaveInventory();
            if (shop.visibleShop == true)
            {
                shop.UpdateShopInventory();
            }
            itemsChanged = false; // 플래그 초기화
        }
    }
}
