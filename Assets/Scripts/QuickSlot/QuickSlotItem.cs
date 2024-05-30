//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class QuickSlotItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
//{
//    private Transform originalParent;
//    private Vector2 originalPosition;
//    private Canvas canvas;
//    private RectTransform rectTransform;
//    private CanvasGroup canvasGroup;
//    private Image image;

//    private void Awake()
//    {
//        rectTransform = GetComponent<RectTransform>();
//        canvasGroup = GetComponent<CanvasGroup>();
//        image = GetComponent<Image>();
//    }

//    public void SetSprite(Sprite sprite)
//    {
//        image.sprite = sprite;
//        image.enabled = sprite != null;
//    }

//    public Sprite GetSprite()
//    {
//        return image.sprite;
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        originalParent = transform.parent;
//        originalPosition = rectTransform.anchoredPosition;
//        canvas = originalParent.GetComponentInParent<Canvas>();

//        transform.SetParent(canvas.transform);
//        canvasGroup.blocksRaycasts = false;
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        canvasGroup.blocksRaycasts = true;
//        transform.SetParent(originalParent);
//        rectTransform.anchoredPosition = originalPosition;

//        RaycastResult result = eventData.pointerCurrentRaycast;
//        if (result.gameObject != null && result.gameObject.CompareTag("QuickSlot"))
//        {
//            Transform targetSlot = result.gameObject.transform;
//            transform.SetParent(targetSlot);
//            rectTransform.anchoredPosition = Vector2.zero;
//        }
//        else
//        {
//            // 드래그가 실패하면 원래 자리로 돌아갑니다.
//            rectTransform.anchoredPosition = originalPosition;
//            transform.SetParent(originalParent);
//        }
//    }
//}