using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaitSceneMangaer : MonoBehaviourPunCallbacks
{
    public GameObject startButton;
    private GameStartButton buttonScript;

    private int playerId;

    private bool gameStart = false;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        playerId = PhotonNetwork.LocalPlayer.ActorNumber;

        startButton = GameObject.Find("Button");
        buttonScript = startButton.GetComponent<GameStartButton>();

        if (playerId == 1) {
            startButton.SetActive(true);
        }
        else {
            startButton.SetActive(false);
        }

        Debug.Log(playerId);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerId == 1) {
            if (buttonScript.gameStart) {
                startButton.SetActive(false);
                photonView.RPC(nameof(StartGame), RpcTarget.AllViaServer);
                //Room.IsVisible = false;
            }
        }

        if (gameStart) {
            PhotonNetwork.IsMessageQueueRunning = false;
            SceneManager.LoadScene("GameScene");
        }
    }

    [PunRPC]
    void StartGame() {
        gameStart = true;
    }
}
