using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public int ID;
    public int amount;
    public int slotnum;

    public InventoryItem(int id, int amount, int slotnum)
    {
        this.ID = id;
        this.amount = amount;
        this.slotnum = slotnum;
    }
}
