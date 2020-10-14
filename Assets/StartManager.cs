using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public GameObject one;
    public GameObject two;
    public GameObject three;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine("StartEffect");
    }

    IEnumerator StartEffect()
    {
        three.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        three.gameObject.SetActive(false);
        two.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        two.gameObject.SetActive(false);
        one.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        one.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
