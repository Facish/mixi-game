using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    public int life;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer leafsprite;
    private bool OnPlayer;
    // Start is called before the first frame update
    void Start()
    {
        life = 50;
        OnPlayer = false;
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        leafsprite = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
        if (life <= 25)
        {
            foreach(Transform leafchild in gameObject.transform)
            {
                leafchild.GetComponent<SpriteRenderer>().color = Color.red;
            }
            leafsprite.color = Color.red;
        }
        if (-40 <= life && life <= 0)
        {
            transform.localScale = new Vector3(0.5f + life * 0.01f, 0.5f + life * 0.01f, 1);
            transform.localRotation = Quaternion.Euler(0, 0, life - 45);
            life -= 1;
        }else if (OnPlayer)
        {
            life -= 1;
        }
        if (life <= -40)
        {
            rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
            life -= 1;
        }
        if(life <= -100)
        {

            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.transform.position.y + 0.5 <= collision.gameObject.transform.position.y)
        {
            OnPlayer = true;
        }
    }

    // Update is called once per frame
    void OnCollisionExit2D(Collision2D collision)
    {
        OnPlayer = false;
    }
}
