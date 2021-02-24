using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public float speed;
    public float jumpForce;
    public LayerMask ground;
    public Collider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        switchAnim();
    }

    void Move()
    {
        //get from -1.0~1.0
        //walk
        float horizontalMove = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalMove * speed * Time.deltaTime, rb.velocity.y);
        //get -1,0,1
        //turn direction
        float faceDirection = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("run",Mathf.Abs(faceDirection));
        if(faceDirection != 0)
        {
            transform.localScale = new Vector3(faceDirection,1,1);
        }
        //jump
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.deltaTime);
            anim.SetBool("jumping", true);
        }
    }

    void switchAnim()
    {
        if (anim.GetBool("jumping"))
        {
            if(rb.velocity.y < 0)
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", false);
            anim.SetBool("idle",true);
        }
    }
}
