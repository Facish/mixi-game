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

    [SerializeField]
    public GameObject resultText = default;
    private SpriteRenderer resultTextSprite;

    private int maxFruit = -1;
    private int winPlayer = -1;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    private bool result = true;
    // Start is called before the first frame update
    private void Start()
    {
        //resultTextObj = GameObject.Find("Text");
        resultTextSprite = resultText.GetComponent<SpriteRenderer>();
        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);
        player3.gameObject.SetActive(false);
        player4.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (result && receiveVariable) {
            if (!winner) {
                //text = "no winner!";
                resultTextSprite.sprite = resultText.GetComponent<ResultText>().NoWinner;
                Debug.Log("no win");
            }
            else {
                int maxFruit = -1; 
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
                    //text = "player" + winPlayer.ToString() + "win!";
                    switch(winPlayer) {
                        case 0: 
                            {
                                resultTextSprite.sprite = resultText.GetComponent<ResultText>().Player1Win;
                                player1.gameObject.SetActive(true);
                                player1.GetComponent<Animator>().SetTrigger("OnceAnim");
                            }
                            break;
                        case 1:
                            {
                                resultTextSprite.sprite = resultText.GetComponent<ResultText>().Player2Win;
                                player3.gameObject.SetActive(true);
                                player3.GetComponent<Animator>().SetTrigger("OnceAnim");
                            }
                            break;
                        case 2:
                            {
                                resultTextSprite.sprite = resultText.GetComponent<ResultText>().Player3Win;
                                player2.gameObject.SetActive(true);
                                player2.GetComponent<Animator>().SetTrigger("OnceAnim");
                            }
                            break;
                        case 3:
                            {
                                resultTextSprite.sprite = resultText.GetComponent<ResultText>().Player4Win;
                                player4.gameObject.SetActive(true);
                                player4.GetComponent<Animator>().SetTrigger("OnceAnim");
                            }
                            break;
                    }
                    
                    Debug.Log("player" + winPlayer.ToString() + "win");
                }
                else {
                    //text = "draw!";
                    resultTextSprite.sprite = resultText.GetComponent<ResultText>().Draw;
                    Debug.Log("draw");
                }
            }
            
            result = false;
        }

        //resultText.text = text;

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
