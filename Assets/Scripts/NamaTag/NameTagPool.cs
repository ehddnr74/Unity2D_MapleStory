using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTagPool : MonoBehaviour
{
    public GameObject NameTagPrefab; // HP 바 프리팹
    public int poolSize = 50; // 풀 크기

    private Queue<GameObject> nameTagPool = new Queue<GameObject>();

    public static NameTagPool instance;

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
            CreateNewNameTag();
        }
    }

    private void CreateNewNameTag()
    {
        GameObject NameTagInstance = Instantiate(NameTagPrefab, transform);
        NameTagInstance.SetActive(false);
        nameTagPool.Enqueue(NameTagInstance);
    }

    public GameObject GetNameTag()
    {
        if (nameTagPool.Count > 0)
        {
            GameObject NameTagInstance = nameTagPool.Dequeue();
            NameTagInstance.SetActive(true);
            return NameTagInstance;
        }
        else
        {
            CreateNewNameTag();
            return GetNameTag();
        }
    }

    public void ReturnNameTag(GameObject NameTagInstance)
    {
        NameTagInstance.SetActive(false);
        nameTagPool.Enqueue(NameTagInstance);
    }
}
