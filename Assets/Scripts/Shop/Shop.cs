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

public class Shop : MonoBehaviour
{
    int slotAmount;

    private PlayerData playerData;

    public GameObject shopitem;
    public GameObject selectBox;

    private Inventory inv;
    public int isSelectedBoxIndex = -1;

    private GameObject shopParentPanel;
    private GameObject shopPanel;
    private GameObject SlotPanel;
    public GameObject Slot;

    ItemDataBase itemdataBase;

    public List<GameObject> slots = new List<GameObject>();
    public List<GameObject> selectBoxes = new List<GameObject>();



    private void Start()
    {
        // ������ �Ŵ����� ���� �÷��̾� ������ �ε�
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadData();
            playerData = DataManager.instance.nowPlayer;
        }

        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        itemdataBase = GetComponent<ItemDataBase>();

        slotAmount = 8;
        SlotPanel = GameObject.Find("SlotPanel");
        shopPanel = GameObject.Find("ShopPanel");
        shopParentPanel = GameObject.Find("ShopParentPanel");

        for (int i = 0; i < slotAmount; i++)
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
        TextMeshProUGUI mesoText = shopPanel.transform.Find("Meso/MesoText").GetComponent<TextMeshProUGUI>();
        if(mesoText != null)
        {
            mesoText.text = playerData.meso.ToString();
        }
    }

    private void OnExitButtonClicked()
    {
        shopParentPanel.SetActive(false);
    }

    private void OnBuyButtonClicked()
    {
        if (0 <= isSelectedBoxIndex && isSelectedBoxIndex < slotAmount)
        {
            if (playerData.meso - itemdataBase.dataBase[isSelectedBoxIndex].Price > 0)
            {
                inv.AddItem(isSelectedBoxIndex);
                DataManager.instance.LoseMeso(itemdataBase.dataBase[isSelectedBoxIndex].Price);
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

}
