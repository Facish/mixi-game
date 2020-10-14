using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public GameManager GameManager;
    public int type;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == 0)
        {
            try
            {
                collision.gameObject.GetComponent<PlayerManager>().point += 1;
                Destroy(gameObject);

            }
            catch
            {

            }
        }else if(type == 1)
        {
            try
            {
                GameManager.grow += 1;
                Destroy(gameObject);

            }
            catch
            {

            }
            
        }
    }

    // Update is called once per frame
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
}
