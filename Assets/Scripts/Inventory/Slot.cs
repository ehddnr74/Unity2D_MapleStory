using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
    public int id;
    private Inventory inv;

    public GameObject inventoryItemPrefab;

    public void ClearSlot()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateSlot(Item newItem, int amount)
    {
        ClearSlot();

        if (newItem != null)
        {
            GameObject itemObj = Instantiate(inventoryItemPrefab);
            ItemDT itemDT = itemObj.GetComponent<ItemDT>();
            itemDT.item = newItem;
            itemDT.amount = amount;
            itemDT.slot = id;
            itemDT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemDT.amount.ToString();
            itemDT.transform.SetParent(transform, false);
            itemObj.GetComponent<Image>().sprite = newItem.Icon;
            itemObj.name = newItem.Name;
            itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        ItemDT droppedItem = eventData.pointerDrag.GetComponent<ItemDT>();

        // ���� �������� ���� ���
        if (inv.items[id].ID == -1)
        { 
            // droppedItem�� ���� ������ �����
            inv.items[droppedItem.slot] = new Item();
            // ���ο� ���Կ� droppedItem�� �־���
            inv.items[id] = droppedItem.item;
            // droppedItem�� ���� ������Ʈ
            droppedItem.slot = id;
        }
        else
        {
            // ��ü�� �������� ������
            Transform item = this.transform.GetChild(0);
            ItemDT currentSlotItem = item.GetComponent<ItemDT>();

            // ��ü�� �������� ���� ���� ��ȣ�� ����
            int previousSlot = droppedItem.slot;

           //item.GetComponent<ItemDT>().slot = droppedItem.slot;

            // ���� ���Կ� �ִ� �������� droppedItem�� ���� �������� �ű�
            currentSlotItem.slot = previousSlot;
            item.transform.SetParent(inv.slots[previousSlot].transform);
            item.transform.position = inv.slots[previousSlot].transform.position;

            // droppedItem�� ���� �������� �ű�
            droppedItem.slot = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;

            // inv.items ����Ʈ ������Ʈ
            inv.items[previousSlot] = currentSlotItem.item;
            inv.items[id] = droppedItem.item;
        }
        // ������ ���� �÷��� ����
        inv.itemsChanged = true;
    } 
}
