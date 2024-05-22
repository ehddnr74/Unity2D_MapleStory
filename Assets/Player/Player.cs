using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Jump,
        Attack,
        ProneStab,
    }

    public PlayerState mPlayerState;

    Rigidbody2D mRigidBody;
    Animator mAnimator;

    public float dir;
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    private bool isGround;

    float proneAttackTime = 0f;
    bool proneAttack = true;

    private float raycastDistance = 5f; // ����ĳ��Ʈ �Ÿ�
    private float fallThroughDuration = 0.3f; // �������� ���� �浹 ��Ȱ��ȭ �ð�

    //private Collider2D feetCollider;

    // Start is called before the first frame update
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mPlayerState = PlayerState.Idle;

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

        }
    }

    private void FixedUpdate()
    {
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
        }
        else if (dir > 0) // ������ �̵�
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {

    //        isGround = true; // ���� ���·� ����
    //        Debug.Log("����");
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {

    //        isGround = false; // ���� ���� ����
    //        Debug.Log("��������");
    //    }
    //}


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
            mRigidBody.velocity = new Vector2(0f, 0f); // FixedUpdate���� �� ������ ���Ƽ� �̲��������� ���帲 ����
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

        if (proneAttack == false) // ���� ���� ���� 
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

    private bool IsGroundBelow()
    {
        // Ground ���̾ �����ϵ��� ����
        RaycastHit2D rayHit = Physics2D.Raycast(mRigidBody.position, Vector2.down, raycastDistance, LayerMask.GetMask("Lower Ground"));
        return rayHit.collider != null;
    }

        private IEnumerator FallThroughPlatform()
    {
        // �÷��̾�� Ground ���̾� ���� �浹 ���� ����
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);
        yield return new WaitForSeconds(fallThroughDuration);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);

        mAnimator.SetBool("IsProne", false);
        mPlayerState = PlayerState.Jump;
        mAnimator.SetBool("IsJumping", true);

    }


}
