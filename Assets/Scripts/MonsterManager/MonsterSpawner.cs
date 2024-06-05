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
        // ���÷� 5�ʸ��� ���͸� ����
        InvokeRepeating("SpawnMonster", 2.0f, 5.0f);
    }


    void SpawnMonster()
    {

        if (currentMonsterCount >= maxMonsters)
        {
            return; // �ִ� �������� �����ϸ� �������� ����
        }

        // �������� ���� ����Ʈ ����
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

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

