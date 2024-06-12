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

    public bool secondSuriken;

    public float speed = 10f;
    public float lifetime = 0.5f; // �������� �߻�� �� Ǯ�� ��ȯ�Ǳ������ �ð�
    public string shurikenType; // ������ Ÿ�� 

    public bool direction;  // false�� �������� ���ư�

    private Transform target; // ������ ������ ��ġ
    public float trackingRange; // ���� ����

    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        secondSuriken = player.SecondSuriken;
        shurikenManager = FindObjectOfType<ShurikenManager>();

        direction = player.flipX;
        // ǥâ �߻� �� ���� �� �� ����
        CheckForEnemiesInRange();

        // ���� �ð��� ���� �� �������� Ǯ�� ��ȯ�ϴ� �ڷ�ƾ ����
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    void OnDisable()
    {
        target = null;
    }

    void Update()
    {
        if (target != null)
        {
            // ���Ͱ� �ִ� �������� ǥâ �̵�
            Vector2 targetDirection = (target.position - transform.position).normalized;
            transform.Translate(targetDirection * speed * Time.deltaTime);
        }
        else
        {
            // ���͸� �������� ������ ������ �������� ǥâ �̵�
            Vector2 moveDirection = direction ? Vector2.right : Vector2.left;
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    private void CheckForEnemiesInRange()
    {
        Vector2 directionVector = direction ? Vector2.right : Vector2.left;
        Vector2 boxSize = new Vector2(trackingRange, trackingRange / 2); // �� ���̷� �þ� ���� ����

        // �÷��̾� ��ġ���� ��¦ ������ �Ǵ� ���ʿ� Ž�� ������ ������ ���
        Vector2 offset = direction ? Vector2.right * 0.5f : Vector2.left * 0.5f;
        Vector2 startPosition = (Vector2)player.transform.position + offset;

        // �簢�� ���� ���� ���� ����
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(startPosition + directionVector * trackingRange / 2, boxSize, 0);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = collider.gameObject;
                }
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(lifetime); // ���� �ð�(1��) ���

        // �������� Ǯ�� ��ȯ
        if (shurikenManager != null)
        {
            shurikenManager.ReturnShurikenToPool(gameObject, shurikenType);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // ������ ��� ���� �߰�
            int damage = CalculateDamage(); // ��: �������� ����ϴ� �Լ�
            bool isCritical = CheckForCriticalHit(); // ��: ũ��Ƽ�� ��Ʈ�� Ȯ���ϴ� �Լ�

            // �浹�� ��ü�� RedSnailController�� ������ �ִ��� Ȯ���ϰ� �������� �ݴϴ�.
            RedSnailController enemy = collision.GetComponent<RedSnailController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, isCritical, secondSuriken);
                if (secondSuriken)
                    player.SecondSuriken = false;
            }
            // �������� Ǯ�� ��ȯ
            shurikenManager.ReturnShurikenToPool(gameObject, shurikenType);
        }
    }

    private int CalculateDamage()
    {
        // ������ ��� ���� (�ӽ÷� 10���� ����)
        return Random.Range(1, 1000);
    }

    private bool CheckForCriticalHit()
    {
        // ũ��Ƽ�� ��Ʈ Ȯ�� ���� (�ӽ÷� 20% Ȯ���� ũ��Ƽ�� ��Ʈ)
        return Random.value < 0.2f;
    }

    private void OnDrawGizmos()
    {
        // ���õ� ���¿��� ���� ������ �ð�ȭ
        Gizmos.color = Color.red;
        Vector2 directionVector = direction ? Vector2.right : Vector2.left;
        Vector2 boxSize = new Vector2(trackingRange, trackingRange / 2);
        Gizmos.DrawWireCube(transform.position + (Vector3)directionVector * trackingRange / 2, boxSize);
    }
}
