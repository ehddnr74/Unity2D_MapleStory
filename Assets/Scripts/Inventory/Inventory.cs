using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class Inventory : MonoBehaviour
{
    private Player player;
    private QuickSlot quickSlot;

    public GameObject inventoryPanel;
    GameObject Content;
    ItemDataBase itemdataBase;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    public GameObject firstSurikenEffect; // 인벤토리 가장 앞에 위치한 표창의 테두리 이펙트 프리펩
    public int currentSurikenSlot;

    private PlayerData playerData;

    private TextMeshProUGUI mesoText;

    public bool activeInventory = false;

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

        player = GameObject.Find("Player").GetComponent<Player>();
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();

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

                    if (items[i].Type == "suriken")
                    {
                        itemDT.amount = 1000;
                        itemDT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemDT.amount.ToString();
                    }
                    else
                    {
                        itemDT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemDT.amount.ToString();
                    }
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

                if (data.amount > 1 && items[i].ID != 6 && items[i].ID != 7) 
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

                if(items[i].ID == 6 && items[i].ID == 7)
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
        UpdateSurikenEffect();
    }

    public void UpdateMesoUI(PlayerData pd)
    {
        playerData = pd;

        if (mesoText != null)
        {
            mesoText.text = playerData.meso.ToString();
        }
    }

    public void UpdatesurikenAmountText(int amount) // 퀵슬롯의 공격 아이콘에 맵핑된 Key를 누를 시 호출
    {
        ItemDT itemDT = slots[currentSurikenSlot].GetComponentInChildren<ItemDT>();
        if (itemDT.amount > 2)
        {
            itemDT.amount -= amount;
            itemDT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemDT.amount.ToString();
        }
         else // UpdateSurikenEffect()함수를 실행시켜 currentSurikenSlot을 업데이트
        {
            int remainingAmount = amount - 1;
            itemDT.amount--;
            itemDT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemDT.amount.ToString();
            SaveInventory();
            UpdateSurikenEffect();
            ItemDT NewitemDT = slots[currentSurikenSlot].GetComponentInChildren<ItemDT>();
            NewitemDT.amount -= remainingAmount;
            NewitemDT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = NewitemDT.amount.ToString();
        }


        itemsChanged = true;
    }

    public void UpdateSurikenEffect() //  인벤토리의 표창들 중에 가장 우선적으로 위치한 표창의 슬롯을 찾아내 표창의 테두리에 애니메이션 효과
    {
        string inventoryDataPath = Application.persistentDataPath + "/Inventory.json";
        if (File.Exists(inventoryDataPath))
        {
            string inventoryDataJson = File.ReadAllText(inventoryDataPath);
            List<InventoryItem> inventoryItems = JsonConvert.DeserializeObject<List<InventoryItem>>(inventoryDataJson);

            foreach (InventoryItem invItems in inventoryItems)
            {
                Slot invSlot = slots[invItems.slotnum].GetComponent<Slot>();
                ItemDT invDT = invSlot.GetComponentInChildren<ItemDT>();
                Image img = invDT.transform.GetChild(1).GetComponent<Image>();

                if (img != null)
                {
                    Color tempColor = img.color;
                    tempColor.a = 0f;
                    img.color = tempColor;
                }
            }
            
            foreach (InventoryItem invItems in inventoryItems)
            {
                if (invItems.ID == 6 || invItems.ID == 7) //표창
                {
                    if(invItems.amount <= 1) // 표창 개수가 1이하면 다음 인벤토리의 표창으로 사용
                        continue;
                    
                    Slot invSlot = slots[invItems.slotnum].GetComponent<Slot>();
                    currentSurikenSlot = invItems.slotnum;
                    ItemDT invDT = invSlot.GetComponentInChildren<ItemDT>();
                    Image img = invDT.transform.GetChild(1).GetComponent<Image>();
                    if (img != null)
                    {
                        Color tempColor = img.color;
                        tempColor.a = 1f;
                        img.color = tempColor;
                    }
                    // 표창이 인벤토리에 있는 경우 flag를 줌
                    // 표창이 인벤토리에 있는 경우 flag를 줌

                    player.currentSuriken = invItems.ID;
                    quickSlot.ExistSuriken = true;
                    break;
                }
                quickSlot.ExistSuriken = false;
            }
        }
    }


    void Update()
    {
        // 아이템이 변경된 경우 인벤토리를 저장
        if (itemsChanged)
        {
            SaveInventory();
            UpdateSurikenEffect();
            if (shop.visibleShop == true)
            {
                shop.UpdateShopInventory();
            }
            itemsChanged = false; // 플래그 초기화
        }
    }
}
