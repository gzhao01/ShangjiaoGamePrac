using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall_turret : MonoBehaviour
{
    private Animator anim;
    public LineRenderer line;
    private AnimatorStateInfo animInfo;

    public Transform shotCheck;
    public Transform shotPoint;
    public GameObject bullet;

    public bool faceLeft = true;
    public bool faceright = false;
    public bool isShot = false;
    public bool isBulletExist = false;

    public float shotInterval = 4.0f;
    private float shotTime = 0.0f;
    private float pointYoffset;
    private float checkYoffset;
    private float shotX;
    //public bool loop = true;
    public LayerMask playerLayer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        line.enabled = true;
        line.startColor = Color.red;
        line.endColor = Color.red;
        line.startWidth = 0.15f;
        line.endWidth = 0.15f;
        pointYoffset = shotPoint.position.y - transform.position.y;
        checkYoffset = shotCheck.position.y - transform.position.y;
        shotX = shotPoint.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchAnim();
        //SwitchScale();

    }

    private void FixedUpdate()
    {
        SwitchScale();
    }

    void ShotCheck()
    {
        Vector2 checkDirection = new Vector2(shotCheck.position.x - shotPoint.position.x, shotCheck.position.y - shotPoint.position.y);
        float checkDistance = Vector2.Distance(shotCheck.position, shotPoint.position);
        //Debug.Log(checkDirection);
        //Debug.Log(checkDistance);

        RaycastHit2D checkShot = Physics2D.Raycast(shotPoint.position, checkDirection, checkDistance, playerLayer);
        //Debug.DrawRay(transform.position, checkDirection,Color.red,0.2f);

        if (checkShot)
        {
            isShot = true;
            ShotBullet();
            line.SetPosition(0, new Vector3(shotPoint.position.x, shotPoint.position.y, -1));
            line.SetPosition(1, new Vector3(checkShot.point.x, checkShot.point.y, -1));
        }
        else
        {
            isShot = false;
            line.SetPosition(0, new Vector3(shotPoint.position.x, shotPoint.position.y, -1));
            line.SetPosition(1, new Vector3(shotCheck.position.x, shotCheck.position.y, -1));
        }
    }

    void SwitchAnim()
    {
        animInfo = anim.GetCurrentAnimatorStateInfo(0);
        anim.SetBool("shot", isShot);
        //anim.SetFloat("scale", transform.localScale.x);
        anim.SetBool("face_left", faceLeft);
        anim.SetBool("face_right", faceright);
        if (animInfo.normalizedTime <= 0.0f)
        {
            line.enabled = false;
            //Debug.Log("射线没了吗？");
        }
    }

    void SwitchScale()
    {


        if (animInfo.normalizedTime >= 1.0f && animInfo.IsName("left-right"))
        {
            faceLeft = false;
            faceright = true;
            //transform.localScale = new Vector3(-1, 1, 1);
            //shotPoint.position = new Vector3(transform.position.x + pointXoffset, shotY, 0);
            shotPoint.position = new Vector3(shotX,transform.position.y - pointYoffset, 0);
            shotCheck.position = new Vector3(shotX, transform.position.y - checkYoffset, 0);
            //shotCheck.position = new Vector3(transform.position.x + checkXoffset, shotY, 0);
            line.enabled = true;
        }
        else if (animInfo.normalizedTime >= 1.0f && animInfo.IsName("right-left"))
        {
            faceLeft = true;
            faceright = false;
            //transform.localScale = new Vector3(1, 1, 1);
            //shotPoint.position = new Vector3(transform.position.x - pointXoffset, shotY, 0);
            //shotCheck.position = new Vector3(transform.position.x - checkXoffset, shotY, 0);
            shotPoint.position = new Vector3(shotX, transform.position.y + pointYoffset, 0);
            shotCheck.position = new Vector3(shotX, transform.position.y + checkYoffset, 0);

            line.enabled = true;

        }
        ShotCheck();

    }

    void ShotBullet()
    {

        if (!isBulletExist)
        {
            GameObject shotBullet = Instantiate(bullet, shotPoint.position, Quaternion.identity);
            Rigidbody2D bulletRb = shotBullet.GetComponent<Rigidbody2D>();
            //Vector2 shotDirection = new Vector2(shotCheck.position.x - shotPoint.position.x, shotCheck.position.y - shotPoint.position.y);
            //bullet.GetComponent<Rigidbody2D>().AddForce(shotDirection, ForceMode2D.Impulse);
            //float xVelocity = shotCheck.position.x - shotPoint.position.x;
            //bulletRb.velocity = new Vector2(xVelocity * 10, bulletRb.velocity.y);
            bulletRb.velocity = (shotCheck.position - shotPoint.position).normalized * 10.0f;
            Debug.Log(bulletRb.velocity);
            //Debug.Log(xVelocity);
            Debug.Log(bulletRb);
            shotTime = Time.time;
            isBulletExist = true;
        }
        float nowTime = Time.time;
        if (nowTime >= (shotTime + shotInterval) && isBulletExist)
            isBulletExist = false;


    }
}
