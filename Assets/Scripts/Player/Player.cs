using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    private static Player instance;
    private DamageTextManager damageTextManager;

    public GameObject surikenManagerObject;
    private ShurikenManager surikenManager;
    private QuickSlot quickSlot; 


    public enum PlayerState
    {
        Idle,
        Walk,
        Jump,
        Attack,
        ProneStab,
        Ladder,
        JumpAttack,
        Hit,
    }

    FeetScript feetScript;

    public PlayerState mPlayerState;

    Collider2D col;
    Rigidbody2D mRigidBody;
    Animator mAnimator;

    public float dir;
    public float moveSpeed = 10f;
    public float ladderMoveSpeed = 4f;
    public float jumpForce = 25f;
    public float criticalProbability = 0f; //ũ��Ƽ�� Ȯ��
    public bool isGround;
    private Ladder currentLadder;
    public bool isLadder;
    public bool isLaddering;
    private bool LadderPosition; // ĳ���Ͱ� ��ٸ� Ż �� Ladder���� ������ ����

    public bool attachDroppedItem; // �����۰� ��Ҵ��� ����
    public bool pickable; //�������� �ֿ� �� �ִ� ���� ��ȯ ���� 

    float AttackTime = 0f;
    float JumpAttackTime = 0f;
    public bool isAttacking;

    float proneAttackTime = 0f;
    bool proneAttack = true;

    private float raycastDistance = 50f; // ����ĳ��Ʈ �Ÿ�
    private float fallThroughDuration = 0.2f; // �������� ���� �浹 ��Ȱ��ȭ �ð�

    public bool canAttack = true;
    public float attackCoolDown = 0.6f;

    public bool flipX = false;

    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    public bool invincibel = false; // ����
    public float invincibilityPeriod = 2f; // �����ð�

    public bool toJump;
    public bool toAttack;


    public bool canDoubleJump; // ���� ���� ���� ���� ���� ����
    public float doubleJumpForce = 5f;
    public float jumpTime;

    public bool doubleJumping;


    private float hitTime;

    public int currentSuriken = 6;

    public bool luckySeven; // ��Ű���� ��뿩��

    public bool SecondSuriken;





    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        col = GetComponent<Collider2D>();
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mPlayerState = PlayerState.Idle;

        feetScript = GetComponentInChildren<FeetScript>();
        surikenManager = surikenManagerObject.GetComponent<ShurikenManager>();
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
        damageTextManager = FindObjectOfType<DamageTextManager>();
        //feetCollider = transform.Find("FeetCollider").GetComponent<Collider2D>(); // �� �ݶ��̴��� ã��
    }

    // Update is called once per frame
    void Update()
    {
        SetSpriteDir(dir); // dir�� ���� ��������Ʈ ���� �Լ�

        switch (mPlayerState)
        {
            case PlayerState.Idle:
                idle();
                break;

            case PlayerState.Walk:
                walk();
                break;

            case PlayerState.Jump:
                jump();
                break;

            case PlayerState.Attack:
                attack();
                break;

            case PlayerState.ProneStab:
                prone();
                break;

            case PlayerState.Ladder:
                ladder();
                break;

            case PlayerState.JumpAttack:
                jumpAttack();
                break;

            case PlayerState.Hit:
                hit();
                break;
        }

        if (isAttacking)
        {
            if (AttackTime > 0.5f)
            {
                isAttacking = false;
                AttackTime = 0f;
            }

            AttackTime += Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (mPlayerState == PlayerState.Ladder)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (isGround)
                {
                    isLadder = false;
                }
            }
            return;
        }

        if (mPlayerState == PlayerState.Attack)
            return;

        if (isGround)
        {
            if (mPlayerState != PlayerState.ProneStab) // ProneStab �߿��� �������� ��������
            {
                dir = Input.GetAxisRaw("Horizontal");
                mRigidBody.velocity = new Vector2(dir * moveSpeed, mRigidBody.velocity.y);
            }
        }


    }

    private void SetSpriteDir(float dir)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (dir < 0) // ���� �̵�
        {
            spriteRenderer.flipX = false;
            flipX = false;
        }
        else if (dir > 0) // ������ �̵�
        {
            spriteRenderer.flipX = true;
            flipX = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }
    private IEnumerator Hitting()
    {
        yield return new WaitForSeconds(invincibilityPeriod);
        invincibel = false;
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            timer += Time.deltaTime;
            mRigidBody.velocity = direction * knockbackForce;
            yield return null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            if (mPlayerState == PlayerState.Idle)
            {
                isLadder = true;
            }
        }

        if (collision.CompareTag("DroppedItem"))
        {
            attachDroppedItem = true;
            if (pickable)
            {
                pickable = false;
                PickupItem(collision.gameObject);
            }
        }

        if (!invincibel && collision.gameObject.CompareTag("Enemy"))
        {
            int damageAmount = CalculateDamageFromEnemy(collision.gameObject); // �����κ��� �Դ� ������ ���
            ShowPlayerDamage(damageAmount, collision.transform.position); // ������ �ؽ�Ʈ ǥ��

            mPlayerState = PlayerState.Hit;
            mAnimator.SetBool("IsHitting",true);
            invincibel = true;
            StartCoroutine(Hitting());
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(ApplyKnockback(knockbackDirection));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
        }
        //    if (collision.gameObject.CompareTag("Ground"))
        //    {

        //        isGround = false; // ���� ���� ����
        //        Debug.Log("��������");
        //    }

        if (collision.CompareTag("DroppedItem"))
        {
            attachDroppedItem = false;
        }
    }

    private void PickupItem(GameObject itemObject)
    {
        DropItemData dropItemData = itemObject.GetComponent<DropItemData>();
        if (dropItemData != null)
        {
            Item item = dropItemData.item;

            Inventory inventory = FindObjectOfType<Inventory>(); // �κ��丮 �Ŵ����� ã�Ƽ� ���

            if (inventory != null)
            {
                if (item.Type != "Meso")
                {
                    inventory.AddItem(item.ID); // �κ��丮�� ������ �߰�
                }
                else
                {
                    DataManager.instance.AddMeso(UnityEngine.Random.Range(100, 300));
                }
            }
            // ������ ������Ʈ ����
            Destroy(itemObject);
        }
    }



private void hit()
    {
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) // HitAnimation ����߿� Animation�� �ٲ㼭 ������ �� �ֵ���
        {
            mPlayerState = PlayerState.Walk;
            mAnimator.SetBool("IsHitting", false);
            hitTime = 0f;
        }

        if(hitTime >= 2.0f) // 2�� ���� HitAnimation ��� 
        {
            mPlayerState = PlayerState.Idle;
            mAnimator.SetBool("IsHitting", false);
            hitTime = 0f;
        }
        hitTime += Time.deltaTime;
    }

    // Player Fsm Funtion
    private void idle()
    {
        if (dir != 0)  // To Walk
            mPlayerState = PlayerState.Walk;
        else
            mAnimator.SetBool("IsWalking", false);

        if (toJump)
        {
            isGround = false;
            mRigidBody.velocity = Vector2.up * jumpForce;
            mAnimator.SetBool("IsJumping", true);
            mPlayerState = PlayerState.Jump;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) // To Prone
        {
            mPlayerState = PlayerState.ProneStab;
            mAnimator.SetBool("IsProne", true);
        }

        if(toAttack)
        {
            toAttack = false;

            if (quickSlot.ExistSuriken && !luckySeven)
            {
                NormalAttack();
            }
            else if (quickSlot.ExistSuriken && luckySeven)
            {
                luckySeven = false;
                StartCoroutine(LuckySeven());
            }

            isAttacking = true;
            mAnimator.SetTrigger("IsAttack");
            mPlayerState = PlayerState.Attack;
            StartCoroutine(AttackCooldown());
        }

        if(isLadder) // �Ʒ����� ���� �ö󰡱�
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isLaddering = true;
                mPlayerState = PlayerState.Ladder;
            }
        }
        
        //if(isLadder && isGround) // ������ �Ʒ� ��������� ���������� �־ ����? 
        //{
        //    if(Input.GetKeyDown(KeyCode.DownArrow))
        //    {
        //        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);
        //        isLaddering = true;
        //        mPlayerState = PlayerState.Ladder;
        //    }
        //}
    }


    private void walk()
    {
        if (dir == 0) // To Walk
            mPlayerState = PlayerState.Idle;
        else
            mAnimator.SetBool("IsWalking", true);

        if (toJump) // To Jump
        {
            isGround = false;
            mRigidBody.velocity = new Vector2(mRigidBody.velocity.x, jumpForce);
            mAnimator.SetBool("IsJumping", true);
            mPlayerState = PlayerState.Jump;
        }

        if (toAttack) // To Attack
        {
            toAttack = false;

            if (quickSlot.ExistSuriken && !luckySeven)
            {
                NormalAttack();
            }
            else if (quickSlot.ExistSuriken && luckySeven)
            {
                luckySeven = false;
                StartCoroutine(LuckySeven());
            }

            isAttacking = true;
            mAnimator.SetTrigger("IsAttack");
            mPlayerState = PlayerState.Attack;
            StartCoroutine(AttackCooldown());
        }
    }

    private void jump()
    {
        if (isGround)
        {
            if (dir != 0)
                mPlayerState = PlayerState.Walk;
            else
                mPlayerState = PlayerState.Idle;

            mAnimator.SetBool("IsJumping", false);

            JumpAttackTime = 0f;
            //jumpTime = 0f;

            toJump = false;
            doubleJumping = false;
        }

        //jumpTime += Time.deltaTime;

        if (!isGround && canDoubleJump)
        {
            float doubleJumpVelocityX = 25f;
            float currentYVelocity = mRigidBody.velocity.y;
            float adjustedDoubleJumpForce = doubleJumpForce;
            float doubleJumpFallMultiplier = 1.2f; // ���� ���� �� �ϰ� �ӵ� ����

            if (currentYVelocity < 0)
            {
                adjustedDoubleJumpForce += currentYVelocity * doubleJumpFallMultiplier; // �ʿ信 ���� ���� �����ϼ���
            }
            canDoubleJump = false;
            mRigidBody.velocity = new Vector2(dir * doubleJumpVelocityX, adjustedDoubleJumpForce);
        }


        if (JumpAttackTime < 0.15f && toAttack) // To Attack
        {
            toAttack = false;

            if (quickSlot.ExistSuriken && !luckySeven)
            {
                NormalAttack();
            }
            else if (quickSlot.ExistSuriken && luckySeven)
            {
                luckySeven = false;
                StartCoroutine(LuckySeven());               
            }


            isAttacking = true;
            mAnimator.SetTrigger("IsAttack");
            mPlayerState = PlayerState.JumpAttack;
            JumpAttackTime = 0f;
        }

        JumpAttackTime += Time.deltaTime;
    }

    private void jumpAttack()
    {
        if (isGround)
        {
            mPlayerState = PlayerState.Idle;
            mAnimator.SetBool("IsJumping", false);
            toJump = false;
        }
    }

    private void attack()
    {
        if (!isAttacking) 
            mPlayerState = PlayerState.Idle;
    }

    private IEnumerator LuckySeven()
    {
        AttackSurken();
        yield return new WaitForSeconds(0.1f);
        SecondAttackSuriken();
    }

    public void NormalAttack()
    {
        StartCoroutine(NormalAttackSurikenCoroutine(currentSuriken));
    }

    private IEnumerator NormalAttackSurikenCoroutine(int itemId)
    {
        yield return new WaitForSeconds(0.3f); // Wait for 1 second
        GameObject suriken = surikenManager.GetShurikenFromPool(itemId, transform.position + Vector3.right * (GetComponent<SpriteRenderer>().flipX ? 4 : -4));
        suriken.SetActive(true); // ������ Ȱ��ȭ
    }

    public void AttackSurken()
    {
        StartCoroutine(AttackSurikenCoroutine(currentSuriken));
    }
    private IEnumerator AttackSurikenCoroutine(int itemId)
    {
        yield return new WaitForSeconds(0.3f); // Wait for 1 second
        GameObject suriken = surikenManager.GetShurikenFromPool(itemId,transform.position + Vector3.right * (GetComponent<SpriteRenderer>().flipX ? 4 : -4));
        suriken.SetActive(true); // ������ Ȱ��ȭ
        Shuriken shurikenScript = suriken.GetComponent<Shuriken>();
        shurikenScript.luckySeven = true;
    }

    public void SecondAttackSuriken() // ��Ű������ ���� ���� �ڷ�ƾ (SecondSuriken)
    {
        StartCoroutine(SecondSurikenCoroutine(currentSuriken));
    }
    private IEnumerator SecondSurikenCoroutine(int itemId)
    {
        yield return new WaitForSeconds(0.00001f);
        GameObject suriken = surikenManager.GetShurikenFromPool(itemId, transform.position + Vector3.right * (GetComponent<SpriteRenderer>().flipX ? 4 : -4));
        suriken.SetActive(true); // ������ Ȱ��ȭ
        Shuriken shurikenScript = suriken.GetComponent<Shuriken>();
        shurikenScript.luckySeven = true;
        shurikenScript.secondSuriken = true;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false; // Disable attacking
        yield return new WaitForSeconds(attackCoolDown); // Wait for 1 second
        canAttack = true; // Enable attacking again
    }

    private void prone()
    {
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            mPlayerState = PlayerState.Idle;
            mAnimator.SetBool("IsProne", false);
        }

        if (proneAttack == false) // ���� ���� ���� 
        {
            proneAttackTime += Time.deltaTime;

            if (proneAttackTime >= 1f)
                proneAttack = true;
        }


        if (toAttack && proneAttack)
        {
            proneAttack = false;
            proneAttackTime = 0f;
            mAnimator.SetTrigger("IsProneAttack");
        }

        if (toJump && IsGroundBelow())
        {
            toJump = false;
            StartCoroutine(FallThroughPlatform());
        }
    }

    private void ladder()
    {
        if (isLadder)
        {
            if (!LadderPosition)
            {
                LadderPosition = true;
                Vector3 ladderPosition = currentLadder.GetLadderPosition();
                transform.position = new Vector3(ladderPosition.x, transform.position.y +0.1f, transform.position.z);
            }

            mRigidBody.velocity = Vector2.zero;
            mRigidBody.gravityScale = 0;

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                mAnimator.SetBool("IsLadder", true);
                float ver = Input.GetAxis("Vertical");
                mRigidBody.velocity = new Vector2(mRigidBody.velocity.x, ver * ladderMoveSpeed);
            }
        }
        else
        {
            ExitLadder();
        }
    }

    private bool IsGroundBelow()
    {
        float offset = 5f; // ���÷� 0.1f�� ����

        // ����ĳ��Ʈ�� ���� ��ġ ���
        Vector2 raycastStartPosition = (Vector2)transform.position - Vector2.up * offset;
        // Ground ���̾ �����ϵ��� ����
        RaycastHit2D rayHit = Physics2D.Raycast(raycastStartPosition, Vector2.down, raycastDistance, LayerMask.GetMask("Ground"));
        return rayHit.collider != null;
    }

    private IEnumerator FallThroughPlatform()
    {
        // �÷��̾�� Ground ���̾� ���� �浹 ���� ����
        
        feetScript.DisableCollider();
        //Physics2D.IgnoreLayerCollision(13, LayerMask.NameToLayer("Ground"), true);
        yield return new WaitForSeconds(fallThroughDuration);
        feetScript.EnableCollider();
       // Physics2D.IgnoreLayerCollision(13, LayerMask.NameToLayer("Ground"), false);

        mAnimator.SetBool("IsProne", false);
        mPlayerState = PlayerState.Jump;
        mAnimator.SetBool("IsJumping", true);

    }


    private void ExitLadder()
    {
        LadderPosition = false;
        mRigidBody.gravityScale = 8f;
        isLaddering = false;
        mAnimator.SetBool("IsLadder", false);
        mPlayerState = PlayerState.Idle;
    }

    private void ShowPlayerDamage(int damageAmount, Vector3 enemyPosition)
    {
        // �÷��̾��� ���� ��ġ�� ������ �ؽ�Ʈ ǥ��
        Vector3 damageTextPosition = transform.position + new Vector3(-1f, 4.2f, 0); // �÷��̾� ���� �ؽ�Ʈ ǥ��
        damageTextManager.ShowPlayerDamage(damageTextPosition, damageAmount);

        DataManager.instance.RemoveHP(damageAmount);
    }

    private int CalculateDamageFromEnemy(GameObject enemy)
    {
        return UnityEngine.Random.Range(10,99);
    }

    // Get Set
    public bool GetIsLaddering() { return isLaddering; }

    public void SetIsOnLadder(Ladder ladder) { this.currentLadder = ladder; }
    public void SetIsGround(bool isground) { isGround = isground; }

    public float GetDir() { return dir; }



}
