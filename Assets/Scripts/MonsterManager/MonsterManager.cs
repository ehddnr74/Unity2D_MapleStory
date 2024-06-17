using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{

    public GameObject[] MonsterPrefabs;
    public int poolSize = 50; // 풀 크기

    private Dictionary<GameObject, Queue<GameObject>> monsterPools = new Dictionary<GameObject, Queue<GameObject>>();
    //public List<Transform> points = new List<Transform>();

    private Queue<GameObject> monsterPool = new Queue<GameObject>(); // 몬스터 오브젝트 풀


    private void Start()
    {
        foreach (var prefab in MonsterPrefabs)
        {
            InitializePool(prefab);
        }
    }

    private void InitializePool(GameObject prefab)
    {
        Queue<GameObject> pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject monster = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            monster.SetActive(false);
            pool.Enqueue(monster);
        }
        monsterPools[prefab] = pool;
    }

    public GameObject GetMonsterFromPool(GameObject prefab)
    {
        if (monsterPools.ContainsKey(prefab) && monsterPools[prefab].Count > 0)
        {
            GameObject monster = monsterPools[prefab].Dequeue();
            monster.SetActive(true);
            return monster;
        }
        else
        {
            GameObject newMonster = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            return newMonster;
        }
    }

    public void ReturnMonsterToPool(GameObject monster, GameObject prefab)
    {
        monster.SetActive(false);
        if (!monsterPools.ContainsKey(prefab))
        {
            monsterPools[prefab] = new Queue<GameObject>();
        }
        monsterPools[prefab].Enqueue(monster);
    }
}