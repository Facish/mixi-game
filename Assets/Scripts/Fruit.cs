using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviourPunCallbacks
{
    GameObject sceneManager;
    GameSceneManager gameSceneManager;

    private bool isAvailable;

    private int getScore = 0;

    public void Spawn() {
        isAvailable = true;
    }

    public void Init(Vector3 origin) {
        transform.position = origin;
    }

    public void Start() {
        sceneManager = GameObject.Find("GameSceneManager");
        gameSceneManager = sceneManager.GetComponent<GameSceneManager>();
    }


    public void TryGetItem(GameObject player) {
        getScore = 0;
        photonView.RPC(nameof(RPCTryGetItem), RpcTarget.AllViaServer);
        player.GetComponent<GamePlayer>().fruitNum += getScore;//getScore;
    }


    [PunRPC]    
    private void RPCTryGetItem(PhotonMessageInfo info) {
        if (isAvailable) {
            isAvailable = false;

            if (info.Sender.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) {
                Debug.Log("get a fruit");
                getScore = 1;
            }
            else {
                Debug.Log("miss");
            }
        }
    }

    public void DeleteFruit() {
        photonView.RPC(nameof(RPCDeleteFruit), RpcTarget.All);
    }

    [PunRPC]
    private void RPCDeleteFruit() {
        gameSceneManager.GetFruits(this.gameObject);
        gameSceneManager.getFruitFlag = true;
        this.gameObject.SetActive(false);
    }

}
