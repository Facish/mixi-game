using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    private Rigidbody2D rb2d;

    private Vector2 jumpDirection = new Vector2(0f, 0f);
    private float jumpAngle = Mathf.PI /6;
    private float jumpPower = 0f;
    private const float FirstJumpPower = 1f;
    private const float AddPowerPerFrame = 0.01f;
    private bool isGround = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update() {
        if (Application.isEditor) {
            // エディタから実行
            if (isGround) {
                // クリック離した瞬間
                if (Input.GetMouseButtonUp(0)) {
                    // ジャンプ
                    rb2d.velocity = jumpPower * jumpDirection;

                    // ジャンプ変数のリセット
                    isGround = false;
                    jumpPower = FirstJumpPower;

                }
                //クリック中
                if (Input.GetMouseButton(0)) {
                    Debug.Log(Input.mousePosition.x);
                    // 画面右クリック
                    if (Input.mousePosition.x > Screen.width/2) {
                        jumpAngle = - Mathf.PI /4;
                    }
                    // 画面左クリック
                    else {
                        jumpAngle = Mathf.PI /4;
                    }

                    // 押している間ジャンプ力に加算
                    jumpPower += AddPowerPerFrame;
                }
            }
        }
        else {
            // 実機で実行
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Leaf") {
            // 葉の上か確認
            isGround = true;
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
}
