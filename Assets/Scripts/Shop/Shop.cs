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

    public GameObject shopitem;
    public GameObject selectBox;

    private Item shopItem;

    private GameObject SlotPanel;
    public GameObject Slot;

    ItemDataBase itemdataBase;

    public List<GameObject> slots = new List<GameObject>();
    public List<GameObject> selectBoxes = new List<GameObject>();

    private void Start()
    {
        itemdataBase = GetComponent<ItemDataBase>();

        slotAmount = 8;
        SlotPanel = GameObject.Find("SlotPanel");

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
    }

    private void OnSelectButtonClicked(int index)
    {
        Debug.Log("Select button clicked: " + index);

        GameObject slotSelectBox = selectBoxes[index];

        Image selectBtnImage = slotSelectBox.GetComponent<Image>();

        if (selectBtnImage != null)
        {
            if (selectBtnImage.sprite == null || selectBtnImage.sprite.name != "ShopSelect")
            {
                selectBtnImage.sprite = Resources.Load<Sprite>("Shop/ShopSelect");
                Color tempColor = selectBtnImage.color;
                tempColor.a = 1f; // 불투명하게 설정
                selectBtnImage.color = tempColor;
            }
            else
            {
                selectBtnImage.sprite = null;
                Color tempColor = selectBtnImage.color;
                tempColor.a = 0f; // 투명하게 설정
                selectBtnImage.color = tempColor;
            }
        }
    }
}
