using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroySkill : MonoBehaviour
{
    private static DontDestroySkill instance; // static ������ ����

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
