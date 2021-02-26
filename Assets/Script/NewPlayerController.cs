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
    public float variableJumpHeight;
    public float wallJumpForce;
    public Vector2 wallJumpDirection;
    private float jumpTimeCounter;
    [SerializeField]
    private bool isJump, isGround,jumpPressed;
    [SerializeField]
    private int jumpCount;
    

    [Header("dash")]
    public float dashTime; //set dash time
    private float dashTimeCounter; //dash time counter
    public float dashSpeed; // dash speed
    private float lastDashTime; // lash dash
    public float dashCoolDown; //cool down time
    private bool canDash = true;
    [SerializeField]
    private bool isDashing;

    [Header("Sliding Wall")]
    public Transform wallCheck;
    public float wallSlidingSpeed;
    private bool isTouchingWall;
    [SerializeField]
    private bool isWallSliding;
    public float wallCheckDis;

    //1 rep right, -1 rep left
    [SerializeField]
    private float faceDirection = 1f;

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
        //maybe useless
        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeight);
        }
        if (Input.GetButtonDown("Dash") && (Time.time>lastDashTime + dashCoolDown) && !isDashing)
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
            faceDirection = rawMove;
        }
        
    }

    void jump()
    {
        //每次在墙壁上重置跳跃次数
        if (isWallSliding)
        {
            jumpCount = 2;
            isJump = false;
        }
        //每次在地面上
        if (isGround)
        {
            jumpCount = 2;
            isJump = false;
        }
        //一段跳(在地面或者空中下落的时候)/但不能在墙上
        if (jumpPressed && jumpCount>0 && !isWallSliding)
        {
            jumpTimeCounter = jumpTime;
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        //wall jumping
        if (jumpPressed && isWallSliding)
        {
            isWallSliding = false;
            isJump = true;
            jumpCount--;
            jumpPressed = false;
            Vector2 forceToAdd = new Vector2(-faceDirection * wallJumpForce * wallJumpDirection.x, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        //二段跳
        if (jumpPressed && isJump && jumpCount > 0)
        {
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        //按住按键跳高
        if(jumpPressed == false && isJump && Input.GetButton("Jump") && jumpTimeCounter>0)
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
        //change to sliging
        if (isWallSliding)
        {
            anim.SetBool("sliding", true);
        }
        if (!isWallSliding)
        {
            anim.SetBool("sliding", false);
        }
    }

    void readyToDash()
    {
        isDashing = true;
        dashTimeCounter = dashTime;
        lastDashTime = Time.time;
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
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDis, ground) || Physics2D.Raycast(wallCheck.position, new Vector3(-1, 0, 0), wallCheckDis, ground);
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
    void OnDrawGizmos()//画线
    {
        Gizmos.color = Color.yellow;
        //显示line_pointList中存储点所组成的射线
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right* wallCheckDis);
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + new Vector3(-1, 0, 0)*wallCheckDis);
    }

}
