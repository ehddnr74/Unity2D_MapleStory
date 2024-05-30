using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out originalLocalPointerPosition
        );
        originalPanelLocalPosition = rectTransform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        ))
        {
            Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
            rectTransform.localPosition = originalPanelLocalPosition + new Vector3(offsetToOriginal.x, offsetToOriginal.y, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그가 끝났을 때 필요한 로직이 있으면 여기에 추가합니다.
    }
}