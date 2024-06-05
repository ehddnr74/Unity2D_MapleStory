using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopKeeper : MonoBehaviour
{
    private Shop shop;

    void Start()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
    }

    void OnMouseDown()
    {
        if (shop != null && !IsPointerOverUIObject())
        {
            shop.visibleShop = !shop.visibleShop;
            shop.shopParentPanel.SetActive(shop.visibleShop);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == shop.shopParentPanel)
            {
                return true;
            }
        }
        return false;
    }
}
