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

    public float jumpTime;
    private float jumpTimeCounter;
    private bool isJump, isGround,jumpPressed;
    private int jumpCount;

    [Header("dash")]
    public float dashTime; //set dash time
    private float dashTimeCounter; //dash time counter
    public float dashSpeed; // dash speed
    private float lastDashTime; // lash dash
    public float dashCoolDown;
    [SerializeField]
    private bool isDashing;

    [Header("Sliding Wall")]
    public Transform wallCheck;
    public float wallSlidingSpeed;
    private bool isTouchingWall;
    [SerializeField]
    private bool isWallSliding;
    public float wallCheckDis;


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
        if (Input.GetButtonDown("Dash"))
        {
            readyToDash();
        }
        checkIfWallSliding();
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position,0.15f,ground);
        groundMovement();
        //should be here, else it will be useless
        sliding();
        jump();
        switchAnim();
        dash();
        checkSurroundings();
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
        //每次在墙壁上重置跳跃次数
        if (isWallSliding)
        {
            jumpCount = 2;
        }
        //每次在地面上
        if (isGround)
        {
            jumpCount = 2;
            isJump = false;
        }
        //一段跳(在地面或者空中下落的时候)
        if (jumpPressed && jumpCount>0)
        {
            jumpTimeCounter = jumpTime;
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        //二段跳
        else if (jumpPressed && isJump && jumpCount > 0)
        {
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        //按住按键跳高
        if(isJump && Input.GetButton("Jump") && jumpTimeCounter>0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimeCounter -= Time.deltaTime;
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
        //dashing
        if (isDashing)
        {
            anim.SetBool("dashing", true);
            anim.SetBool("idle", true);
        }
        //dashing -> idle
        if (!isDashing)
        {
            anim.SetBool("dashing", false);
            anim.SetBool("idle", true);
        }
    }

    void readyToDash()
    {
        isDashing = true;
        dashTimeCounter = dashTime;        
    }

    void dash()
    {
        if (isDashing)
        {
            if (dashTimeCounter > 0)
            {
                //即使站在原地不动也可以dash
                rb.velocity = new Vector2(dashSpeed * gameObject.transform.localScale.x, 0);
                dashTimeCounter -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
            }
        }
    }

    void checkSurroundings()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDis, ground) || Physics2D.Raycast(wallCheck.position, new Vector3(-1,1,1), wallCheckDis, ground);
    }

    void checkIfWallSliding()
    {
        //in the air and close to the wall
        if(!isGround && isTouchingWall && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    void sliding()
    {
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
        }
    }
}
