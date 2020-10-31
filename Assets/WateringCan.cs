﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyWateringCan() {
        photonView.RPC(nameof(RPCDestroyWateringCan), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void RPCDestroyWateringCan() {
        this.gameObject.SetActive(false);
    }
}
