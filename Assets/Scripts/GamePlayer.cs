using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviourPunCallbacks
{
    private Rigidbody2D rb2d;
    private AudioSource audio;

    private Vector2 jumpDirection = new Vector2(0f, 0f);
    private float jumpAngle = Mathf.PI /6;
    private float jumpPower = 0f;
    private const float FirstJumpPower = 1f;
    private const float MaxJumpPower = 10f;
    private const float AddPowerPerDeltaTime = 5f;
    private bool isGround = false;
    private float playerDirection = 1f;


    public int fruitNum = 0;

    GameObject sceneManager;
    GameSceneManager gameSceneManager;

    private DrawLine drawLineSprite;

    public AudioClip shortJumpSound;
    public AudioClip longJumpSound;
    public AudioClip chargeSound1;
    public AudioClip chargeSound2;
    public AudioClip eatSound;
    public AudioClip waterSound;
    public RuntimeAnimatorController player1motion;
    public RuntimeAnimatorController player2motion;
    public RuntimeAnimatorController player3motion;
    public RuntimeAnimatorController player4motion;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
        rb2d = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        sceneManager = GameObject.Find("GameSceneManager");
        gameSceneManager = sceneManager.GetComponent<GameSceneManager>();
        drawLineSprite = this.GetComponent<DrawLine>();

        anim = GetComponent<Animator>();
        if (photonView.ViewID == 1001)
        {
            anim.runtimeAnimatorController = player1motion;
        }
        else if (photonView.ViewID == 2001)
        {
            anim.runtimeAnimatorController = player2motion;
        }
        else if (photonView.ViewID == 3001)
        {
            anim.runtimeAnimatorController = player3motion;
        }
        else if (photonView.ViewID == 4001)
        {
            anim.runtimeAnimatorController = player4motion;
        }

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
                operatePlayerAndroid();
            }


            if (this.transform.position.y < -10f) {
                gameSceneManager.PlayerDied();
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Leaf") {
            // 葉の上か確認
            isGround = true;
            anim.SetBool("isGround", true);
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
        Debug.Log("Trigger");
        if (other.gameObject.tag == "Fruit") {
            var script = other.gameObject.GetComponent<Fruit>();
            script.TryGetItem(gameObject);
            script.DeleteFruit();

            gameSceneManager.PlayerGetFruit(fruitNum);
            audio.PlayOneShot(eatSound, 0.65f);
            //var script = other.gameObject.GetComponent<Fruit>();
            //script.TryGetItem(gameObject);
        }


        if (other.gameObject.tag == "WateringCan") {
            gameSceneManager.GrowLeafbyCan(other.gameObject);
            audio.PlayOneShot(waterSound, 0.8f);
            Debug.Log("Watering Can get!");
        }
    }

    // とりあえずクリック操作を包んだ
    private void operatePlayer() {
        if (isGround) {
            // クリック離した瞬間
            if (Input.GetMouseButtonUp(0)) {
                // ジャンプ
                photonView.RPC(nameof(RPCPlayerMove), RpcTarget.All, jumpPower, jumpDirection);
                isGround = false;
                anim.SetBool("isGround", false);
                drawLineSprite.LineDrawOff();

                if (jumpPower > 5) {
                    audio.PlayOneShot(longJumpSound, 0.95f);
                }
                else {
                    audio.PlayOneShot(audio.clip);
                }
            }
            //クリック中
            if (Input.GetMouseButton(0)) {
                //Debug.Log(Input.mousePosition.x);

                // 画面右クリック
                if (Input.mousePosition.x > Screen.width/2) {
                    jumpAngle = - Mathf.PI /8;
                }
                // 画面左クリック
                else {
                    jumpAngle = Mathf.PI /8;
                }

                // キャラクター方向
                if (playerDirection != Mathf.Sign(jumpAngle)) {
                    photonView.RPC(nameof(RPCPlayerDirection), RpcTarget.All, Mathf.Sign(jumpAngle));
                }
                // 押している間ジャンプ力に加算
                if (jumpPower < MaxJumpPower) {
                    jumpPower += Time.deltaTime*AddPowerPerDeltaTime;
                }


                // 放物線の描画     
                drawLineSprite.SetPosition(new Vector3(this.transform.position.x, this.transform.position.y, 0f));
                Vector2 jumpVec2 = jumpPower * jumpDirection;
                drawLineSprite.SetVelocity(new Vector3(jumpVec2.x, jumpVec2.y, 0f));
                drawLineSprite.LineDrawOn();
            }
            //クリックした瞬間
            if (Input.GetMouseButtonDown(0)) {
                // ジャンプ変数のリセット
                jumpAngle = 0;
                jumpPower = FirstJumpPower;
            }
        }
    }

    private void operatePlayerAndroid() {
        if (isGround) {
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                // tap離した瞬間
                if (touch.phase == TouchPhase.Ended) {
                    // ジャンプ
                    photonView.RPC(nameof(RPCPlayerMove), RpcTarget.All, jumpPower, jumpDirection);
                    isGround = false;
                    anim.SetBool("isGround", false);
                    drawLineSprite.LineDrawOff();

                    if (jumpPower > 5) {
                        audio.PlayOneShot(longJumpSound, 0.95f);
                    }
                    else {
                        audio.PlayOneShot(audio.clip);
                    }
                }
                // tap中
                if (touch.phase == TouchPhase.Moved) {
                    //Debug.Log(Input.mousePosition.x);

                    // 画面右クリック
                    if (touch.position.x > Screen.width/2) {
                        jumpAngle = - Mathf.PI /8;
                    }
                    // 画面左クリック
                    else {
                        jumpAngle = Mathf.PI /8;
                    }

                    // キャラクター方向
                    this.transform.localScale = new Vector3(-Mathf.Sign(jumpAngle), 1, 1);
                    // 押している間ジャンプ力に加算
                    jumpPower += Time.deltaTime*AddPowerPerDeltaTime;

                    // 放物線の描画
                    drawLineSprite.SetPosition(new Vector3(this.transform.position.x, this.transform.position.y, 0f));
                    Vector2 jumpVec2 = jumpPower * jumpDirection;
                    drawLineSprite.SetVelocity(new Vector3(jumpVec2.x, jumpVec2.y, 0f));
                    drawLineSprite.LineDrawOn();
                }
                // tapした瞬間
                if (touch.phase == TouchPhase.Began) {
                    // ジャンプ変数のリセット
                    jumpAngle = 0;
                    jumpPower = FirstJumpPower;
                    //drawLineSprite.LineDrawOn();
                }
            }
        }
    }

    [PunRPC]
    private void RPCPlayerMove(float jumpPower, Vector2 jumpDirection) {
        rb2d.velocity = jumpPower * jumpDirection;
    }

    [PunRPC]
    private void RPCPlayerDirection(float dir) {
        this.transform.localScale = new Vector3(-Mathf.Sign(jumpAngle), 1, 1);
        playerDirection = dir;
    }
}
