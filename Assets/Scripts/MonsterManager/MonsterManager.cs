using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{

    public GameObject MonsterPrefab; // ���� ������
    public int poolSize = 10; // Ǯ ũ��

    private Queue<GameObject> monsterPool = new Queue<GameObject>(); // ���� ������Ʈ Ǯ

    private void Start()
    {
        // ������Ʈ Ǯ �ʱ�ȭ
        InitializePool();
    }

    private void InitializePool()
    {
        // Ǯ�� ���� ������Ʈ�� �̸� �����Ͽ� �߰�
        for (int i = 0; i < poolSize; i++)
        {
            GameObject monster = Instantiate(MonsterPrefab, Vector3.zero, Quaternion.identity);
            monster.SetActive(false); // ������ ���͸� ��Ȱ��ȭ ���·� ����
            monsterPool.Enqueue(monster); // ������ ���͸� Ǯ�� �߰�
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
            // Ǯ�� ��Ȱ��ȭ�� ���Ͱ� ������ ���� �����Ͽ� ��ȯ
            GameObject newMonster = Instantiate(MonsterPrefab, Vector3.zero, Quaternion.identity);
            return newMonster;
        }
    }

    public void ReturnMonsterToPool(GameObject monster)
    {
        // ���͸� ��Ȱ��ȭ ���·� ���� Ǯ�� ��ȯ
        monster.SetActive(false);
        monsterPool.Enqueue(monster);
    }
}
