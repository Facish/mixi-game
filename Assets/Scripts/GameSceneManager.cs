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
    public float gameTimer = 5;
    public float GameTime = 5;
    private Animator anim;

    public int playerId;
    private List<GameObject> leaves = new List<GameObject>();
    private List<GameObject> fallLeaves = new List<GameObject>();

    private List<Vector3> vacantLeafPositions = new List<Vector3>();
    private List<Vector3> occupiedLeafPositions = new List<Vector3>();
    public bool growLeafFlag = false;

    private List<GameObject> wateringCans = new List<GameObject>();
    private int createCansCount = 0;

    private bool[] playerLive = new bool[4];
    private int[] playerFruit = {-1, -1, -1, -1};

    public List<GameObject> fruits = new List<GameObject>();
    public List<GameObject> getFruits = new List<GameObject>();
    public bool getFruitFlag = false; 


    private bool gameEnd = false;
    private bool winner = true;


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
        photonView.RPC(nameof(PlayerBorn), RpcTarget.All, playerId);

        GameObject[] array = GameObject.FindGameObjectsWithTag("Leaf");
        for (int i = 0; i < array.Length; i++) {
            leaves.Add(array[i]);
            var pos = array[i].transform.position;
            vacantLeafPositions.Add(new Vector3(pos.x, pos.y + 1f, pos.z));
        }

        if (playerId == 1) {
            for (int i = 0; i < 6; i++) {
                //photonView.RPC(nameof(CreateFruit), RpcTarget.AllViaServer);
                CreateFruit();
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
        if (playerId == 1 && growLeafFlag) {
            foreach(GameObject leaf in leaves) {
                photonView.RPC(nameof(GrowLeaf), RpcTarget.AllViaServer);
            }
            growLeafFlag = false;
        }
        else if (growLeafFlag) {
            growLeafFlag = false;
        }

        // リンゴの生成
        if (playerId == 1 && getFruitFlag) {
            RecycleFruit();
            getFruitFlag = false;
        }
        else if (getFruitFlag) {
            getFruitFlag = false;
        }

        // ゲーム時間
        gameTimer -= Time.deltaTime*180;
        if (playerId == 1) {
            if (gameTimer < 0) {
                photonView.RPC(nameof(RPCGameEnd), RpcTarget.AllViaServer);
            }
            if (!playerLive[0] && !playerLive[1] && !playerLive[2] && !playerLive[3]) {
                photonView.RPC(nameof(RPCPlayerAllFall), RpcTarget.AllViaServer);
            }

            
            if (gameTimer < GameTime/2 && createCansCount == 0) {
                for (int i = 0; i < 2; i++) {
                    GameObject can = CreateWateringCan();
                    wateringCans.Add(can);
                }
                createCansCount = 1;
            }
            if (gameTimer < GameTime/4 && createCansCount == 1) {
                for (int i = 0; i < 4; i++) {
                    GameObject can = CreateWateringCan();
                    wateringCans.Add(can);
                }
                createCansCount = 2;
            }
        }
        

        // ゲーム終了
        if (gameEnd) {
            // ルーム退出
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();

            SceneManager.sceneLoaded += GameSceneLoaded;
            SceneManager.LoadSceneAsync("ResultScene", LoadSceneMode.Single);
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
            //leaf.transform.localScale = new Vector3(1, 1, 1);
            //leaf.transform.localRotation = Quaternion.Euler(0, 0, 0);
            script.life = script.StartLife/3;
            script.growAmount = script.StartLife/3;
            script.leafSize = 2;
            script.ChangeColor(0);
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            leaf.transform.position = script.growPos;
            leaf.gameObject.SetActive(true);
        }
        fallLeaves.Clear();
    }

    private Vector3 RandomPosition() {
        Debug.Log(vacantLeafPositions.Count);
        int randomPos = Random.Range(0, vacantLeafPositions.Count);
        Vector3 position = vacantLeafPositions[randomPos];
        vacantLeafPositions.RemoveAt(randomPos);
        occupiedLeafPositions.Add(position);
        return position;
    }
    private void UnoccupiedPosition(Vector3 position) {
        occupiedLeafPositions.Remove(position);
        vacantLeafPositions.Add(position);
    }

    //[PunRPC]
    private void CreateFruit() {
        if (playerId == 1) {
            var pos = RandomPosition();
            var fruit = PhotonNetwork.Instantiate("Fruit", pos, Quaternion.identity);
            //fruit.Init(pos);
        }
    }

    [PunRPC]
    public void GetFruits(GameObject fruit) {  
        if (playerId == 1) {   
            UnoccupiedPosition(fruit.transform.position);
        }
        fruits.Remove(fruit);
        getFruits.Add(fruit);
    }

    private void RecycleFruit() {
        foreach(GameObject fruit in getFruits)  {
            var pos = RandomPosition();
            fruit.GetComponent<Fruit>().AppearFruit(pos);
        }
        photonView.RPC(nameof(RPCGetFruitsClear), RpcTarget.AllViaServer);
    }
    /*
    [PunRPC]
    private void RPCRecycleFruit(GameObject fruit, Vector3 pos) {  
        fruit.gameObject.SetActive(true);
        fruit.GetComponent<Fruit>().Init(pos);
    }
    */
    [PunRPC]
    private void RPCGetFruitsClear() {
        getFruits.Clear();
    }

    //[PunRPC]
    private GameObject CreateWateringCan() {
        var pos = RandomPosition();
        var wateringCan = PhotonNetwork.Instantiate("WateringCan", pos, Quaternion.identity);
        return wateringCan;
    }

    public void GrowLeafbyCan(GameObject wateringCan) {
        if (playerId == 1){
            //photonView.RPC(nameof(RPCGrowLeafbyCan), RpcTarget.AllViaServer, wateringCans.IndexOf(wateringCan));
            foreach(GameObject leaf in leaves) {
                var script = leaf.GetComponent<Leaf>();
                script.GrowbyWater();
            }
            wateringCans.Remove(wateringCan);
            //PhotonNetwork.Destory(wateringCan);
            UnoccupiedPosition(wateringCan.transform.position);
            wateringCan.GetComponent<WateringCan>().DestroyWateringCan();
        }
    }
/*    [PunRPC]
    private void RPCGrowLeafbyCan(int index) {
        
        /*var can = wateringCans[index];
        wateringCans.RemoveAt(index);
        can.SetActive(false);*/
//    }

    // プレイヤーの生存フラグON
    [PunRPC]
    private void PlayerBorn(int playerId) {
        playerLive[playerId] = true;
    }

    public void PlayerDied() {
        photonView.RPC(nameof(RPCPlayerDied), RpcTarget.AllViaServer, playerId);
    }
    [PunRPC]
    private void RPCPlayerDied(int playerId) {
        playerLive[playerId] = false;
    }

    // プレイヤーがフルーツ獲得
    public void PlayerGetFruit(int fruitNum) {
        photonView.RPC(nameof(RPCPlayerGetFruit), RpcTarget.AllViaServer, playerId, fruitNum);
        Debug.Log(fruitNum);
    }
    [PunRPC]
    private void RPCPlayerGetFruit(int playerId, int fruitNum) {
        playerFruit[playerId] = fruitNum;
    }

    // ゲーム時間
    [PunRPC]
    private void RPCGameEnd() {
        gameEnd = true;
    }

    [PunRPC]
    private void RPCPlayerAllFall() {
        gameEnd = true;
        winner = false;
    }



    private void GameSceneLoaded(Scene next, LoadSceneMode mode) {
        // MatchSceneManager(script)取得
        var sceneManager = GameObject.FindWithTag("GameManager").GetComponent<ResultSceneManager>();

        // データ受け渡し
        sceneManager.color = color;
        for(int i = 0; i < 4; i++) {
            sceneManager.fruit[i] = playerFruit[i];
        }
        sceneManager.winner = winner;
        sceneManager.receiveVariable = true;

        Debug.Log("pass value");

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
