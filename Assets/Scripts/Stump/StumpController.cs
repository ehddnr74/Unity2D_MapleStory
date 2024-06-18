using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StumpController : MonoBehaviour
{
    private string monsterName = "������";
    public Transform nameTagPosition; // �����±׸� ǥ���� ��ġ (��: ������ �Ӹ� ��)
    private GameObject nameTagInstance; // ������ �����±� �ν��Ͻ�
    private TextMeshProUGUI nameTagText; // �����±��� �ؽ�Ʈ ������Ʈ
    private NameTagPool nameTagPool; // �����±� Ǯ�� �ý���

    public Transform hpBarPosition; // HP �ٸ� ǥ���� ��ġ (��: ������ �Ӹ� ��)
    private GameObject hpBarInstance; // ������ HP �� �ν��Ͻ�
    private Slider hpSlider; // HP ���� Slider ������Ʈ
    private HpBarPool hpBarPool; // HP �� Ǯ�� �ý���

    public enum StumpState
    {
        Stand,
        Move,
        Hit,
        Die,
    }

    private DamageTextManager damageTextManager;

    public ItemPool itemPool; // ������ Ǯ ����
    public StumpState mStumpState;
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

    public Transform leftBoundary; // ���� ���
    public Transform rightBoundary; // ������ ���

    private int experience = 6; // ȹ�� ����ġ��

    public bool onceAddExperience = true;

    private void OnEnable()
    {
        col = GetComponent<Collider2D>();
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        damageTextManager = FindObjectOfType<DamageTextManager>();
        monsterSpawner = GameObject.Find("StumpSpawner").GetComponent<MonsterSpawner>();
        itemDataBase = FindObjectOfType<ItemDataBase>();
        itemPool = GameObject.Find("ItemPoolManager").GetComponent<ItemPool>();

        mStumpState = StumpState.Stand;

        hpBarPool = GameObject.Find("HpBarCanvas").GetComponent<HpBarPool>();
        nameTagPool = GameObject.Find("NameTagCanvas").GetComponent<NameTagPool>();
        currentHealth = maxHealth;

        // HP �� �ν��Ͻ� ���� �� ĵ������ �߰�
        hpBarInstance = hpBarPool.GetHPBar();
        hpSlider = hpBarInstance.GetComponent<Slider>();
        hpSlider.maxValue = maxHealth;
        hpSlider.value = currentHealth;

        hitCheck = false;
        dieCheck = false;

        // ��� ������Ʈ�� �������� ����
        leftBoundary = GameObject.Find("StumpLeftBoundary").transform;
        rightBoundary = GameObject.Find("StumpRightBoundary").transform;


        // �����±� Ǯ�� �ý��� �ʱ�ȭ
        nameTagInstance = nameTagPool.GetNameTag();
        nameTagText = nameTagInstance.GetComponentInChildren<TextMeshProUGUI>();
        nameTagText.text = monsterName; // ���� �̸� ����

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
            if (!luckySeven || (luckySeven && !secondSuriken))
            {
                dieCheck = true;
            }
        }
        else
        {
            hitCheck = true;
        }
        hpSlider.value = currentHealth;
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

        switch (mStumpState)
        {
            case StumpState.Stand:
                stand();
                break;

            case StumpState.Move:
                move();
                break;

            case StumpState.Hit:
                hit();
                break;

            case StumpState.Die:
                die();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (mStumpState == StumpState.Stand)
        {
            mRigidBody.velocity = Vector2.zero;
        }

        if (mStumpState == StumpState.Move)
        {
            mRigidBody.velocity = new Vector2(moveDir * moveSpeed, mRigidBody.velocity.y);
        }
    }

    private void stand()
    {
        // ������ ���� ��ġ�� ��踦 �Ѿ�� �ݴ� �������� �̵�
        if (!isChangingDirection)
        {
            if (transform.position.x >= rightBoundary.position.x || transform.position.x <= leftBoundary.position.x)
            {
                moveDir *= -1f; // �̵� ������ �ݴ�� ����
                SetSpriteDir(moveDir);
                isChangingDirection = true; // ���� ���� �� �÷��׸� ����
            }
        }

        // ���� ���� ���̰�, ������ x ��ǥ�� �ٽ� ���� ���� ������ ���� �� �÷��׸� ����
        if (isChangingDirection && transform.position.x <= (rightBoundary.position.x - 1f) && transform.position.x >= (leftBoundary.position.x + 1f))
        {
            isChangingDirection = false;
        }

        // ���� �ð� ��� �� �̵� ���·� ��ȯ
        if (Time.time - stateChangeTime >= idleTime)
        {
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetBool("IsMoving", true);
            mStumpState = StumpState.Move;
            SetMoveDir();
            stateChangeTime = Time.time;
        }

        if (hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetTrigger("IsHitting");
            mStumpState = StumpState.Hit;
        }

        if (dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetBool("IsDying", true);
            mStumpState = StumpState.Die;
        }
    }

    private void move()
    {
        // �̵� �ð��� ������ ��� ���·� ��ȯ
        if (Time.time - stateChangeTime >= moveTime)
        {
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetBool("IsStanding", true);
            mStumpState = StumpState.Stand;
            stateChangeTime = Time.time;
        }
        // ������ ���� ��ġ�� ��踦 �Ѿ�� �ݴ� �������� �̵�
        if (!isChangingDirection)
        {
            if (transform.position.x > rightBoundary.position.x || transform.position.x < leftBoundary.position.x)
            {
                moveDir *= -1f; // �̵� ������ �ݴ�� ����
                SetSpriteDir(moveDir);
                isChangingDirection = true; // ���� ���� �� �÷��׸� ����
            }
        }

        // ���� ���� ���̰�, ������ x ��ǥ�� �ٽ� ���� ���� ������ ���� �� �÷��׸� ����
        if (isChangingDirection && transform.position.x <= (rightBoundary.position.x - 1f) && transform.position.x >= (leftBoundary.position.x + 1f))
        {
            isChangingDirection = false;
        }


        if (hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetTrigger("IsHitting");
            mStumpState = StumpState.Hit;
        }

        if (dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetBool("IsDying", true);
            mStumpState = StumpState.Die;
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
            mStumpState = StumpState.Stand;
        }
        else
        {
            mAnimator.SetBool("IsDying", true);
            mStumpState = StumpState.Die;
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
        yield return new WaitForSeconds(1.0f);
        col.enabled = true;
        onceAddExperience = true;
        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mStumpState = StumpState.Stand;
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
}
