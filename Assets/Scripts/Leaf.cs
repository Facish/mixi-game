using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviourPunCallbacks
{
    // 葉のスプライト一覧
    public Sprite GreenBig;
    public Sprite GreenMiddle;
    public Sprite GreenSmall;
    public Sprite YellowBig;
    public Sprite YellowMiddle;
    public Sprite YellowSmall;
    public Sprite RedBig;
    public Sprite RedMiddle;
    public Sprite RedSmall;

    private Sprite[,] LeafSprite;


    public float life;
    public int leafColor = 0;
    public float StartLife = 2000f;
    public float growAmount = 2000f;
    public Vector3 growPos;
    public bool isGrowing = false;
    private int leafSize = 0;

    private Rigidbody2D rigidbody2d;
    private SpriteRenderer leafsprite;
    private EdgeCollider2D[] collider2Ds;
    private bool OnPlayer;
    private float lifeDecrease = 30f;
    private float lifeIncrease = 60f;

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

        collider2Ds = GetComponents<EdgeCollider2D>();

        LeafSprite = new Sprite[3,3]{{GreenBig, GreenMiddle, GreenSmall},
                                     {YellowBig, YellowMiddle, YellowSmall},
                                     {RedBig, RedMiddle, RedSmall}};
    }

    void Update()
    {   
        if (PhotonNetwork.IsMasterClient) {
            // lifeに応じて葉の色を変更
            if (life > 0) {
                if (life < StartLife / 8)
                {
                //    foreach(Transform leafchild in gameObject.transform)
                //    {
                //        leafchild.GetComponent<SpriteRenderer>().color = Color.red;
                //    }
                    ChangeColor(2);
                }
                else if (life < StartLife / 3)
                {
                //    foreach(Transform leafchild in gameObject.transform)
                //    {
                //        leafchild.GetComponent<SpriteRenderer>().color = Color.yellow;
                //    }
                    ChangeColor(1);
                }
                else {
                    ChangeColor(0);
                }
            }

            // プレイヤーが乗っている間 or 落下確定時にlifeが減少
            if (OnPlayer && life > 0)
            {
                photonView.RPC(nameof(RPCDecreaseLeafLife), RpcTarget.AllViaServer);
            }
            else if (life <= 0) {
                life -= Time.deltaTime*lifeDecrease;
            }

            if (-40 <= life && life <= 0)
            {
                //this.transform.localScale = new Vector3(0.5f + life * 0.01f, 0.5f + life * 0.01f, 1);
                //this.transform.localRotation = Quaternion.Euler(0, 0, life - 45);
            }
            if (life <= -40 && rigidbody2d.bodyType != RigidbodyType2D.Dynamic)
            {
                photonView.RPC(nameof(RPCFallLeaf), RpcTarget.AllViaServer);
            }
            

            if(this.transform.position.y < -15f)
            {
                // 使いまわし & 非アクティブ化
                photonView.RPC(nameof(RPCDeactivate), RpcTarget.AllViaServer);
            }


            // 葉の育成
            if (growAmount < StartLife) {
                growAmount += Time.deltaTime*lifeIncrease;
                life += Time.deltaTime*lifeIncrease;

                if (growAmount < StartLife/2) {
                    LeafGrow(2);
                }
                else if (growAmount < StartLife*3/4) {
                    LeafGrow(1);
                }
                else {
                    LeafGrow(0);
                }
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

    [PunRPC]
    private void RPCDecreaseLeafLife() {
        life -= Time.deltaTime*lifeDecrease;
    }


    public void ChangeColor(int color) {
        if (leafColor != color) {  
            photonView.RPC(nameof(RPCChangeLeaf), RpcTarget.AllViaServer, color, leafSize);
            leafColor = color;
        }
    }
    private void LeafGrow(int growNum) {
        if (leafSize != growNum) {
            photonView.RPC(nameof(RPCChangeLeaf), RpcTarget.AllViaServer, leafColor, growNum);
            leafSize = growNum;
        }
    }

    [PunRPC]
    private void RPCChangeLeaf(int colorNum, int leafSiz) {
        leafsprite.sprite = LeafSprite[colorNum, leafSiz];
        for (int i = 0; i < 3; i++) {
            if (i == leafSiz) {
                collider2Ds[i].enabled = true;
            }
            else {
                collider2Ds[i].enabled = false;
            }
        }
    }


    [PunRPC]
    private void RPCLeafGrow(int growNum) {
        leafSize = growNum;
    }


    [PunRPC]
    private void RPCFallLeaf() {
        rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
    }

    [PunRPC]
    private void RPCDeactivate() {
        gameSceneManager.FallLeaf(gameObject);
        this.gameObject.SetActive(false);
    }
}
