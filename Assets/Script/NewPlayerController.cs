using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    private Animator anim;
    private Collider2D coll;
    private Rigidbody2D rb;

    public float speed, jumpForce;
    public Transform groundCheck;
    public LayerMask ground;

    private bool isJump, isGround,jumpPressed;
    private int jumpCount;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position,0.1f,ground);
        groundMovement();
        jump();
        switchAnim();
    }

    void groundMovement()
    {
        float rawMove = Input.GetAxisRaw("Horizontal");
        //移动
        rb.velocity = new Vector2(rawMove*speed, rb.velocity.y);
        //朝向
        if (rawMove != 0)
        {
            transform.localScale = new Vector3(rawMove,1,1);
        }
        
    }

    void jump()
    {
        //每次在地面上
        if (isGround)
        {
            jumpCount = 2;
            isJump = false;
        }
        //一段跳(在地面或者空中下落的时候)
        if (jumpPressed && jumpCount>0)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        //二段跳
        else if (jumpPressed && isJump && jumpCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
    }

    void switchAnim()
    {
        anim.SetFloat("run", Mathf.Abs(rb.velocity.x));

        //falling -> idle
        if (isGround)
        {
            anim.SetBool("falling", false);
            anim.SetBool("idle", true);
        }
        //upward movement
        if(!isGround && rb.velocity.y>0)
        {
            anim.SetBool("jumping",true);
        }
        //falling
        if(!isGround &&rb.velocity.y<0)
        {
            anim.SetBool("falling", true);
            anim.SetBool("jumping", false);
        }
    }
}
