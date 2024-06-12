using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenManager : MonoBehaviour
{
    private static ShurikenManager instance;

    //public GameObject surikenPrefab; // ������ ������
    public GameObject ilbeShurikenPrefab; // �Ϻ� ǥâ ������
    public GameObject flameShurikenPrefab;   // �÷��� ǥâ ������
    public int poolSize = 20; // Ǯ ũ��

    //private Queue<GameObject> surikenPool = new Queue<GameObject>(); // ������ ������Ʈ Ǯ
    private Dictionary<string, Queue<GameObject>> shurikenPools = new Dictionary<string, Queue<GameObject>>(); // ǥâ ������Ʈ Ǯ

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool("ilbe", ilbeShurikenPrefab);
            InitializePool("flame", flameShurikenPrefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void InitializePool(string shurikenType, GameObject prefab)
    {
        Queue<GameObject> pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject shuriken = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            shuriken.SetActive(false);
            pool.Enqueue(shuriken);
        }
        shurikenPools[shurikenType] = pool;
    }

    public GameObject GetShurikenFromPool(int itemId, Vector3 position)
    {
        string shurikenType = GetShurikenTypeById(itemId);

        if (shurikenPools.ContainsKey(shurikenType) && shurikenPools[shurikenType].Count > 0)
        {
            GameObject shuriken = shurikenPools[shurikenType].Dequeue();
            if (shuriken != null)
            {
                shuriken.SetActive(true);
                shuriken.transform.position = position;
                Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
                shurikenScript.shurikenType = shurikenType; // ������ Ÿ�� ����
                return shuriken;
            }
        }

        // Ǯ�� �����ִ� �������� ���� ��� ���� ����
        GameObject prefab = GetPrefabByType(shurikenType);
        GameObject newShuriken = Instantiate(prefab, position, Quaternion.identity);
        Shuriken newShurikenScript = newShuriken.GetComponent<Shuriken>();
        newShurikenScript.shurikenType = shurikenType; // ������ Ÿ�� ����
        return newShuriken;
    }

    private string GetShurikenTypeById(int itemId)
    {
        switch (itemId)
        {
            case 7:
                return "flame";
            case 6:
            default:
                return "ilbe";
        }
    }

    private GameObject GetPrefabByType(string shurikenType)
    {
        switch (shurikenType)
        {
            case "flame":
                return flameShurikenPrefab;
            case "ilbe":
            default:
                return ilbeShurikenPrefab;
        }
    }

    public void ReturnShurikenToPool(GameObject shuriken, string shurikenType)
    {
        shuriken.SetActive(false);
        if (!shurikenPools.ContainsKey(shurikenType))
        {
            shurikenPools[shurikenType] = new Queue<GameObject>();
        }
        shurikenPools[shurikenType].Enqueue(shuriken);
    }

}