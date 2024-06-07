using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Shuriken : MonoBehaviour
{
    public Player player;
    public ShurikenManager shurikenManager;


    public float speed = 10f;
    public float lifetime = 0.5f; // �������� �߻�� �� Ǯ�� ��ȯ�Ǳ������ �ð�

    public bool direction;  // false�� �������� ���ư�

    private Transform target; // ������ ������ ��ġ
    public float trackingRange = 5f; // ���� ����

    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        shurikenManager = FindObjectOfType<ShurikenManager>();
        if (!player.flipX)
        {
            direction = false;
        }
        else
        {
            direction = true;
        }

        // ���� ���� �ڷ�ƾ ����
        StartCoroutine(TrackEnemy());

        // ���� �ð��� ���� �� �������� Ǯ�� ��ȯ�ϴ� �ڷ�ƾ ����
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    void Update()
    {
        if (target != null)
        {
            // ���Ͱ� �ִ� �������� ǥâ �̵�
            Vector2 targetDirection = target.position - transform.position;
            transform.Translate(targetDirection.normalized * speed * Time.deltaTime);
        }
        else
        {
            // ���͸� �������� ������ ������ �������� ǥâ �̵�
            if (!direction)
            {
                this.transform.Translate(Vector2.left * speed * Time.deltaTime);
            }
            else
            {
                this.transform.Translate(Vector2.right * speed * Time.deltaTime);
            }
        }
    }

    private IEnumerator TrackEnemy()
    {
        while (true)
        {
            // ���͸� �����ϴ� �ڵ�
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance && distanceToEnemy < trackingRange)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                target = nearestEnemy.transform;
            }
            else
            {
                target = null;
            }

            yield return new WaitForSeconds(0.5f); // �� 0.5�ʸ��� ���� ����
        }
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(lifetime); // ���� �ð�(1��) ���

        // �������� Ǯ�� ��ȯ
        if (shurikenManager != null)
        {
            shurikenManager.ReturnSurikenToPool(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            shurikenManager.ReturnSurikenToPool(gameObject);
        }
    }
}
