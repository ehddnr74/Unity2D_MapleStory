using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private Player player;

    public BoxCollider2D GetGroundCol() { return boxCollider; }
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boxCollider != null && player != null)
        {
            if (player.GetIsLaddering())
                boxCollider.usedByEffector = true;
            else
                boxCollider.usedByEffector = false;
        }
    }


}
