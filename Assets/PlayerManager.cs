using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    private float size;
    private int jumprest;
    private Rigidbody2D rigidbody2d;
    private bool isGrounded;
    public LayerMask LeafLayer;
    public int point;
    void Start()
    {
        size = 1.25f;
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        jumprest = 2;
    }

    private void Update()
    {
        isGrounded = Physics2D.Linecast(transform.position - transform.up * (size / 2f + 0.05f) + transform.right * (size / 2f), transform.position - transform.up * (size / 2f + 0.05f) - transform.right * (size / 2f), LeafLayer);

        if (isGrounded && rigidbody2d.velocity.y <= 0.05)
        {
            jumprest = 2;
        }
        if(Input.GetKeyDown(KeyCode.Space) && jumprest >= 1)
        {
            jumprest -= 1;
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, speed * 2);
            isGrounded = false;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if(x != 0)
        {
            rigidbody2d.velocity = new Vector2(x * speed, rigidbody2d.velocity.y);
        }
        else
        {
            rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        }

    }
}
