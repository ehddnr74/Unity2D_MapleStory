using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{

    public GameObject MonsterPrefab; // 몬스터 프리팹
    public int poolSize = 10; // 풀 크기

    private Queue<GameObject> monsterPool = new Queue<GameObject>(); // 몬스터 오브젝트 풀

    private void Start()
    {
        // 오브젝트 풀 초기화
        InitializePool();
    }

    private void InitializePool()
    {
        // 풀에 몬스터 오브젝트를 미리 생성하여 추가
        for (int i = 0; i < poolSize; i++)
        {
            GameObject monster = Instantiate(MonsterPrefab, Vector3.zero, Quaternion.identity);
            monster.SetActive(false); // 생성한 몬스터를 비활성화 상태로 설정
            monsterPool.Enqueue(monster); // 생성한 몬스터를 풀에 추가
        }
    }

    public GameObject GetMonsterFromPool()
    {
        if (monsterPool.Count > 0)
        {
            GameObject monster = monsterPool.Dequeue();
            monster.SetActive(true);
            return monster;
        }
        else
        {
            // 풀에 비활성화된 몬스터가 없으면 새로 생성하여 반환
            GameObject newMonster = Instantiate(MonsterPrefab, Vector3.zero, Quaternion.identity);
            return newMonster;
        }
    }

    public void ReturnMonsterToPool(GameObject monster)
    {
        // 몬스터를 비활성화 상태로 만들어서 풀에 반환
        monster.SetActive(false);
        monsterPool.Enqueue(monster);
    }
}
