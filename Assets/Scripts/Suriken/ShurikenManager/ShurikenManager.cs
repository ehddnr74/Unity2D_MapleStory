using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenManager : MonoBehaviour
{
    private static ShurikenManager instance;

    //public GameObject surikenPrefab; // 수리검 프리팹
    public GameObject ilbeShurikenPrefab; // 일비 표창 프리팹
    public GameObject flameShurikenPrefab;   // 플레임 표창 프리팹
    public int poolSize = 20; // 풀 크기

    //private Queue<GameObject> surikenPool = new Queue<GameObject>(); // 수리검 오브젝트 풀
    private Dictionary<string, Queue<GameObject>> shurikenPools = new Dictionary<string, Queue<GameObject>>(); // 표창 오브젝트 풀

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
                shurikenScript.shurikenType = shurikenType; // 수리검 타입 설정
                return shuriken;
            }
        }

        // 풀에 남아있는 수리검이 없는 경우 새로 생성
        GameObject prefab = GetPrefabByType(shurikenType);
        GameObject newShuriken = Instantiate(prefab, position, Quaternion.identity);
        Shuriken newShurikenScript = newShuriken.GetComponent<Shuriken>();
        newShurikenScript.shurikenType = shurikenType; // 수리검 타입 설정
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