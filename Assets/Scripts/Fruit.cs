using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviourPunCallbacks
{
    private bool isAvailable;

    private int getScore = 0;

    public void Spawn() {
        isAvailable = true;
    }

    public void Init(Vector3 origin) {
        transform.position = origin;
    }


    public void TryGetItem(GameObject player) {
        photonView.RPC(nameof(RPCTryGetItem), RpcTarget.AllViaServer);
        player.GetComponent<GamePlayer>().fruitNum += getScore;
        this.gameObject.SetActive(false);
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
}
