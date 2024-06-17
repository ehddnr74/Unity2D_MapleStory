using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryShopUI : MonoBehaviour
{
    private static DontDestoryShopUI instance; // static ������ ����

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
