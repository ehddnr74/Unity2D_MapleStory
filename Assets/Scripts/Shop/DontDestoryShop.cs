using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryShop : MonoBehaviour
{
    private static DontDestoryShop instance; // static ������ ����

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
