using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class SkillDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 offset;
    private Transform originalParent;

    private QuickSlot qSlot;

    public Sprite skillIcon;

    void Start()
    {
        qSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
        originalParent = this.transform.parent;
        this.transform.position = eventData.position - offset;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position - offset;
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
                    quickSlot.itemIcon = skillIcon;
                    //�ش� �����Կ� ������ �߰�
                    qSlot.AddItemToQuickSlot(skillIcon, quickSlot.slotNum);
                    //break;
                }
            }
        }
        this.transform.SetParent(originalParent);
        this.transform.position = originalParent.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}