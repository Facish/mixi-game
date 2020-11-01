using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public string color = "red";



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor) {
            // エディタから実行
            if (Input.GetMouseButtonDown(0)) {
                // イベントに登録
                //SceneManager.sceneLoaded += MatchSceneLoaded;
                // シーン切り替え
                SceneManager.LoadScene("MatchScene");
            }
        }
        else {
            // 実機で実行
            if (Input.GetMouseButtonDown(0)) {
                // イベントに登録
                //SceneManager.sceneLoaded += MatchSceneLoaded;
                // シーン切り替え
                SceneManager.LoadScene("MatchScene");
            }

            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                 if (touch.phase == TouchPhase.Began) {
                     // イベントに登録
                    //SceneManager.sceneLoaded += MatchSceneLoaded;
                    // シーン切り替え
                    SceneManager.LoadScene("MatchScene");
                 }
            }
        }
    }

    private void MatchSceneLoaded(Scene next, LoadSceneMode mode) {
        // MatchSceneManager(script)取得
        var sceneManager = GameObject.FindWithTag("GameManager").GetComponent<MatchSceneManager>();

        // データ受け渡し
        sceneManager.color = color;

        // イベントから削除
        SceneManager.sceneLoaded -= MatchSceneLoaded;
    }
}
