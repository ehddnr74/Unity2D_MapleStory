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
    // �� UI ������ ����
    int slotAmount;

    public bool visibleShop = false; // ������ �������� �ִ°�

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

    // �� UI �κ��丮 ���빰 ����
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
        // ������ �Ŵ����� ���� �÷��̾� ������ �ε�
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

        for (int i = 0; i < slotAmount; i++) // ��UI ������ ���� 
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

            //�̰� ��ư �ƴ� �̹����� ���� �̸��� ������ 
            Image selectBtn = selectBoxes[i].GetComponent<Image>();
            if (selectBtn != null)
            {
                Color tempColor = selectBtn.color;
                tempColor.a = 0f; // �����ϰ� ����
                selectBtn.color = tempColor;
                selectBtn.raycastTarget = false;
            }

            // ��ư ������Ʈ�� ã�� Ŭ�� �̺�Ʈ ���
            Button selectButton = selectBoxes[i].GetComponentInChildren<Button>();
            if (selectButton != null)
            {
                selectButton.gameObject.GetComponent<Image>().raycastTarget = true;
                int index = i; // ĸó ���� �ذ��� ���� ���� ������ ����մϴ�.
                selectButton.onClick.AddListener(() => OnSelectButtonClicked(index));
            }

            // �ؽ�Ʈ ������Ʈ�� ã�Ƽ� ����
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
                priceText.text = $"{currentItem.Price} �޼�";
            }
        }

        ///////////////////////////////// 
        ///  �� UI�� �κ��丮 ������ ����
        invslotAmount = 24;

        string contentPath = "Scroll View/Viewport/Content";
        Transform contentTransform = shopPanel.transform.Find(contentPath);
        Content = contentTransform.gameObject; // Content�� �ڽ����� ���Ե� �����ϱ� ���� ��ġ ã��

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

            //�̰� ��ư �ƴ� �̹����� ���� �̸��� ������ 
            Image invSelectImg = invShopselectBoxes[i].GetComponent<Image>();
            if (invSelectImg != null)
            {
                Color tempColor = invSelectImg.color;
                tempColor.a = 0f; // �����ϰ� ����
                invSelectImg.color = tempColor;
                invSelectImg.raycastTarget = false;
            }

            Image invShopMesoUI = invShopselectBoxes[i].transform.Find("InvItemMesoUI").GetComponent<Image>();
            if(invShopMesoUI != null)
            {
                Color tempColor = invShopMesoUI.color;
                tempColor.a = 0f; // �����ϰ� ����
                invShopMesoUI.color = tempColor;
                invShopMesoUI.raycastTarget = false;
            }

            // ��ư ������Ʈ�� ã�� Ŭ�� �̺�Ʈ ���
            Button invShopselectButton = invShopselectBoxes[i].GetComponentInChildren<Button>();
            if (invShopselectButton != null)
            {
                invShopselectButton.gameObject.GetComponent<Image>().raycastTarget = true;
                int index = i; // ĸó ���� �ذ��� ���� ���� ������ ����մϴ�.
                invShopselectButton.onClick.AddListener(() => OnInvShopSelectButtonClicked(index));
            }
        }

        UpdateShopInventory();

        //// �� UI�� �κ��丮 ������ ����
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
                tempColor.a = 0f; // �����ϰ� ����
                selectboxesImage.color = tempColor;
            }
        }

        selectBoxes[index].GetComponent<SelectBox>().isSelected = true;

        if (selectBtnImage.sprite == null || selectBtnImage.sprite.name != "ShopSelect")
        {
            selectBtnImage.sprite = Resources.Load<Sprite>("Shop/ShopSelect");
            Color tempColor = selectBtnImage.color;
            tempColor.a = 1f; // �������ϰ� ����
            selectBtnImage.color = tempColor;
        }
        else
        {
            if (selectBoxes[index].GetComponent<SelectBox>().isSelected)
                selectBoxes[index].GetComponent<SelectBox>().isSelected = false;

            selectBtnImage.sprite = null;
            Color tempColor = selectBtnImage.color;
            tempColor.a = 0f; // �����ϰ� ����
            selectBtnImage.color = tempColor;
        }
    }


    public void UpdateShopMesoText(PlayerData pd) // �÷��̾� �޼ҿ� ���� Shop MesoTextUI ���� 
    {
        playerData = pd;

        TextMeshProUGUI mesoText = shopPanel.transform.Find("Meso/MesoText").GetComponent<TextMeshProUGUI>();
        if (mesoText != null)
        {
            mesoText.text = pd.meso.ToString();
        }
    }

    private void ClearShopInventory() // ������ ���� �κ��丮 ��� �ʱ�ȭ
    {
        currentShopInv.Clear();

        foreach (var invShopItem in inventoryItems)
        {
            invShopItem.GetComponent<InvItems>().isSet = false;

            Image invShopItemImg = invShopItem.GetComponent<Image>();

            invShopItemImg.sprite = null;
            Color tempColor = invShopItemImg.color;
            tempColor.a = 0f; // �����ϰ� ����
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
                temColor.a = 0f; // �����ϰ� ����
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

    public void UpdateShopInventory() // ������ ���� �κ��丮 ��� ������Ʈ
    {
        ClearShopInventory();

        string inventoryDataPath = Application.persistentDataPath + "/Inventory.json";
        if (File.Exists(inventoryDataPath))
        {
            string inventoryDataJson = File.ReadAllText(inventoryDataPath);
            iti = JsonConvert.DeserializeObject<List<InventoryItem>>(inventoryDataJson); // �κ��丮 ������ ��� �������� 
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
                        tempColor.a = 1f; // �����ϰ� ����
                        invShopItemImg.color = tempColor;

                        invShopItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = iti[i].amount.ToString();

                        TextMeshProUGUI invitemNameText = invShopselectBoxes[i].transform.Find("InvItemNameText").GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI sellText = invShopselectBoxes[i].transform.Find("SellText").GetComponent<TextMeshProUGUI>();
                        Image MesoUI = invShopselectBoxes[i].transform.Find("InvItemMesoUI").GetComponent<Image>();

                        if(MesoUI != null)
                        { 
                            Color temColor = MesoUI.color;
                            temColor.a = 1f; // �����ϰ� ����
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
                            sellText.text = $"{ItemToAdd.SellPrice} �޼�";
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
                tempColor.a = 0f; // �����ϰ� ����
                selectboxesImage.color = tempColor;
            }
        }

        invShopselectBoxes[index].GetComponent<SelectBox>().isSelected = true;

        if (selectBtnImage.sprite == null || selectBtnImage.sprite.name != "ShopSelect")
        {
            selectBtnImage.sprite = Resources.Load<Sprite>("Shop/ShopSelect");
            Color tempColor = selectBtnImage.color;
            tempColor.a = 1f; // �������ϰ� ����
            selectBtnImage.color = tempColor;
        }
        else
        {
            if (invShopselectBoxes[index].GetComponent<SelectBox>().isSelected)
                invShopselectBoxes[index].GetComponent<SelectBox>().isSelected = false;

            selectBtnImage.sprite = null;
            Color tempColor = selectBtnImage.color;
            tempColor.a = 0f; // �����ϰ� ����
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
