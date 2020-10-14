using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public int time;
    public GameObject leaf;
    public GameObject point;
    public GameObject maincamera;
    // Update is called once per frame
    public void GrowLeaf()
    {
        GameObject newleaf = Instantiate(leaf, new Vector3(1.6f, Random.Range(-0.5f,0.5f), 5), Quaternion.Euler(0, 0, -45));
        newleaf.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        newleaf.transform.SetParent(gameObject.transform, false);
    }

    public void GrowPoint()
    {
        GameObject newpoint = Instantiate(point, new Vector3(1.6f, Random.Range(-0.5f, 0.5f), 5), Quaternion.Euler(0, 0, 0));
        newpoint.transform.localScale = new Vector3(0.25f, 0.25f, 1);
        newpoint.transform.SetParent(gameObject.transform, false);
    }
}
