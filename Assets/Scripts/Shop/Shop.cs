using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using System.IO;

public class Shop : MonoBehaviour
{
    // 샵 UI 아이템 구성
    int slotAmount;

    public bool visibleShop = false; // 상점이 보여지고 있는가

    private PlayerData playerData;

    public GameObject shopitem;
    public GameObject selectBox;

    private Inventory inv;
    public int isSelectedBoxIndex = -1;

    public GameObject shopParentPanel;
    private GameObject shopPanel;
    private GameObject SlotPanel;
    public GameObject Slot;

    private QuickSlot quickSlot;
    private ItemDataBase itemdataBase;

    public List<GameObject> slots = new List<GameObject>();
    public List<GameObject> selectBoxes = new List<GameObject>();
    //

    // 샵 UI 인벤토리 내용물 구성
    int invslotAmount;
    public GameObject invShopitem;
    public GameObject invShopSelectBox;
    public GameObject invShopSlot;

    GameObject Content;

    public int isInvSelectedBoxIndex = -1;

    public List<GameObject> invShopslots = new List<GameObject>();
    public List<GameObject> invShopselectBoxes = new List<GameObject>();
    public List<GameObject> inventoryItems = new List<GameObject>();

    public List<InventoryItem> iti = new List<InventoryItem>();

    public List<Item> currentShopInv = new List<Item>();

    private AudioSource audioSource;
    public AudioClip BuySound;
    public AudioClip SellSound;
    public AudioClip TabSound;


    private void Start()
    {
        // 데이터 매니저를 통해 플레이어 데이터 로드
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadData();
            playerData = DataManager.instance.nowPlayer;
        }

        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
        itemdataBase = inv.GetComponent<ItemDataBase>();
        audioSource = gameObject.AddComponent<AudioSource>();

        slotAmount = 8;
        SlotPanel = GameObject.Find("SlotPanel");
        shopPanel = GameObject.Find("ShopPanel");
        shopParentPanel = GameObject.Find("ShopParentPanel");

        shopParentPanel.SetActive(visibleShop);

        for (int i = 0; i < slotAmount; i++) // 샵UI 아이템 구성 
        {
            slots.Add(Instantiate(Slot));
            slots[i].transform.SetParent(SlotPanel.transform, false);
            slots[i].GetComponent<ShopSlot>().slotID = i;

            GameObject item = Instantiate(shopitem);
            item.transform.SetParent(slots[i].transform, false);

            selectBoxes.Add(Instantiate(selectBox));
            selectBoxes[i].transform.SetParent(slots[i].transform, false);
            selectBoxes[i].GetComponent<ShopSlot>().slotID = i;

            item.GetComponent<Image>().sprite = itemdataBase.dataBase[i].Icon;

            //이거 버튼 아님 이미지임 지을 이름이 없었음 
            Image selectBtn = selectBoxes[i].GetComponent<Image>();
            if (selectBtn != null)
            {
                Color tempColor = selectBtn.color;
                tempColor.a = 0f; // 투명하게 설정
                selectBtn.color = tempColor;
                selectBtn.raycastTarget = false;
            }

            // 버튼 컴포넌트를 찾아 클릭 이벤트 등록
            Button selectButton = selectBoxes[i].GetComponentInChildren<Button>();
            if (selectButton != null)
            {
                selectButton.gameObject.GetComponent<Image>().raycastTarget = true;
                int index = i; // 캡처 문제 해결을 위해 지역 변수를 사용합니다.
                selectButton.onClick.AddListener(() => OnSelectButtonClicked(index));
            }

            // 텍스트 컴포넌트를 찾아서 설정
            TextMeshProUGUI itemNameText = selectBoxes[i].transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = selectBoxes[i].transform.Find("PriceText").GetComponent<TextMeshProUGUI>();

            Item currentItem = itemdataBase.dataBase[i];

            if (itemNameText != null)
            {
                itemNameText.gameObject.GetComponent<TextMeshProUGUI>().raycastTarget = false;
                itemNameText.text = currentItem.Name;
            }
            if (priceText != null)
            {
                priceText.gameObject.GetComponent<TextMeshProUGUI>().raycastTarget = false;
                priceText.text = $"{currentItem.Price} 메소";
            }
        }

        ///////////////////////////////// 
        ///  샵 UI에 인벤토리 보유물 구성
        invslotAmount = 24;

        string contentPath = "Scroll View/Viewport/Content";
        Transform contentTransform = shopPanel.transform.Find(contentPath);
        Content = contentTransform.gameObject; // Content의 자식으로 슬롯들 정렬하기 위해 위치 찾기

        for (int i = 0; i < invslotAmount; i++)
        {
            invShopslots.Add(Instantiate(invShopSlot));
            invShopslots[i].transform.SetParent(Content.transform, false);
            invShopslots[i].GetComponent<ShopSlot>().slotID = i;

            GameObject invitem = Instantiate(invShopitem);
            invitem.transform.SetParent(invShopslots[i].transform, false);
            inventoryItems.Add(invitem);

            invShopselectBoxes.Add(Instantiate(invShopSelectBox));
            invShopselectBoxes[i].transform.SetParent(invShopslots[i].transform, false);
            invShopselectBoxes[i].GetComponent<ShopSlot>().slotID = i;

            //이거 버튼 아님 이미지임 지을 이름이 없었음 
            Image invSelectImg = invShopselectBoxes[i].GetComponent<Image>();
            if (invSelectImg != null)
            {
                Color tempColor = invSelectImg.color;
                tempColor.a = 0f; // 투명하게 설정
                invSelectImg.color = tempColor;
                invSelectImg.raycastTarget = false;
            }

            Image invShopMesoUI = invShopselectBoxes[i].transform.Find("InvItemMesoUI").GetComponent<Image>();
            if(invShopMesoUI != null)
            {
                Color tempColor = invShopMesoUI.color;
                tempColor.a = 0f; // 투명하게 설정
                invShopMesoUI.color = tempColor;
                invShopMesoUI.raycastTarget = false;
            }

            // 버튼 컴포넌트를 찾아 클릭 이벤트 등록
            Button invShopselectButton = invShopselectBoxes[i].GetComponentInChildren<Button>();
            if (invShopselectButton != null)
            {
                invShopselectButton.gameObject.GetComponent<Image>().raycastTarget = true;
                int index = i; // 캡처 문제 해결을 위해 지역 변수를 사용합니다.
                invShopselectButton.onClick.AddListener(() => OnInvShopSelectButtonClicked(index));
            }
        }

        UpdateShopInventory();

        //// 샵 UI에 인벤토리 보유물 구성
        /////////////////////////////////

        Button buyBtn = shopPanel.transform.Find("BuyBtn").GetComponent<Button>();
        if (buyBtn != null)
        {
            buyBtn.onClick.AddListener(() => OnBuyButtonClicked());
        }

        Button exitBtn = shopPanel.transform.Find("ExitBtn").GetComponent<Button>();
        if (exitBtn != null)
        {
            exitBtn.onClick.AddListener(() => OnExitButtonClicked());
        }

        Button SellBtn = shopPanel.transform.Find("SellBtn").GetComponent<Button>();
        if (SellBtn != null)
        {
            SellBtn.onClick.AddListener(() => OnSellButtonClicked());
        }

        TextMeshProUGUI mesoText = shopPanel.transform.Find("Meso/MesoText").GetComponent<TextMeshProUGUI>();
        if(mesoText != null)
        {
            mesoText.text = playerData.meso.ToString();
        }
    }

    private void OnSellButtonClicked()
    {
        if (0 <= isInvSelectedBoxIndex && isInvSelectedBoxIndex < slotAmount)
        {
            if (iti[isInvSelectedBoxIndex].amount > 1)
            {
                PlaySound(SellSound);
                audioSource.volume = 0.2f;
                DataManager.instance.AddMeso(currentShopInv[isInvSelectedBoxIndex].SellPrice);
                inv.RemoveItem(currentShopInv[isInvSelectedBoxIndex].ID);

                Item SellItem = itemdataBase.FetchItemByID(currentShopInv[isInvSelectedBoxIndex].ID);
                for (int i = 0; i < quickSlot.slotAmount; i++)
                {
                    QuickSlotDT qSlotDT = quickSlot.slots[i].GetComponentInChildren<QuickSlotDT>();
                    if (qSlotDT != null && qSlotDT.iconPath == SellItem.IconPath)
                    {
                        quickSlot.RemoveQuicktSlotItem(qSlotDT.iconPath,qSlotDT.slotNum, 1);
                        break;
                    }
                }
            }
            else
            {

                DataManager.instance.AddMeso(currentShopInv[isInvSelectedBoxIndex].SellPrice);
                inv.RemoveItem(currentShopInv[isInvSelectedBoxIndex].ID);

                Item SellItem = itemdataBase.FetchItemByID(currentShopInv[isInvSelectedBoxIndex].ID);
                for (int i = 0; i < quickSlot.slotAmount; i++)
                {
                    QuickSlotDT qSlotDT = quickSlot.slots[i].GetComponentInChildren<QuickSlotDT>();
                    if (qSlotDT != null && qSlotDT.iconPath == SellItem.IconPath)
                    {
                        quickSlot.RemoveQuicktSlotItem(qSlotDT.iconPath, qSlotDT.slotNum, 1);
                        break;
                    }
                }
            }
        }
    }
    

    private void OnExitButtonClicked()
    {
        PlaySound(TabSound);
        audioSource.volume = 0.2f;
        visibleShop = false;
        shopParentPanel.SetActive(visibleShop);
    }

    private void OnBuyButtonClicked()
    {
        if (0 <= isSelectedBoxIndex && isSelectedBoxIndex < slotAmount)
        {
            if (playerData.meso - itemdataBase.dataBase[isSelectedBoxIndex].Price > 0)
            {
                PlaySound(BuySound);
                audioSource.volume = 0.2f;
                inv.AddItem(isSelectedBoxIndex);
                DataManager.instance.LoseMeso(itemdataBase.dataBase[isSelectedBoxIndex].Price);

                Item buyItem = itemdataBase.FetchItemByID(isSelectedBoxIndex);
                for (int i = 0; i < quickSlot.slotAmount; i++) 
                {
                    QuickSlotDT qSlotDT = quickSlot.slots[i].GetComponentInChildren<QuickSlotDT>();
                    if(qSlotDT != null && qSlotDT.iconPath == buyItem.IconPath)
                    {
                        quickSlot.AddAmountQuicktSlotItem(qSlotDT.slotNum, 1);
                        break;
                    }
                }
            }
        }
    }

    private void OnSelectButtonClicked(int index)
    {
        isSelectedBoxIndex = index;

        GameObject slotSelectBox = selectBoxes[index];

        Image selectBtnImage = slotSelectBox.GetComponent<Image>();

        foreach (var selectboxes in selectBoxes)
        {
            if (selectboxes.GetComponent<SelectBox>().isSelected)
            {
                selectboxes.GetComponent<SelectBox>().isSelected = false;

                Image selectboxesImage = selectboxes.GetComponent<Image>();

                selectboxesImage.sprite = null;
                Color tempColor = selectboxesImage.color;
                tempColor.a = 0f; // 투명하게 설정
                selectboxesImage.color = tempColor;
            }
        }

        selectBoxes[index].GetComponent<SelectBox>().isSelected = true;

        if (selectBtnImage.sprite == null || selectBtnImage.sprite.name != "ShopSelect")
        {
            selectBtnImage.sprite = Resources.Load<Sprite>("Shop/ShopSelect");
            Color tempColor = selectBtnImage.color;
            tempColor.a = 1f; // 불투명하게 설정
            selectBtnImage.color = tempColor;
        }
        else
        {
            if (selectBoxes[index].GetComponent<SelectBox>().isSelected)
                selectBoxes[index].GetComponent<SelectBox>().isSelected = false;

            selectBtnImage.sprite = null;
            Color tempColor = selectBtnImage.color;
            tempColor.a = 0f; // 투명하게 설정
            selectBtnImage.color = tempColor;
        }
    }


    public void UpdateShopMesoText(PlayerData pd) // 플레이어 메소에 따른 Shop MesoTextUI 갱신 
    {
        playerData = pd;

        TextMeshProUGUI mesoText = shopPanel.transform.Find("Meso/MesoText").GetComponent<TextMeshProUGUI>();
        if (mesoText != null)
        {
            mesoText.text = pd.meso.ToString();
        }
    }

    private void ClearShopInventory() // 상점에 보일 인벤토리 목록 초기화
    {
        currentShopInv.Clear();

        foreach (var invShopItem in inventoryItems)
        {
            invShopItem.GetComponent<InvItems>().isSet = false;

            Image invShopItemImg = invShopItem.GetComponent<Image>();

            invShopItemImg.sprite = null;
            Color tempColor = invShopItemImg.color;
            tempColor.a = 0f; // 투명하게 설정
            invShopItemImg.color = tempColor;

            invShopItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        for (int i = 0; i < invShopselectBoxes.Count; i++)
        {
            TextMeshProUGUI invitemNameText = invShopselectBoxes[i].transform.Find("InvItemNameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI sellText = invShopselectBoxes[i].transform.Find("SellText").GetComponent<TextMeshProUGUI>();
            Image MesoUI = invShopselectBoxes[i].transform.Find("InvItemMesoUI").GetComponent<Image>();

            if (MesoUI != null)
            {
                Color temColor = MesoUI.color;
                temColor.a = 0f; // 투명하게 설정
                MesoUI.color = temColor;
                MesoUI.raycastTarget = false;
            }

            if (invitemNameText != null)
            {
                invitemNameText.gameObject.GetComponent<TextMeshProUGUI>().raycastTarget = false;
                invitemNameText.text = "";
            }
            if (sellText != null)
            {
                sellText.gameObject.GetComponent<TextMeshProUGUI>().raycastTarget = false;
                sellText.text = "";
            }
        }
    }

    public void UpdateShopInventory() // 상점에 보일 인벤토리 목록 업데이트
    {
        ClearShopInventory();

        string inventoryDataPath = Application.persistentDataPath + "/Inventory.json";
        if (File.Exists(inventoryDataPath))
        {
            string inventoryDataJson = File.ReadAllText(inventoryDataPath);
            iti = JsonConvert.DeserializeObject<List<InventoryItem>>(inventoryDataJson); // 인벤토리 아이템 목록 가져오기 
        }
            for (int i = 0; i < iti.Count; i++)
        {
            if (iti[i].amount > 0 && iti[i].ID != -1) 
            {
                Item ItemToAdd = itemdataBase.FetchItemByID(iti[i].ID);
                currentShopInv.Add(ItemToAdd);
                foreach (var invShopItem in inventoryItems)
                {
                    if (invShopItem.GetComponent<InvItems>().isSet == false)
                    {
                        invShopItem.GetComponent<InvItems>().isSet = true;
                        Image invShopItemImg = invShopItem.GetComponent<Image>();

                        invShopItemImg.sprite = ItemToAdd.Icon;

                        Color tempColor = invShopItemImg.color;
                        tempColor.a = 1f; // 투명하게 설정
                        invShopItemImg.color = tempColor;

                        invShopItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = iti[i].amount.ToString();

                        TextMeshProUGUI invitemNameText = invShopselectBoxes[i].transform.Find("InvItemNameText").GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI sellText = invShopselectBoxes[i].transform.Find("SellText").GetComponent<TextMeshProUGUI>();
                        Image MesoUI = invShopselectBoxes[i].transform.Find("InvItemMesoUI").GetComponent<Image>();

                        if(MesoUI != null)
                        { 
                            Color temColor = MesoUI.color;
                            temColor.a = 1f; // 투명하게 설정
                            MesoUI.color = temColor;
                            MesoUI.raycastTarget = false;
                        }

                        if (invitemNameText != null)
                        {
                            invitemNameText.gameObject.GetComponent<TextMeshProUGUI>().raycastTarget = false;
                            invitemNameText.text = ItemToAdd.Name;
                        }
                        if (sellText != null)
                        {
                            sellText.gameObject.GetComponent<TextMeshProUGUI>().raycastTarget = false;
                            sellText.text = $"{ItemToAdd.SellPrice} 메소";
                        }
                        break;
                    }
                }
            }
        }
    }

    private void OnInvShopSelectButtonClicked(int index)
    {
        isInvSelectedBoxIndex = index;

        GameObject shopSlotSelectBox = invShopselectBoxes[index];

        Image selectBtnImage = shopSlotSelectBox.GetComponent<Image>();

        foreach (var selectboxes in invShopselectBoxes)
        {
            if (selectboxes.GetComponent<SelectBox>().isSelected)
            {
                selectboxes.GetComponent<SelectBox>().isSelected = false;

                Image selectboxesImage = selectboxes.GetComponent<Image>();

                selectboxesImage.sprite = null;
                Color tempColor = selectboxesImage.color;
                tempColor.a = 0f; // 투명하게 설정
                selectboxesImage.color = tempColor;
            }
        }

        invShopselectBoxes[index].GetComponent<SelectBox>().isSelected = true;

        if (selectBtnImage.sprite == null || selectBtnImage.sprite.name != "ShopSelect")
        {
            selectBtnImage.sprite = Resources.Load<Sprite>("Shop/ShopSelect");
            Color tempColor = selectBtnImage.color;
            tempColor.a = 1f; // 불투명하게 설정
            selectBtnImage.color = tempColor;
        }
        else
        {
            if (invShopselectBoxes[index].GetComponent<SelectBox>().isSelected)
                invShopselectBoxes[index].GetComponent<SelectBox>().isSelected = false;

            selectBtnImage.sprite = null;
            Color tempColor = selectBtnImage.color;
            tempColor.a = 0f; // 투명하게 설정
            selectBtnImage.color = tempColor;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
