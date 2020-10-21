using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviourPunCallbacks
{
    private Rigidbody2D rb2d;

    private Vector2 jumpDirection = new Vector2(0f, 0f);
    private float jumpAngle = Mathf.PI /6;
    private float jumpPower = 0f;
    private const float FirstJumpPower = 1f;
    private const float AddPowerPerFrame = 0.01f;
    private bool isGround = false;


    public int fruitNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update() {
        if (photonView.IsMine) {
            if (Application.isEditor) {
                // エディタから実行
                operatePlayer();
            }
            else {
                // 実機で実行
                operatePlayer();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Leaf") {
            // 葉の上か確認
            isGround = true;
        }
    }
    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Leaf") {
            //isGround = false;
        }
    }
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Leaf") {
            // 乗っている葉の法線ベクトルを用いてジャンプ方向を決める（内積）
            Vector2 dir = new Vector2(other.contacts[0].normal.x * Mathf.Cos(jumpAngle) - other.contacts[0].normal.y * Mathf.Sin(jumpAngle),
                                        other.contacts[0].normal.x * Mathf.Sin(jumpAngle) + other.contacts[0].normal.y * Mathf.Cos(jumpAngle));
            jumpDirection = dir.normalized;
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Fruit") {
            other.gameObject.SetActive(false);
            //var script = other.gameObject.GetComponent<Fruit>();
            //script.TryGetItem(gameObject);
        }
    }

    // とりあえずクリック操作を包んだ
    private void operatePlayer() {
        if (isGround) {
                // クリック離した瞬間
                if (Input.GetMouseButtonUp(0)) {
                    // ジャンプ
                    rb2d.velocity = jumpPower * jumpDirection;
                    isGround = false;
                }
                //クリック中
                if (Input.GetMouseButton(0)) {
                    Debug.Log(Input.mousePosition.x);

                    // 画面右クリック
                    if (Input.mousePosition.x > Screen.width/2) {
                        jumpAngle = - Mathf.PI /8;
                    }
                    // 画面左クリック
                    else {
                        jumpAngle = Mathf.PI /8;
                    }

                    // キャラクター方向
                    this.transform.localScale = new Vector3(-Mathf.Sign(jumpAngle), 1, 1);
                    // 押している間ジャンプ力に加算
                    jumpPower += AddPowerPerFrame;
                }
                //クリックした瞬間
                if (Input.GetMouseButtonDown(0)) {
                    // ジャンプ変数のリセット
                    jumpAngle = 0;
                    jumpPower = FirstJumpPower;
                }
            }
    }

    [PunRPC]
    private void HitByFruit() {
        
    }
}
