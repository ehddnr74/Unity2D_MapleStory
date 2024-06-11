using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlotDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int slotNum;
    public Sprite itemIcon;
    public string iconPath;

    private QuickSlot quickSlot;
    private Vector2 offset;
    private Transform originalParent;

    public int itemAmount; // ������ ���� 

    void Start()
    {
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemIcon != null)
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            originalParent = this.transform.parent;
            //this.transform.SetParent(this.transform.parent.parent.parent);
            this.transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemIcon != null)
        {
            this.transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject pointerEnterObject = eventData.pointerEnter;

        if (pointerEnterObject != null && pointerEnterObject.CompareTag("QuickSlot"))
        {
            QuickSlotDT targetSlotDT = pointerEnterObject.GetComponent<QuickSlotDT>();
            if (targetSlotDT != null)
            {
                quickSlot.SwapItems(slotNum, targetSlotDT.slotNum);
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
            this.transform.SetParent(originalParent);
            this.transform.position = originalParent.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            quickSlot.itemsChanged = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (iconPath == "Key.Item" || iconPath == "Key.Minimap" || iconPath == "Key.PickUp" 
            || iconPath == "Key.Attack" || iconPath == "Key.Skill" || iconPath == "Key.Stat" || iconPath == "Key.Jump")
            return;
        

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // ���콺 ������ Ŭ�� �� ������ �������� �����, �ش� ������ ������ �ʱ�ȭ
            itemIcon = null;
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // �����ϰ� ����
            itemAmount = 0;
            GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
        }
        quickSlot.itemsChanged = true;
    }
}
