using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    public Item item;
    public int amount;
    public int slot;

    private Inventory inv;
    private QuickSlot qSlot;
    private Vector2 offset;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        qSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
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
        // Raycast�� ����Ͽ� �����Ͱ� � UI ��� ���� �ִ��� Ȯ��
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
                        // �ش� �����Կ� ������ �߰�
                        qSlot.AddItemToQuickSlot(item.Icon, quickSlot.slotNum);
                        break;
                    }
                }
            }
        }
        this.transform.SetParent(inv.slots[slot].transform); // ������ �θ�� �ǵ���
        this.transform.position = inv.slots[slot].transform.position; // ������ ��ġ�� �̵�  
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}

