using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryStat : MonoBehaviour
{
    private static DontDestoryStat instance; // static ������ ����

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
