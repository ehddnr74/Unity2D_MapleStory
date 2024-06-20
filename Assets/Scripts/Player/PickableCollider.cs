using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using UnityEngine.UIElements;

public class PickableCollider : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player =  GameObject.Find("Player").GetComponent<Player>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("DroppedItem"))
        {
            player.attachDroppedItem = true;
            if (player.pickable)
            {
                player.pickable = false;
                player.PickupItem(collision.gameObject);
            }
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DroppedItem"))
        {
            player.attachDroppedItem = false;
        }
    }

}
