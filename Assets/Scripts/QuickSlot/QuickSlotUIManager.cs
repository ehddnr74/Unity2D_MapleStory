using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotUIManager : MonoBehaviour
{
    private static QuickSlotUIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
