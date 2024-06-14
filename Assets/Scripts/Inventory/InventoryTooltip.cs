using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTooltip : MonoBehaviour
{
    private Item item;
    private GameObject tooltip;

    private Image itemImage;

    private Inventory inv;

    private void Start()
    {
        tooltip = GameObject.Find("ToolTip");
        itemImage = tooltip.GetComponent<Image>();
        tooltip.SetActive(false);

        inv = GameObject.Find("Inventory").GetComponent<Inventory>();   
    }

    private void Update()
    {
        if(tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition;
        }

        if (!inv.activeInventory)
        {
            Deactivate();
        }
                
    }
    public void Activate(Item item)
    {
        this.item = item;
        itemImage.sprite = item.ToolTip;
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }
}
