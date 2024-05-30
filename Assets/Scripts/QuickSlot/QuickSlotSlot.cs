using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlotSlot : MonoBehaviour
{   
    public int slotID;
    public Image slotImage;

    public void SetID(int id)
    {
        slotID = id;
    }

    public int GetID()
    {
        return slotID;
    }
}
