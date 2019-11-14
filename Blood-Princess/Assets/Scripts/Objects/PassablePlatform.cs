using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassablePlatform : MonoBehaviour
{
    public GameObject Player;
    public LayerMask PlayerLayer;

    

    private const float DetectDis = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        CheckPlayer();
    }

    private void CheckPlayer()
    {
        Vector2 Offset = GetComponent<BoxCollider2D>().offset;
        Vector2 Size = GetComponent<BoxCollider2D>().size;
        Size.x *= transform.localScale.x;
        Size.y += transform.localScale.y;

        Vector2 TopPos = (Vector2)transform.position + Offset + (Size.y / 2 + DetectDis / 2) * Vector2.up;

        if (Player)
        {
            RaycastHit2D hit = Physics2D.BoxCast(TopPos, new Vector2(Size.x, DetectDis), 0, Vector2.up, 0, PlayerLayer);
            if (!hit)
            {
                Player = null;
                GetComponent<ColliderInfo>().TopPassable = false;
            }
        }
    }
}
