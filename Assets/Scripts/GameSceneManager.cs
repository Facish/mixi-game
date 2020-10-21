using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    public string color = "red";

    public bool gameStart = false;
    public int gameTimer = 18000;


    private int playerId;
    private List<GameObject> leaves = new List<GameObject>();
    private List<GameObject> fallLeaves = new List<GameObject>();
    public bool growLeafFlag = false;

    [SerializeField]
    private Fruit fruitPrefab = default;

    public List<GameObject> fruits = new List<GameObject>();
    public List<GameObject> getFruits = new List<GameObject>();
    public bool getFruitFlag = false; 


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        playerId = PhotonNetwork.LocalPlayer.ActorNumber;
        //playerId = 1;
        var v = new Vector3(0f, 0f);
        switch(playerId) {
            case 1: 
                v = new Vector3(-6f, 4f);
                break;

            case 2: 
                v = new Vector3(6f, 4f);
                break;

            case 3: 
                v = new Vector3(-2f, 4f);
                break;

            case 4: 
                v = new Vector3(2f, 4f);
                break;
        }
        PhotonNetwork.Instantiate("Player", v, Quaternion.identity);

        GameObject[] array = GameObject.FindGameObjectsWithTag("Leaf");
        for (int i = 0; i < array.Length; i++) {
            leaves.Add(array[i]);
        }

        if (playerId == 1) {
            for (int i = 0; i < 10; i++) {
                photonView.RPC(nameof(CreateFruit), RpcTarget.AllViaServer);
            }
        }
        array = GameObject.FindGameObjectsWithTag("Fruit");
        for (int i = 0; i < array.Length; i++) {
            fruits.Add(array[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 葉が落ちた時のアクティブ化(マスタークライアントで処理)
        if (PhotonNetwork.IsMasterClient && growLeafFlag) {
            foreach(GameObject leaf in leaves) {
                photonView.RPC(nameof(GrowLeaf), RpcTarget.AllViaServer);
            }
            growLeafFlag = false;
        }
        else if (growLeafFlag) {
            growLeafFlag = false;
        }

        // リンゴの生成
        if (PhotonNetwork.IsMasterClient && getFruitFlag) {
            photonView.RPC(nameof(RecycleFruit), RpcTarget.AllViaServer);
            getFruitFlag = false;
        }
        else if (getFruitFlag) {
            getFruitFlag = false;
        }
    }

    // 落ちた葉を非アクティブのListに追加
    public void FallLeaf(GameObject leaf) {
        growLeafFlag = true;
        leaves.Remove(leaf);
        fallLeaves.Add(leaf);
    }

    // 非アクティブな葉を再利用
    [PunRPC]
    private void GrowLeaf() {
        foreach(GameObject leaf in fallLeaves) {
            var script = leaf.GetComponent<Leaf>();
            var rb2d = leaf.GetComponent<Rigidbody2D>();
            var sprite = leaf.GetComponent<SpriteRenderer>();
            leaf.transform.localScale = new Vector3(1, 1, 1);
            leaf.transform.position = script.growPos;
            leaf.transform.localRotation = Quaternion.Euler(0, 0, 0);
            script.life = script.StartLife/3;
            script.growAmount = script.StartLife/3;
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            leaf.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    private void CreateFruit() {
        var pos = new Vector3(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
        var fruit = PhotonNetwork.Instantiate("Fruit", pos, Quaternion.identity);
        //fruit.Init(pos);
    }

    [PunRPC]
    public void GetFruits(GameObject fruit) {
        fruits.Remove(fruit);
        getFruits.Add(fruit);
    }

    [PunRPC]
    private void RecycleFruit() {
        foreach(GameObject fruit in getFruits) {
            var pos = new Vector3(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
            fruit.gameObject.SetActive(true);
            fruit.GetComponent<Fruit>().Init(pos);
        }
        getFruits.Clear();
    }
}
