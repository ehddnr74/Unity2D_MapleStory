using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryShop : MonoBehaviour
{
    private static DontDestoryShop instance; // static 변수로 변경

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
