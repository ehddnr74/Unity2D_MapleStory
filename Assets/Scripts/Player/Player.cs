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
    public float criticalProbability = 0f; //크리티컬 확률
    public bool isGround;
    private Ladder currentLadder;
    public bool isLadder;
    public bool isLaddering;
    private bool LadderPosition; // 캐릭터가 사다리 탈 때 Ladder기준 포지션 정렬

    public bool attachDroppedItem; // 아이템과 닿았는지 여부
    public bool pickable; //아이템을 주울 수 있는 상태 전환 변수 

    float AttackTime = 0f;
    float JumpAttackTime = 0f;
    public bool isAttacking;

    float proneAttackTime = 0f;
    bool proneAttack = true;

    private float raycastDistance = 50f; // 레이캐스트 거리
    private float fallThroughDuration = 0.2f; // 내려가는 동안 충돌 비활성화 시간

    public bool canAttack = true;
    public float attackCoolDown = 0.6f;

    public bool flipX = false;

    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    public bool invincibel = false; // 무적
    public float invincibilityPeriod = 2f; // 무적시간

    public bool toJump;
    public bool toAttack;


    public bool canDoubleJump; // 더블 점프 가능 여부 저장 변수
    public float doubleJumpForce = 5f;
    public float jumpTime;

    public bool doubleJumping;


    private float hitTime;

    public int currentSuriken = 6;

    public bool luckySeven; // 럭키세븐 사용여부

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
        //feetCollider = transform.Find("FeetCollider").GetComponent<Collider2D>(); // 발 콜라이더를 찾음
    }

    // Update is called once per frame
    void Update()
    {
        SetSpriteDir(dir); // dir에 따른 스프라이트 반전 함수

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
            if (mPlayerState != PlayerState.ProneStab) // ProneStab 중에는 연산하지 않을거임
            {
                dir = Input.GetAxisRaw("Horizontal");
                mRigidBody.velocity = new Vector2(dir * moveSpeed, mRigidBody.velocity.y);
            }
        }


    }

    private void SetSpriteDir(float dir)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (dir < 0) // 왼쪽 이동
        {
            spriteRenderer.flipX = false;
            flipX = false;
        }
        else if (dir > 0) // 오른쪽 이동
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
            int damageAmount = CalculateDamageFromEnemy(collision.gameObject); // 적으로부터 입는 데미지 계산
            ShowPlayerDamage(damageAmount, collision.transform.position); // 데미지 텍스트 표시

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

        //        isGround = false; // 착지 상태 해제
        //        Debug.Log("착지해제");
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

            Inventory inventory = FindObjectOfType<Inventory>(); // 인벤토리 매니저를 찾아서 사용

            if (inventory != null)
            {
                if (item.Type != "Meso")
                {
                    inventory.AddItem(item.ID); // 인벤토리에 아이템 추가
                }
                else
                {
                    DataManager.instance.AddMeso(UnityEngine.Random.Range(100, 300));
                }
            }
            // 아이템 오브젝트 삭제
            Destroy(itemObject);
        }
    }



private void hit()
    {
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) // HitAnimation 재생중에 Animation을 바꿔서 움직일 수 있도록
        {
            mPlayerState = PlayerState.Walk;
            mAnimator.SetBool("IsHitting", false);
            hitTime = 0f;
        }

        if(hitTime >= 2.0f) // 2초 동안 HitAnimation 재생 
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

        if(isLadder) // 아래에서 위로 올라가기
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isLaddering = true;
                mPlayerState = PlayerState.Ladder;
            }
        }
        
        //if(isLadder && isGround) // 위에서 아래 내려가기는 하향점프가 있어서 생략? 
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
            float doubleJumpFallMultiplier = 1.2f; // 더블 점프 시 하강 속도 배율

            if (currentYVelocity < 0)
            {
                adjustedDoubleJumpForce += currentYVelocity * doubleJumpFallMultiplier; // 필요에 따라 값을 조정하세요
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
        suriken.SetActive(true); // 수리검 활성화
    }

    public void AttackSurken()
    {
        StartCoroutine(AttackSurikenCoroutine(currentSuriken));
    }
    private IEnumerator AttackSurikenCoroutine(int itemId)
    {
        yield return new WaitForSeconds(0.3f); // Wait for 1 second
        GameObject suriken = surikenManager.GetShurikenFromPool(itemId,transform.position + Vector3.right * (GetComponent<SpriteRenderer>().flipX ? 4 : -4));
        suriken.SetActive(true); // 수리검 활성화
        Shuriken shurikenScript = suriken.GetComponent<Shuriken>();
        shurikenScript.luckySeven = true;
    }

    public void SecondAttackSuriken() // 럭키세븐을 쓰기 위한 코루틴 (SecondSuriken)
    {
        StartCoroutine(SecondSurikenCoroutine(currentSuriken));
    }
    private IEnumerator SecondSurikenCoroutine(int itemId)
    {
        yield return new WaitForSeconds(0.00001f);
        GameObject suriken = surikenManager.GetShurikenFromPool(itemId, transform.position + Vector3.right * (GetComponent<SpriteRenderer>().flipX ? 4 : -4));
        suriken.SetActive(true); // 수리검 활성화
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

        if (proneAttack == false) // 무한 공격 방지 
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
        float offset = 5f; // 예시로 0.1f로 설정

        // 레이캐스트의 시작 위치 계산
        Vector2 raycastStartPosition = (Vector2)transform.position - Vector2.up * offset;
        // Ground 레이어만 감지하도록 설정
        RaycastHit2D rayHit = Physics2D.Raycast(raycastStartPosition, Vector2.down, raycastDistance, LayerMask.GetMask("Ground"));
        return rayHit.collider != null;
    }

    private IEnumerator FallThroughPlatform()
    {
        // 플레이어와 Ground 레이어 간의 충돌 무시 설정
        
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
        // 플레이어의 현재 위치에 데미지 텍스트 표시
        Vector3 damageTextPosition = transform.position + new Vector3(-1f, 4.2f, 0); // 플레이어 위에 텍스트 표시
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
