using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Uooop : MonoBehaviour
{

    bool start=true;
    public GameObject cube;
    public UnityEvent StopReco;
    private Transform HiARCam;

    void Awake()
    {
        HiARCam = GameObject.Find("HiARCamera").transform;
    }


    void Update()
    {

        if (start&&cube.activeSelf)
        {
            cube.transform.localPosition += Vector3.up * Time.deltaTime * 1;
            if (cube.transform.localPosition.y>=6f)
            {
                start = false;
                cube.transform.SetParent(HiARCam);
                StopReco.Invoke();
            }
        }
    }
}
