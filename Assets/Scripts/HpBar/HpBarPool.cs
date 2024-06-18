using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarPool : MonoBehaviour
{
    public GameObject hpBarPrefab; // HP 바 프리팹
    public int poolSize = 50; // 풀 크기

    private Queue<GameObject> hpBarPool = new Queue<GameObject>();

    public static HpBarPool instance;

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

    void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewHPBar();
        }
    }

    private void CreateNewHPBar()
    {
        GameObject hpBarInstance = Instantiate(hpBarPrefab, transform);
        hpBarInstance.SetActive(false);
        hpBarPool.Enqueue(hpBarInstance);
    }

    public GameObject GetHPBar()
    {
        if (hpBarPool.Count > 0)
        {
            GameObject hpBarInstance = hpBarPool.Dequeue();
            hpBarInstance.SetActive(true);
            return hpBarInstance;
        }
        else
        {
            CreateNewHPBar();
            return GetHPBar();
        }
    }

    public void ReturnHPBar(GameObject hpBarInstance)
    {
        hpBarInstance.SetActive(false);
        hpBarPool.Enqueue(hpBarInstance);
    }
}
