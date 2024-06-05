using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemManager : MonoBehaviour
{
    private static EventSystemManager instance;
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
