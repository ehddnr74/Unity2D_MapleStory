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
        if (shop != null)
        {
            shop.visibleShop = !shop.visibleShop;
            shop.shopParentPanel.SetActive(shop.visibleShop);
            shop.UpdateShopInventory();
        }
    }
}
