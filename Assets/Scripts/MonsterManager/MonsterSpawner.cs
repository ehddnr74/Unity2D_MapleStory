using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public MonsterManager monsterManager;
    public GameObject someMonsterPrefab;
    public Transform[] spawnPoints; // �迭�� ���� ����Ʈ�� ����

    public int maxMonsters = 10; // �ִ� ������
    private int currentMonsterCount = 0; // ���� Ȱ��ȭ�� ���� ��

    void Start()
    {
        // ���� �ÿ� ���� ����Ʈ�� �ʱ� ���� ����
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnMonster(spawnPoints[i]);
        }
    }


    void SpawnMonster(Transform spawnPoint)
    {

        if (currentMonsterCount >= maxMonsters)
        {
            return; // �ִ� �������� �����ϸ� �������� ����
        }

        GameObject monster = monsterManager.GetMonsterFromPool(someMonsterPrefab);

        if (monster != null)
        {
            monster.transform.position = spawnPoint.position; // ���õ� ���� ����Ʈ�� ��ġ ����
            currentMonsterCount++;
        }
    }

    void DespawnMonster(GameObject monster)
    {
        monsterManager.ReturnMonsterToPool(monster, someMonsterPrefab);
        currentMonsterCount--;
    }
}

