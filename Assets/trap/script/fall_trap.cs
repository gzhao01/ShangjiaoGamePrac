using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fall_trap : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxColl;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("人来了");
            Falling();
            //Debug.Log(rb.velocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        else if(collision.gameObject.tag == "Player")
        {
            rb.gravityScale = 1.0f;
        }
    }

    private void Falling()
    {
        rb.velocity = new Vector2(rb.velocity.x, -1*20);
    }
}
