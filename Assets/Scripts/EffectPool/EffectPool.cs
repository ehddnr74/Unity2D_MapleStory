using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : MonoBehaviour
{
    public GameObject[] prefabs; // Ǯ���� ������Ʈ�� �����յ�
    public int poolSize = 10; // Ǯ ������

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    public static EffectPool instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //InitializePool();
    }

    private void InitializePool()
    {
        foreach (var prefab in prefabs)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.name = prefab.name;
                obj.transform.SetParent(transform);
                objectPool.Enqueue(obj);
            }

            poolDictionary[prefab.name] = objectPool;
        }
    }

    public GameObject GetObject(string prefabName, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, float lifetime)
    {
        if (poolDictionary.ContainsKey(prefabName) && poolDictionary[prefabName].Count > 0)
        {
            GameObject obj = poolDictionary[prefabName].Dequeue();
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPosition;
            obj.transform.localRotation = localRotation;
            obj.transform.localScale = localScale;
            obj.SetActive(true);

            StartCoroutine(ReturnObjectAfterDelay(obj, lifetime));
            return obj;
        }
        else
        {
            Debug.LogWarning("No prefab with name " + prefabName + " found in pool.");
            return null;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        string prefabName = obj.name.Replace("(Clone)", ""); // "(Clone)" ����
        if (poolDictionary.ContainsKey(prefabName))
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform); // Ǯ�� ���ư� ���� �θ� Ǯ�� ����
            poolDictionary[prefabName].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("No pool with name " + prefabName + " found.");
        }
    }

    private IEnumerator ReturnObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnObject(obj);
    }
}
