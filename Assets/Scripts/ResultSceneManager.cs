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

    private bool draw = false;

    private GameObject resultTextObj;
    private Text resultText;
    // Start is called before the first frame update
    private void Start()
    {
        resultTextObj = GameObject.Find("Text");
        resultText = resultTextObj.GetComponent<Text>();

        if (!winner) {
            resultText.text = "no winner!";
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

            if (!draw) {
                resultText.text = "player" + winPlayer.ToString() + "win!";
            }
            else {
                resultText.text = "draw!";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        }
    }
}
