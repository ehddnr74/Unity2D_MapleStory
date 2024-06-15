using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class SkillDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector2 offset;
    private Transform originalParent;

    private QuickSlot qSlot;

    private SkillToolTip skillTooltip;

    public Sprite skillIcon;
    public string skillName;
    public int skillLevel;
    public string skillToolTipPath;

    void Start()
    {
        qSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
        GameObject skill = GameObject.Find("Skill").gameObject;
        skillTooltip = skill.GetComponent<SkillToolTip>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillName != "크리티컬 샷")
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            originalParent = this.transform.parent;
            this.transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillName != "크리티컬 샷")
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
                    if (skillName != "크리티컬 샷")
                    {
                        quickSlot.itemIcon = skillIcon;
                        //해당 퀵슬롯에 아이템 추가
                        qSlot.AddItemToQuickSlot(skillIcon, quickSlot.slotNum, 0);
                        //break;
                    }
                }
            }
        }
        if (skillName != "크리티컬 샷")
        {
            this.transform.SetParent(originalParent);
            this.transform.position = originalParent.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(skillLevel > 0)
        skillTooltip.Activate(skillToolTipPath);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillLevel > 0)
            skillTooltip.Deactivate();
    }
}
