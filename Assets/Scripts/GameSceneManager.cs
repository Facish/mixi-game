using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    public string color = "red";

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
