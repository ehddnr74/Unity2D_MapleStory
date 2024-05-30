using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    int slotAmount;
    GameObject quickSlotPanel;

    public GameObject quickSlot;
    public GameObject quickSlotItem;


    public List<GameObject> slots = new List<GameObject>();


    private void Start()
    {
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

        Sprite luckySevenSprite = Resources.Load<Sprite>("SkillIcons/LuckySeven");
        Sprite heistSprite = Resources.Load<Sprite>("SkillIcons/Heist");
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
                if (slotImage != null && slotImage.sprite == icon)
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
}
  


