using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using static RedSnailController;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using Unity.Mathematics;
using TMPro;


public class RedSnailController : MonoBehaviour
{

    private string monsterName = "���� ������";
    public Transform nameTagPosition; // �����±׸� ǥ���� ��ġ (��: ������ �Ӹ� ��)
    private GameObject nameTagInstance; // ������ �����±� �ν��Ͻ�
    private TextMeshProUGUI nameTagText; // �����±��� �ؽ�Ʈ ������Ʈ
    private NameTagPool nameTagPool; // �����±� Ǯ�� �ý���

    public Transform hpBarPosition; // HP �ٸ� ǥ���� ��ġ (��: ������ �Ӹ� ��)
    private GameObject hpBarInstance; // ������ HP �� �ν��Ͻ�
    private Slider hpSlider; // HP ���� Slider ������Ʈ
    private HpBarPool hpBarPool; // HP �� Ǯ�� �ý���


    private AudioSource audioSource;
    public AudioClip HitSound;
    public AudioClip DieSound;

    public enum RedSnailState
    {
        Stand,
        Move,
        Hit,
        Die,
    }

    private DamageTextManager damageTextManager;


    public ItemPool itemPool; // ������ Ǯ ����
    public RedSnailState mRedSnailState;
    private MonsterSpawner monsterSpawner; // ���� ������ ����
    private ItemDataBase itemDataBase; // ������ �����ͺ��̽� ����


    Collider2D col;
    Rigidbody2D mRigidBody;
    Animator mAnimator;

    public float moveTime = 3f; // �̵� �ð�
    public float idleTime = 1f; // ��� �ð�
    public float moveSpeed = 3f;
    private float stateChangeTime;
    private float moveDir;

    private bool isChangingDirection = false; // ���� ���� ������ ���θ� ��Ÿ���� �÷���

    public int maxHealth = 500; // �ִ� ü��
    private int currentHealth;

    private bool hitCheck;
    private bool dieCheck;

    public List<DropItem> dropTable; // ��� ���̺� �߰�
    public GameObject itemPrefab; // ������ ������

    private int experience = 8; // ȹ�� ����ġ��

    public bool onceAddExperience = true;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        col = GetComponent<Collider2D>();
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        damageTextManager = FindObjectOfType<DamageTextManager>();
        monsterSpawner = GameObject.Find("RedSnailSpawner").GetComponent<MonsterSpawner>();
        itemDataBase = FindObjectOfType<ItemDataBase>();
        itemPool = GameObject.Find("ItemPoolManager").GetComponent<ItemPool>();

        mRedSnailState = RedSnailState.Stand;

        hpBarPool = GameObject.Find("HpBarCanvas").GetComponent<HpBarPool>();
        nameTagPool = GameObject.Find("NameTagCanvas").GetComponent<NameTagPool>();

        currentHealth = maxHealth;

        // HP �� �ν��Ͻ� ���� �� ĵ������ �߰�
        hpBarInstance = hpBarPool.GetHPBar();
        hpSlider = hpBarInstance.GetComponent<Slider>();
        hpSlider.maxValue = maxHealth;
        hpSlider.value = currentHealth;

        // �����±� Ǯ�� �ý��� �ʱ�ȭ
        nameTagInstance = nameTagPool.GetNameTag();
        nameTagText = nameTagInstance.GetComponentInChildren<TextMeshProUGUI>();
        nameTagText.text = monsterName; // ���� �̸� ����

        hitCheck = false;
        dieCheck = false;

        SetMoveDir();
        stateChangeTime = Time.time;
    }
    private void OnDisable()
    {
        if (hpBarInstance != null)
        {
            hpBarPool.ReturnHPBar(hpBarInstance);
            hpBarInstance = null;
        }

        if (nameTagInstance != null)
        {
            nameTagPool.ReturnNameTag(nameTagInstance);
            nameTagInstance = null;
        }
    }



    public void TakeDamage(Vector3 Position, int damage, bool isCritical, bool luckySeven, bool secondSuriken)
    {
        damageTextManager.ShowDamage(Position, damage, isCritical);
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (luckySeven)
            {
                if (!secondSuriken)
                {
                    // ù ��° ǥâ�� �¾��� �� ������ �ʵ��� ����
                    StartCoroutine(WaitForSecondShuriken());
                    PlaySound(DieSound, 0.2f);
                }
                else
                {
                    // �� ��° ǥâ�� �¾��� ���� ���� �׵��� ó��
                    StartCoroutine(LateDie());
                    PlaySound(DieSound, 0.2f);
                }
            }
            else
            {
                // ��Ű������ �ƴ� ��쿡�� ù ��° ǥâ�� �¾��� �� �׵��� ó��
                dieCheck = true;
                PlaySound(DieSound, 0.2f);
            }
        }
        else
        {
            hitCheck = true;
            PlaySound(HitSound, 0.2f);
        }
            hpSlider.value = currentHealth;
    }

    private IEnumerator WaitForSecondShuriken()
    {
        yield return new WaitForSeconds(0.2f); // �� ��° ǥâ�� ���� �ð��� ��ٸ�
        if (currentHealth <= 0)
        {
            dieCheck = true;
        }
    }

    private IEnumerator LateDie()
    {
        yield return new WaitForSeconds(0.2f);

        dieCheck = true;
    }

    private void Update()
    {
        if (hpBarInstance != null)
        {
            // ���� ��ġ�� ���� HP �� ��ġ ������Ʈ
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(hpBarPosition.position);
            hpBarInstance.transform.position = screenPosition;
        }

        if (nameTagInstance != null)
        {
            // ���� ��ġ�� ���� �����±� ��ġ ������Ʈ
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(nameTagPosition.position);
            nameTagInstance.transform.position = screenPosition;
        }

        SetSpriteDir(moveDir);

        switch (mRedSnailState)
        {
            case RedSnailState.Stand:
                stand();
                break;

            case RedSnailState.Move:
                move();
                break;

            case RedSnailState.Hit:
                hit();
                break;

            case RedSnailState.Die:
                die();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (mRedSnailState == RedSnailState.Move)
        {
            mRigidBody.velocity = new Vector2(moveDir * moveSpeed, mRigidBody.velocity.y);
        }
    }

    private void stand()
    {
        // ������ ���� ��ġ�� x ��ǥ 46.2�� �Ѿ�� �ݴ� �������� �̵�
        if (transform.position.x >= 46.2f || transform.position.x <= -76.17f)
        {
            moveDir *= -1f; // �̵� ������ �ݴ�� ����
            SetSpriteDir(moveDir);
            isChangingDirection = true; // ���� ���� �� �÷��׸� ����
        }

        // ���� ���� ���̰�, ������ x ��ǥ�� �ٽ� ���� ���� ������ ���� �� �÷��׸� ����
        if (isChangingDirection && transform.position.x <= 45.2f && transform.position.x >= -75.17f)
        {
            isChangingDirection = false;
        }


        // ���� �ð� ��� �� �̵� ���·� ��ȯ
        if (Time.time - stateChangeTime >= idleTime)
        {
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetBool("IsMoving", true);
            mRedSnailState = RedSnailState.Move;
            SetMoveDir();
            stateChangeTime = Time.time;
        }

        if(hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetTrigger("IsHitting");
            mRedSnailState = RedSnailState.Hit;
        }

        if(dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetBool("IsDying", true);
            mRedSnailState = RedSnailState.Die;
        }
    }

    private void move()
    {
        // �̵� �ð��� ������ ��� ���·� ��ȯ
        if (Time.time - stateChangeTime >= moveTime)
        {
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetBool("IsStanding", true);
            mRedSnailState = RedSnailState.Stand;
            stateChangeTime = Time.time;
        }

        // ������ ���� ��ġ�� x ��ǥ 46.2�� �Ѿ�� �ݴ� �������� �̵�
        if (transform.position.x >= 46.2f || transform.position.x <= -76.17f)
        {
            moveDir *= -1f; // �̵� ������ �ݴ�� ����
            SetSpriteDir(moveDir);
            isChangingDirection = true; // ���� ���� �� �÷��׸� ����
        }

        // ���� ���� ���̰�, ������ x ��ǥ�� �ٽ� ���� ���� ������ ���� �� �÷��׸� ����
        if (isChangingDirection && transform.position.x <= 45.2f && transform.position.x >= -75.17f)
        {
            isChangingDirection = false;
        }

        if (hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetTrigger("IsHitting");
            mRedSnailState = RedSnailState.Hit;
        }

        if (dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetBool("IsDying", true);
            mRedSnailState = RedSnailState.Die;
        }
    }
    private void hit()
    {
        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        mRigidBody.velocity = Vector2.zero; // �ǰ� �߿��� �������� ����
        yield return new WaitForSeconds(0.15f); // �ǰ� �ִϸ��̼� ��� �ð�
        if (currentHealth > 0) // �ǰ� �� ü���� ���� ���� ���� Stand ���·� ��ȯ
        {
            mAnimator.ResetTrigger("IsHitting"); // Ʈ���� �缳��
            mAnimator.SetBool("IsStanding", true);
            mRedSnailState = RedSnailState.Stand;
        }
        else
        {
            mAnimator.SetBool("IsDying", true);
            mRedSnailState = RedSnailState.Die;
        }
    }
    private void die()
    {
        if (onceAddExperience)
        {
            onceAddExperience = false;
            DataManager.instance.AddExperience(experience);
        }
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        col.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        mRigidBody.velocity = Vector2.zero; // �ӵ� �ʱ�ȭ
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
        onceAddExperience = true;
        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mRedSnailState = RedSnailState.Stand;
        currentHealth = maxHealth; // �ʱ� ü���� �ִ� ü������ ����
        DropItems(); // ������ ��� ���� �߰�
        monsterSpawner.DespawnMonster(gameObject); // ���͸� Ǯ�� ��ȯ

    }

    private void DropItems()
    {
            int cnt = 0; // ������ �������� ����������� �ٸ��� �ϱ� ���� ���� 
            if (itemDataBase == null) return;

            foreach (var dropItem in dropTable)
            {
                if (Random.value <= dropItem.dropChance)
                {
                    cnt++;
                    Item item = itemDataBase.FetchItemByID(dropItem.itemId);
                    Debug.Log(item.Name);
                    if (item != null)
                    {
                        Vector3 dropPosition = transform.position + new Vector3(cnt * 1.6f, 0f, 0f);
                        GameObject droppedItem = itemPool.GetItem(dropPosition, Quaternion.identity);
                        DropItemData dropItemData = droppedItem.GetComponent<DropItemData>();
                        if (dropItemData != null)
                        {
                            dropItemData.Initialize(item, itemPool);
                        }
                    }
                }
            }
        }

    private void SetMoveDir()
    {
        moveDir = Random.Range(-1f, 1f);
        SetSpriteDir(moveDir);
    }

    private void SetSpriteDir(float dir)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (dir < 0) // ���� �̵�
        {
            spriteRenderer.flipX = false;
        }
        else if (dir > 0) // ������ �̵�
        {
            spriteRenderer.flipX = true;
        }
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            audioSource.volume = volume;
        }
    }
}
