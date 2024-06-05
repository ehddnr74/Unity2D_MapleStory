using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    private static Player instance;
    public enum PlayerState
    {
        Idle,
        Walk,
        Jump,
        Attack,
        ProneStab,
        Ladder,

    }

    FeetScript feetScript;

    public PlayerState mPlayerState;

    Collider2D col;
    Rigidbody2D mRigidBody;
    Animator mAnimator;

    public float dir;
    public float moveSpeed = 3f;
    public float ladderMoveSpeed = 4f;
    public float jumpForce = 7f;
    public bool isGround;
    private Ladder currentLadder;
    public bool isLadder;
    public bool isLaddering;
    private bool LadderPosition; // 캐릭터가 사다리 탈 때 Ladder기준 포지션 정렬

    float proneAttackTime = 0f;
    bool proneAttack = true;

    private float raycastDistance = 50f; // 레이캐스트 거리
    private float fallThroughDuration = 0.2f; // 내려가는 동안 충돌 비활성화 시간

    //private Collider2D feetCollider;

    // Start is called before the first frame update

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
        }
        else if (dir > 0) // 오른쪽 이동
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    isGround = true;
        //}

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
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
    }


    // Player Fsm Funtion
    private void idle()
    {
        if (dir != 0)  // To Walk
            mPlayerState = PlayerState.Walk;
        else
            mAnimator.SetBool("IsWalking", false);

        if (Input.GetKeyDown(KeyCode.LeftAlt) && isGround) // To Jump
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

        if (Input.GetKeyDown(KeyCode.LeftControl)) // To Attack
        {
            mAnimator.SetTrigger("IsAttack");
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

        if (Input.GetKeyDown(KeyCode.LeftAlt) && isGround) // To Jump
        {
            isGround = false;
            mRigidBody.velocity = new Vector2(mRigidBody.velocity.x, jumpForce);
            mAnimator.SetBool("IsJumping", true);
            mPlayerState = PlayerState.Jump;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) // To Prone
        {
            mRigidBody.velocity = new Vector2(0f, 0f); // FixedUpdate에서 한 프레임 돌아서 미끄러지듯이 엎드림 방지
            mPlayerState = PlayerState.ProneStab;
            mAnimator.SetBool("IsProne", true);
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
        }
    }

    private void attack()
    {

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


        if (Input.GetKeyDown(KeyCode.LeftControl) && proneAttack)
        {
            proneAttack = false;
            proneAttackTime = 0f;
            mAnimator.SetTrigger("IsProneAttack");
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt) && IsGroundBelow())
        {
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

    // Get Set
    public bool GetIsLaddering() { return isLaddering; }

    public void SetIsOnLadder(Ladder ladder) { this.currentLadder = ladder; }
    public void SetIsGround(bool isground) { isGround = isground; }



}
