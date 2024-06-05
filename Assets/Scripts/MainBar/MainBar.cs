using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBar : MonoBehaviour
{
    private static MainBar instance;
    void Awake()
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

