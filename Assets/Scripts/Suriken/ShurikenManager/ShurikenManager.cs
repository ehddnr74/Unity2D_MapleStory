using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenManager : MonoBehaviour
{
    private static ShurikenManager instance;

    public GameObject surikenPrefab; // ������ ������
    public int poolSize = 20; // Ǯ ũ��

    private Queue<GameObject> surikenPool = new Queue<GameObject>(); // ������ ������Ʈ Ǯ

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

    //private void Start()
    //{
    //    InitializePool();
    //}

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject suriken = Instantiate(surikenPrefab, Vector3.zero, Quaternion.identity);
            suriken.SetActive(false);
            surikenPool.Enqueue(suriken);
        }
    }

    public GameObject GetSurikenFromPool(Vector3 position)
    {
        if (surikenPool.Count > 0)
        {
            GameObject suriken = surikenPool.Dequeue();
            if (suriken != null)
            {
                suriken.SetActive(true);
                suriken.transform.position = position;
                return suriken;
            }
        }
        // Ǯ�� �����ִ� �������� ���� ��� ���� ����
        GameObject newSuriken = Instantiate(surikenPrefab, position, Quaternion.identity);
        return newSuriken;
    }

    public void ReturnSurikenToPool(GameObject suriken)
    {
        suriken.SetActive(false);
        surikenPool.Enqueue(suriken);
    }

}