using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviourPunCallbacks
{
    public int life;
    public int StartLife = 2000;
    public int growAmount = 2000;
    public Vector3 growPos;
    public bool isGrowing = false;

    private Rigidbody2D rigidbody2d;
    private SpriteRenderer leafsprite;
    private bool OnPlayer;
    private int lifeDecrease = 2;

    GameObject sceneManager;
    GameSceneManager gameSceneManager;
    // Start is called before the first frame update
    void Start()
    {
        life = StartLife;
        growPos = this.transform.position;
        OnPlayer = false;
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
        leafsprite = gameObject.GetComponent<SpriteRenderer>();

        sceneManager = GameObject.Find("GameSceneManager");
        gameSceneManager = sceneManager.GetComponent<GameSceneManager>();
    }

    void Update()
    {
        if (growAmount < StartLife) {
            growAmount += 1;
            life += 1;
        }
        
        // lifeに応じて葉の色を変更
        if (life < StartLife / 8)
        {
        //    foreach(Transform leafchild in gameObject.transform)
        //    {
        //        leafchild.GetComponent<SpriteRenderer>().color = Color.red;
        //    }
            leafsprite.color = Color.red;
        }
        else if (life < StartLife / 3)
        {
        //    foreach(Transform leafchild in gameObject.transform)
        //    {
        //        leafchild.GetComponent<SpriteRenderer>().color = Color.yellow;
        //    }
            leafsprite.color = Color.yellow;
        }
        else {
            leafsprite.color = Color.white;
        }

        // プレイヤーが乗っている間 or 落下確定時にlifeが減少
        if (OnPlayer || life < 0)
        {
            life -= lifeDecrease;
        }

        // マスタークライアントで落下処理
        if (PhotonNetwork.IsMasterClient){
            // プレイヤーが乗っているときlife減少 & 葉が落ちる演出
            if (-40 <= life && life <= 0)
            {
                //this.transform.localScale = new Vector3(0.5f + life * 0.01f, 0.5f + life * 0.01f, 1);
                this.transform.localRotation = Quaternion.Euler(0, 0, life - 45);
            }
            if (life <= -40)
            {
                rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
            }
            if(this.transform.position.y < -6f)
            {
                // 使いまわし & 非アクティブ化
                gameSceneManager.FallLeaf(gameObject);
                this.gameObject.SetActive(false);
            }
        }
    }

    // Playerが上に載っているか判定
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.transform.position.y + 0.5 <= collision.gameObject.transform.position.y)
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
