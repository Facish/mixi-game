using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultText : MonoBehaviour
{
    public Sprite Player1Win;
    public Sprite Player2Win;
    public Sprite Player3Win;
    public Sprite Player4Win;
    public Sprite Draw;
    public Sprite NoWinner;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
