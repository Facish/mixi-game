using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{

    GameObject sceneManager;
    GameSceneManager gameSceneManager;


    private float theta = Mathf.PI/8;
    private float StartAngle = -Mathf.PI/4;
    private float MoveAngle = Mathf.PI;
    private float Radius = 14f;
    private Vector2 Center = new Vector2(0f, -8f);

    // Start is called before the first frame update
    void Start()
    {
        sceneManager = GameObject.Find("GameSceneManager");
        gameSceneManager = sceneManager.GetComponent<GameSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        var addAngle = MoveAngle * (1f - gameSceneManager.gameTimer/gameSceneManager.GameTime);
        theta = StartAngle + addAngle;

        var posX = Center.x + Radius * Mathf.Cos(theta);
        var posY = Center.y + Radius * Mathf.Sin(theta);

        this.transform.position = new Vector3(posX, posY, 20f);
    }
}
