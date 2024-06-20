using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryNameTag : MonoBehaviour
{
    public static DontDestoryNameTag instance;

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
