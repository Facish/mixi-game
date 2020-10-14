using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> branch = new List<GameObject>();
    int time;
    public int grow = 0;
    public GameObject maincamera;
    public GameObject branchlist;
    public GameObject branchobject;
    public GameObject can;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(time % (240 - grow * 5) == 0)
        {
            branch[Random.Range(0, branch.Count)].GetComponent<Branch>().GrowLeaf();
            branch[Random.Range(0, branch.Count)].GetComponent<Branch>().GrowPoint();
        }
        if (time % (480 - grow * 10) == 0)
        {
            GameObject newbranch = Instantiate(branchobject, new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 10), Quaternion.Euler(0, 0, 0));
            newbranch.transform.SetParent(branchlist.transform, false);
            branch.Add(newbranch);
            GameObject newcan = Instantiate(can, new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 10), Quaternion.Euler(0, 0, 0));
            newcan.transform.SetParent(maincamera.transform, false);
        }

        time += 1;
    }
}
