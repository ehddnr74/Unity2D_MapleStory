using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public int amount;
    public int slot;

    private Inventory inv;
    private InventoryTooltip tooltip;
    private QuickSlot qSlot;
    private Vector2 offset;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        tooltip = inv.GetComponent<InventoryTooltip>();
        qSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            inv.items[slot] = new Item();
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            this.transform.SetParent(this.transform.parent.parent.parent.parent);
            this.transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            this.transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Raycast를 사용하여 포인터가 어떤 UI 요소 위에 있는지 확인
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("QuickSlot"))
            {
                QuickSlotDT quickSlot = result.gameObject.GetComponent<QuickSlotDT>();
                if (quickSlot != null)
                {
                    if (item.ID != 6 && item.ID != 7)
                    {
                        quickSlot.itemIcon = item.Icon;
                        quickSlot.iconPath = item.IconPath;
                        quickSlot.itemAmount = amount;
                        // 해당 퀵슬롯에 아이템 추가
                        qSlot.AddItemToQuickSlot(item.Icon, quickSlot.slotNum, amount);
                        break;
                    }
                }
            }
        }
        this.transform.SetParent(inv.slots[slot].transform); // 원래의 부모로 되돌림
        this.transform.position = inv.slots[slot].transform.position; // 원래의 위치로 이동  
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }
}

