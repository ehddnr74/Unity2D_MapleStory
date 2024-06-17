using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    public GameObject itemPrefab;
    public int poolSize = 40;

    private Queue<GameObject> itemPool;

    public static ItemPool instance;

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
        itemPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject item = Instantiate(itemPrefab);
            item.SetActive(false);
            itemPool.Enqueue(item);
        }
    }

    public GameObject GetItem(Vector3 position, Quaternion rotation)
    {
        GameObject item;
        if (itemPool.Count > 0)
        {
            item = itemPool.Dequeue();
            if (item == null) // 혹시 null일 경우 대비
            {
                item = Instantiate(itemPrefab, position, rotation);
            }
        }
        else
        {
            item = Instantiate(itemPrefab, position, rotation);
        }
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.SetActive(true);
        return item;
    }

    public void ReturnItem(GameObject item)
    {
        if (item != null)
        {
            item.SetActive(false);
            itemPool.Enqueue(item);
        }
    }
}
