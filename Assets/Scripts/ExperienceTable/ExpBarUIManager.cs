using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBarUIManager : MonoBehaviour
{
    private static ExpBarUIManager instance;
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
