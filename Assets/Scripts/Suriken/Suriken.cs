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

    public bool luckySeven;

    public bool secondSuriken;

    public float criticalProbability;

    public float speed = 10f;
    public float lifetime = 0.5f; // �������� �߻�� �� Ǯ�� ��ȯ�Ǳ������ �ð�
    public string shurikenType; // ������ Ÿ�� 

    public bool direction;  // false�� �������� ���ư�

    private Transform target; // ������ ������ ��ġ
    public float trackingRange; // ���� ����

    private bool isCriticalHit; //ũ��Ƽ�� ���� 

    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
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
            criticalProbability = DataManager.instance.nowPlayer.criticalProbability;
            bool isCritical = CheckForCriticalHit(); // ��: ũ��Ƽ�� ��Ʈ�� Ȯ���ϴ� �Լ�
            // ������ ��� ���� �߰�
            int damage = CalculateDamage(); // ��: �������� ����ϴ� �Լ�

            // �浹�� ��ü�� RedSnailController�� ������ �ִ��� Ȯ���ϰ� �������� �ݴϴ�.
            RedSnailController redSnail = collision.GetComponent<RedSnailController>();
            BlueSnailController blueSnail = collision.GetComponent<BlueSnailController>();
            GreenMushRoomController greenMushRoom = collision.GetComponent<GreenMushRoomController>();
            if (redSnail != null)
            {
                if (!secondSuriken)
                {
                    Vector3 displayPosition = collision.transform.position + new Vector3(-1.5f, 3.0f, 0);
                    redSnail.TakeDamage(displayPosition, damage, isCritical, luckySeven, secondSuriken);
                }
                else
                {
                    Vector3 secondPoisition = collision.transform.position + new Vector3(-1.5f, 6.0f, 0);
                    redSnail.TakeDamage(secondPoisition, damage, isCritical, luckySeven, secondSuriken);
                }
                Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");

                if (luckySeven)
                    luckySeven = false;
                if (secondSuriken)
                    secondSuriken = false;

                Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");
            }

            if (blueSnail != null)
            {
                if (!secondSuriken)
                {
                    Vector3 displayPosition = collision.transform.position + new Vector3(-1.5f, 3.0f, 0);
                    blueSnail.TakeDamage(displayPosition, damage, isCritical, luckySeven, secondSuriken);
                }
                else
                {
                    Vector3 secondPoisition = collision.transform.position + new Vector3(-1.5f, 6.0f, 0);
                    blueSnail.TakeDamage(secondPoisition, damage, isCritical, luckySeven, secondSuriken);
                }
                Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");

                if (luckySeven)
                    luckySeven = false;
                if (secondSuriken)
                    secondSuriken = false;

                Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");
            }

            if (greenMushRoom != null)
            {
                if (!secondSuriken)
                {
                    Vector3 displayPosition = collision.transform.position + new Vector3(-1.5f, 3.0f, 0);
                    greenMushRoom.TakeDamage(displayPosition, damage, isCritical, luckySeven, secondSuriken);
                }
                else
                {
                    Vector3 secondPoisition = collision.transform.position + new Vector3(-1.5f, 6.0f, 0);
                    greenMushRoom.TakeDamage(secondPoisition, damage, isCritical, luckySeven, secondSuriken);
                }
                Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");

                if (luckySeven)
                    luckySeven = false;
                if (secondSuriken)
                    secondSuriken = false;

                Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");
            }
            // �������� Ǯ�� ��ȯ
            shurikenManager.ReturnShurikenToPool(gameObject, shurikenType);
            Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");
        }
    }

    private int CalculateDamage()
    {
        if (luckySeven)
        {
            if (isCriticalHit)
            {
                int minAttack = (int)(DataManager.instance.playerStat.minAttackPower * 1.5f);
                int maxAttack = (int)(DataManager.instance.playerStat.maxAttackPower * 1.5f);
                int skillLevel = SkillManager.instance.skillCollection.skills[0].skillLevel;

                float PerDamage = SkillManager.instance.skillCollection.skills[0].levelEffects[skillLevel].damageIncrease;

                int MinDamage = (int)(minAttack * PerDamage);
                int MaxDamage = (int)(maxAttack * PerDamage);

                return Random.Range(MinDamage * 2, MaxDamage * 2);
            }
            else
            {
                int minAttack = (int)(DataManager.instance.playerStat.minAttackPower * 1.5f);
                int maxAttack = (int)(DataManager.instance.playerStat.maxAttackPower * 1.5f);
                int skillLevel = SkillManager.instance.skillCollection.skills[0].skillLevel;

                float PerDamage = SkillManager.instance.skillCollection.skills[0].levelEffects[skillLevel].damageIncrease;

                int MinDamage = (int)(minAttack * PerDamage);
                int MaxDamage = (int)(maxAttack * PerDamage);

                return Random.Range(MinDamage, MaxDamage);
            }
        }
        else
        {
            if (isCriticalHit)
            {
                int minAttack = (int)(DataManager.instance.playerStat.minAttackPower * 1.5f);
                int maxAttack = (int)(DataManager.instance.playerStat.maxAttackPower * 1.5f);
                return Random.Range(minAttack * 2, maxAttack * 2);
            }
            else
            {
                int minAttack = (int)(DataManager.instance.playerStat.minAttackPower * 1.5f);
                int maxAttack = (int)(DataManager.instance.playerStat.maxAttackPower * 1.5f);
                return Random.Range(minAttack, maxAttack);
            }
        }
    }

    private bool CheckForCriticalHit()
    {
        isCriticalHit = Random.value < DataManager.instance.nowPlayer.criticalProbability / 100;
        return isCriticalHit;
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
