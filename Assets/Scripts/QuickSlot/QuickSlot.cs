using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using static UnityEditor.Progress;

public class QuickSlot : MonoBehaviour
{
    

    int slotAmount;
    GameObject quickSlotPanel;

    public GameObject quickSlot;
    public GameObject quickSlotItem;
    public bool itemsChanged = false;

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

            //������ �̹��� �����ϰ� ����
            Image slotItemImage = slotItem.GetComponent<Image>();
            if(slotItemImage != null)
            {
                Color tempColor = slotItemImage.color;
                tempColor.a = 0f; // �����ϰ� ����
                slotItemImage.color = tempColor;
            }

        }

        LoadQuickSlot();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            SaveQuickSlot();
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

            // �κ��丮 ������ ������� ���Կ� ������ ��ġ
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

    //�κ��丮 ������(ItemDT)�� ���� �����Կ� Icon�� �Ű����� ���� 
    // ���� icon�� ǥâ�� ��� return �ʿ�
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
                    // �̹� �ش� �������� �ִ� ��� ����
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
                        tempColor.a = 1f; // �������ϰ� ����
                        slotImage.color = tempColor;
                        quickSlotDT.itemIcon = icon;
                        quickSlotDT.iconPath = icon.name; // �������� ��θ� ����

                        // ������ �߰��� �Ϸ�� �� ������ ����
                        itemsChanged = true;
                    }
                }
            }
        }
    }

    // �� ������ ���� ������ ������ ��ȯ�ϴ� �޼���
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

                // ������ ��� ��ȯ
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
}
  


