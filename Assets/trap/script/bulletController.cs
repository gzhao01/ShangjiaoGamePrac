using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    private Animator anim;
    private Collider2D coll;
    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Start()
    {

        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -40)
        {
            Destroy(gameObject);
        }
    }

    void DestroyBullet()
    {
        Debug.Log("by-by");
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wall")
        {
            Debug.Log("hit something");
            anim.SetBool("hit", true);
        }

    }
}
