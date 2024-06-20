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
    public static Transform firstTarget; // ù ��° ǥâ�� Ÿ�� ����
    public float trackingRange; // ���� ����

    private bool isCriticalHit; //ũ��Ƽ�� ���� 
    private bool hasHit; // ���� �浹�ߴ��� ����

    private AudioSource audioSource;
    public AudioClip luckySevenHitSound;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        shurikenManager = FindObjectOfType<ShurikenManager>();

        direction = player.flipX;
        hasHit = false; // �浹 �ʱ�ȭ

        if (!secondSuriken)
        {
            // ù ��° ǥâ �߻� �� ���� �� �� ����
            CheckForEnemiesInRange();
            firstTarget = target; // ù ��° ǥâ�� Ÿ�� ����
        }
        else
        {
            // �� ��° ǥâ�� ù ��° ǥâ�� Ÿ���� ���
            target = firstTarget;
        }

        // ǥâ �߻� �� ���� �� �� ����
       // CheckForEnemiesInRange();

        // ���� �ð��� ���� �� �������� Ǯ�� ��ȯ�ϴ� �ڷ�ƾ ����
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    void OnDisable()
    {
        target = null;
        if (!secondSuriken)
        {
            firstTarget = null; // ù ��° ǥâ�� ��Ȱ��ȭ�Ǹ� Ÿ�� �ʱ�ȭ
        }
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
        Vector2 boxSize = new Vector2(trackingRange, trackingRange / 3); // �� ���̷� �þ� ���� ����

        // �÷��̾� ��ġ���� ��¦ ������ �Ǵ� ���ʿ� Ž�� ������ ������ ���
        Vector2 offset = direction ? new Vector2(8f, -2f): new Vector2(-8f, -2f);
        Vector2 startPosition = (Vector2)player.transform.position + offset;

        // �簢�� ���� ���� ���� ����
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(startPosition + directionVector * trackingRange / 3, boxSize, 0);

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
        if (hasHit)
            return; // �̹� �浹�� ��� �߰� ó���� ����

        if (collision.CompareTag("Enemy"))
        {
            hasHit = true;

            criticalProbability = DataManager.instance.nowPlayer.criticalProbability;
            bool isCritical = CheckForCriticalHit(); // ��: ũ��Ƽ�� ��Ʈ�� Ȯ���ϴ� �Լ�
            // ������ ��� ���� �߰�
            int damage = CalculateDamage(); // ��: �������� ����ϴ� �Լ�

            // �浹�� ��ü�� RedSnailController�� ������ �ִ��� Ȯ���ϰ� �������� �ݴϴ�.
            RedSnailController redSnail = collision.GetComponent<RedSnailController>();
            BlueSnailController blueSnail = collision.GetComponent<BlueSnailController>();
            GreenMushRoomController greenMushRoom = collision.GetComponent<GreenMushRoomController>();
            OrangeMushRoomController orangeMushRoom = collision.GetComponent<OrangeMushRoomController>();
            StumpController stump = collision.GetComponent<StumpController>();
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

            if (orangeMushRoom != null)
            {
                if (!secondSuriken)
                {
                    Vector3 displayPosition = collision.transform.position + new Vector3(-1.5f, 3.0f, 0);
                    orangeMushRoom.TakeDamage(displayPosition, damage, isCritical, luckySeven, secondSuriken);
                }
                else
                {
                    Vector3 secondPoisition = collision.transform.position + new Vector3(-1.5f, 6.0f, 0);
                    orangeMushRoom.TakeDamage(secondPoisition, damage, isCritical, luckySeven, secondSuriken);
                }
                Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");

                if (luckySeven)
                    luckySeven = false;
                if (secondSuriken)
                    secondSuriken = false;

                Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");
            }

            if (stump != null)
            {
                if (!secondSuriken)
                {
                    Vector3 displayPosition = collision.transform.position + new Vector3(-1.5f, 3.0f, 0);
                    stump.TakeDamage(displayPosition, damage, isCritical, luckySeven, secondSuriken);
                }
                else
                {
                    Vector3 secondPoisition = collision.transform.position + new Vector3(-1.5f, 6.0f, 0);
                    stump.TakeDamage(secondPoisition, damage, isCritical, luckySeven, secondSuriken);
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
            //Debug.Log($"Hit enemy with luckySeven: {luckySeven}, secondSuriken: {secondSuriken}");
            // �������� Ǯ�� ��ȯ
           // StartCoroutine(PlaySoundAndReturn(luckySevenHitSound));
        }
    }

    private IEnumerator PlaySoundAndReturn(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            audioSource.volume = 0.2f;
            yield return new WaitForSeconds(clip.length);
        }
        // Ǯ�� ��ȯ
        shurikenManager.ReturnShurikenToPool(gameObject, shurikenType);
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

        // �÷��̾� ��ġ���� ��¦ ������ �Ǵ� ���ʿ� Ž�� ������ ������ ���
        Vector2 offset = direction ? new Vector2(8f, -2f) : new Vector2(-8f, -2f);
        Vector3 startPosition = (Vector3)player.transform.position + new Vector3(offset.x,offset.y,0f);
        Vector2 boxSize = new Vector2(trackingRange, trackingRange / 3);
        Gizmos.DrawWireCube(startPosition + (Vector3)directionVector * trackingRange / 3, boxSize);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
