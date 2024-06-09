using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class QuickSlotItem
{
    public int slotNum;
    public string iconPath;


    public QuickSlotItem(int slotnum, string iconPath)
    {
        this.slotNum = slotnum;
        this.iconPath = iconPath;
    }
}

