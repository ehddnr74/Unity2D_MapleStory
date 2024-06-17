using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class FeetScript : MonoBehaviour
{

    Rigidbody2D playerRigid;
    private Player player;

    private Collider2D col;

    private bool lowerGroundCol;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        player = FindObjectOfType<Player>();
        playerRigid = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if(!player.isGround)
        {
            if (playerRigid.velocity.y > 0)
            {
               IgnoreGroundCollision(true);
            }
            else
            {
               IgnoreGroundCollision(false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.layer == 7) //Lower Ground
        //{
        //    lowerGroundCol = true;
        //}
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MonsterGround"))
        {
            if (player != null)
            {
                player.SetIsGround(true);
                Debug.Log("충돌");
            }
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MonsterGround"))
        {
            if (player != null)
            {
                player.SetIsGround(false);
                Debug.Log("충돌 끝");
            }
        }
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }

    public void EnableCollider()
    {
        col.enabled = true;
    }

    public void IgnoreGroundCollision(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Feet"), LayerMask.NameToLayer("Ground"), ignore);
    }
    //public void GroundIgnoreTrue()
    //{
    //    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Feet"), LayerMask.NameToLayer("Ground"), true);
    //}
    //public void GroundIgnoreFalse()
    //{
    //    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Feet"), LayerMask.NameToLayer("Ground"), false);
    //}

}
