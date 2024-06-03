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

        // 기존 아이템이 없는 경우
        if (inv.items[id].ID == -1)
        { 
            // droppedItem의 이전 슬롯을 비워줌
            inv.items[droppedItem.slot] = new Item();
            // 새로운 슬롯에 droppedItem을 넣어줌
            inv.items[id] = droppedItem.item;
            // droppedItem의 슬롯 업데이트
            droppedItem.slot = id;
        }
        else
        {
            // 교체할 아이템을 가져옴
            Transform item = this.transform.GetChild(0);
            ItemDT currentSlotItem = item.GetComponent<ItemDT>();

            // 교체할 아이템의 현재 슬롯 번호를 보관
            int previousSlot = droppedItem.slot;

           //item.GetComponent<ItemDT>().slot = droppedItem.slot;

            // 현재 슬롯에 있는 아이템을 droppedItem의 이전 슬롯으로 옮김
            currentSlotItem.slot = previousSlot;
            item.transform.SetParent(inv.slots[previousSlot].transform);
            item.transform.position = inv.slots[previousSlot].transform.position;

            // droppedItem을 현재 슬롯으로 옮김
            droppedItem.slot = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;

            // inv.items 리스트 업데이트
            inv.items[previousSlot] = currentSlotItem.item;
            inv.items[id] = droppedItem.item;
        }
        // 아이템 변경 플래그 설정
        inv.itemsChanged = true;
    } 
}
