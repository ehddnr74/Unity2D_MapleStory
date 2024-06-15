using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryBuff : MonoBehaviour
{
    private static DontDestoryBuff instance; // static ������ ����

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
