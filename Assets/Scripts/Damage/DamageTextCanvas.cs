using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextCanvas : MonoBehaviour
{

    public static DamageTextCanvas instance;
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
