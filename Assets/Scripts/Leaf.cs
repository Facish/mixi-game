using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviourPunCallbacks
{
    public float life;
    public Color color = new Color(0, 0, 0);
    public float StartLife = 2000f;
    public float growAmount = 2000f;
    public Vector3 growPos;
    public bool isGrowing = false;

    private Rigidbody2D rigidbody2d;
    private SpriteRenderer leafsprite;
    private bool OnPlayer;
    private float lifeDecrease = 30f;

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
            growAmount += Time.deltaTime*5;
            life += Time.deltaTime*5;
        }
        
        if (PhotonNetwork.IsMasterClient) {
            // lifeに応じて葉の色を変更
            if (life > 0) {
                if (life < StartLife / 8)
                {
                //    foreach(Transform leafchild in gameObject.transform)
                //    {
                //        leafchild.GetComponent<SpriteRenderer>().color = Color.red;
                //    }
                    color = Color.red;
                    ChangeColor(color);
                }
                else if (life < StartLife / 3)
                {
                //    foreach(Transform leafchild in gameObject.transform)
                //    {
                //        leafchild.GetComponent<SpriteRenderer>().color = Color.yellow;
                //    }
                    color = Color.yellow;
                    ChangeColor(color);
                }
                else {
                    color = Color.white;
                    ChangeColor(color);
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
            

            if(this.transform.position.y < -10f)
            {
                // 使いまわし & 非アクティブ化
                photonView.RPC(nameof(RPCDeactivate), RpcTarget.AllViaServer);
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


    private void ChangeColor(Color color) {
        if (leafsprite.color != color) {
            float[] leafColor = {(float)color.r, (float)color.g, (float)color.b};
            photonView.RPC(nameof(RPCChangeColor), RpcTarget.AllViaServer, leafColor);
        }
    }

    [PunRPC]
    private void RPCChangeColor(float[] color) {
        leafsprite.color = new Color(color[0], color[1], color[2]);
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
