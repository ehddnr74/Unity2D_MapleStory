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

    // ������ ���� �÷���
    public bool itemsChanged = false;
    private Shop shop;

    #region �̱���
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
        // ������ �Ŵ����� ���� �÷��̾� ������ �ε�
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
                    itemDT.amount = 1; // ���ο� �������� ������ 1�� ����
                    itemDT.slot = i; // �߰��� �������� ���� �ε����� ����
                    itemDT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemDT.amount.ToString();
                    itemDT.transform.SetParent(slots[i].transform, false);
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Icon;
                    itemObj.name = itemToAdd.Name;
                    itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // ���� �߾ӿ� ��ġ
                    break;
                }
            }
        }
        itemsChanged = true; // �������� �߰��Ǿ��� �� �÷��� ����
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
                    // ���� ������ �������� ��� ������ ���ҽ�Ŵ
                    data.amount--;
                    data.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.amount.ToString();
                }
                else
                {
                    // ������ 1�� ��� �������� ����
                    items[i] = new Item();
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }

                itemsChanged = true; // �������� ���ŵǾ��� �� �÷��� ����
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
            // �κ��丮 ������ ������� ���Կ� ������ ��ġ
            foreach (InventoryItem inventoryItem in inventoryItems)
            {
                Item item = itemdataBase.FetchItemByID(inventoryItem.ID);
                items[inventoryItem.slotnum] = item;

                // ���Կ� ������ ��ġ
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

        // �������� ����� ��� �κ��丮�� ����
        if (itemsChanged)
        {
            SaveInventory();
            if (shop.visibleShop == true)
            {
                shop.UpdateShopInventory();
            }
            itemsChanged = false; // �÷��� �ʱ�ȭ
        }
    }
}
