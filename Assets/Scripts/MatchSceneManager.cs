using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchSceneManager : MonoBehaviourPunCallbacks
{
    public string color = "red";

    // Start is called before the first frame update
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();    
        PhotonNetwork.SendRate = 30; // 1秒間にメッセージ送信を行う回数
        PhotonNetwork.SerializationRate = 30; // 1秒間にオブジェクト同期を行う回数
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinRandomRoom();
    }

    // マッチングが成功したときに呼ばれるコールバック
    public override void OnJoinedRoom() {
        PhotonNetwork.IsMessageQueueRunning = false;
        Debug.Log("ルームに参加しました");

        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
    }

    // 通常のマッチングが失敗した時に呼ばれるコールバック
    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log($"ルーム参加に失敗しました: {message}");
    }

    // ランダムマッチングが失敗した時に呼ばれるコールバック
    public override void OnJoinRandomFailed(short returnCode, string message) {
        // ランダムに参加できるルームが存在しないなら、新しいルームを作成する
        PhotonNetwork.CreateRoom(
            null,
            new RoomOptions() {
                MaxPlayers = 4
            }
        );
    }


    private void GameSceneLoaded(Scene next, LoadSceneMode mode) {
        // MatchSceneManager(script)取得
        var sceneManager = GameObject.FindWithTag("GameManager").GetComponent<GameSceneManager>();

        // データ受け渡し
        sceneManager.color = color;

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
