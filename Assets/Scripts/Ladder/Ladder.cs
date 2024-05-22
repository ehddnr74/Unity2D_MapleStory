using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    // Start is called before the first frame update


    void Start()
    {
    }

    public Vector3 GetLadderPosition()
    {
        return transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.SetIsOnLadder(this);
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.SetIsOnLadder(null);
        }
    }
}
