using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using static RedSnailController;
using Random = UnityEngine.Random;


public class RedSnailController : MonoBehaviour
{
    public enum RedSnailState
    {
        Stand,
        Move,
        Hit,
        Die,
    }

    private DamageTextManager damageTextManager;

    public RedSnailState mRedSnailState;


    Collider2D col;
    Rigidbody2D mRigidBody;
    Animator mAnimator;

    public float moveTime = 3f; // �̵� �ð�
    public float idleTime = 1f; // ��� �ð�
    public float moveSpeed = 3f;
    private float moveStartTime;
    private float moveDir;

    private bool isChangingDirection = false; // ���� ���� ������ ���θ� ��Ÿ���� �÷���


    private void Start()
    {
        col = GetComponent<Collider2D>();
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        damageTextManager = FindObjectOfType<DamageTextManager>();

        mRedSnailState = RedSnailState.Stand;
        SetMoveDir();
    }

    public void TakeDamage(int damage, bool isCritical, bool secondSuriken)
    {
        if (!secondSuriken)
        {
            Vector3 displayPosition = transform.position + new Vector3(-1.5f, 3.0f, 0); // y������ 1.0f ��ŭ �� ���� ����
            damageTextManager.ShowDamage(displayPosition, damage, isCritical); 
        }
        else
        {
            Vector3 secondPoisition = transform.position + new Vector3(-1.5f, 6.0f, 0); 
            damageTextManager.ShowDamage(secondPoisition, damage, isCritical);
        }
        
    }

    private void Update()
    {
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
        // ���� �ð� ��� �� �̵� ���·� ��ȯ
        if (Time.time - moveStartTime >= idleTime)
        {
            mAnimator.SetBool("IsMoving", true);
            mRedSnailState = RedSnailState.Move;
            SetMoveDir();
        }
    }

    private void move()
    {
        // �̵� �ð��� ������ ��� ���·� ��ȯ
        if (Time.time - moveStartTime >= moveTime)
        {
            mAnimator.SetBool("IsStanding", true);
            mRedSnailState = RedSnailState.Stand;
            moveStartTime = Time.time;
        }

        // ������ ���� ��ġ�� x ��ǥ 46.2�� �Ѿ�� �ݴ� �������� �̵�
        if (transform.position.x > 46.2f || transform.position.x < -76.17f)
        {
            moveDir *= -1f; // �̵� ������ �ݴ�� ����
            SetSpriteDir(moveDir);
            isChangingDirection = true; // ���� ���� �� �÷��׸� ����
        }

        // ���� ���� ���̰�, ������ x ��ǥ�� �ٽ� ���� ���� ������ ���� �� �÷��׸� ����
        if (isChangingDirection && transform.position.x <= 46.2f && transform.position.x >= -76.17f)
        {
            isChangingDirection = false;
        }
    }
    private void hit()
    {

    }

    private void die()
    {
       
    }

    private void SetMoveDir()
    {
        moveDir = Random.Range(-1f, 1f);
        moveStartTime = Time.time;
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
