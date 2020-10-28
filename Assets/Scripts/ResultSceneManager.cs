using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultSceneManager : MonoBehaviour
{
    public string color = "red";
    public bool winner = true;
    public int[] fruit = new int[4];


    public bool receiveVariable = false;

    private bool draw = false;

    public GameObject resultTextObj = default;
    private Text resultText;
    private string text;

    private bool result = true;
    // Start is called before the first frame update
    private void Start()
    {
        //resultTextObj = GameObject.Find("Text");
        resultText = resultTextObj.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (result && receiveVariable) {
            if (!winner) {
                text = "no winner!";
                Debug.Log("no win");
            }
            else {
                var maxFruit = -1; 
                int winPlayer = -1;
                for (int i = 0; i < 4; i++) {
                    if (maxFruit < fruit[i]) {
                        maxFruit = fruit[i];
                        winPlayer = i;
                        draw = false;
                    }
                    else if (maxFruit == fruit[i]) {
                        draw = true;
                    }
                }
                Debug.Log(maxFruit);

                if (!draw) {
                    text = "player" + winPlayer.ToString() + "win!";
                }
                else {
                    text = "draw!";
                }
            }
            
            result = false;
        }

        resultText.text = text;

        if (Application.isEditor) {
            // エディタから実行
            if (Input.GetMouseButtonDown(0)) {
                // シーン切り替え
                SceneManager.LoadScene("TitleScene");
            }
        }
        else {
            // 実機で実行
            if (Input.GetMouseButtonDown(0)) {
                // シーン切り替え
                SceneManager.LoadScene("TitleScene");
            }

            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    // シーン切り替え
                    SceneManager.LoadScene("TitleScene");
                }
            }
        }
    }
}
