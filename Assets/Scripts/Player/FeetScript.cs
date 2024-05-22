using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class FeetScript : MonoBehaviour
{

    Rigidbody2D playerRigid;
    private Player player;

    private bool lowerGroundCol;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        playerRigid = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        //if (lowerGroundCol)
        //{
        //    if (playerRigid.velocity.y > 0)
        //        GroundIgnoreTrue();
        //    else
        //        GroundIgnoreFalse();
        //}
               
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if(collision.gameObject.layer == 7) //Lower Ground
        //{
        //    lowerGroundCol = true;
        //}
        if (collision.gameObject.CompareTag("Ground"))
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

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (player != null)
            {
                player.SetIsGround(false);
                Debug.Log("충돌 끝");
            }
        }
    }

    public void GroundIgnoreTrue()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), true);
    }
    public void GroundIgnoreFalse()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), false);
    }

}
