using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GiftShopUI : MonoBehaviour 
{
    private HiAREngineBehaviour hiarEnginBehaviour;
    private Button ScanBtn;
    private Button backWardBtn;

    void Awake()
    {
        hiarEnginBehaviour = GameObject.Find("HiARCamera").GetComponent<HiAREngineBehaviour>();
        ScanBtn = transform.Find("BackGroundImageBottom/ScanBtn").GetComponent<Button>();
        backWardBtn = transform.Find("BackGroundImageTop/BackWardBtn").GetComponent<Button>();
    }


    void Start()
    {
        ScanBtn.onClick.AddListener(StartScan);
        backWardBtn.onClick.AddListener(BackWard);

    }




    void StartScan()
    {
        //hiarEnginBehaviour.StartRecognition();

		UIManager.Instance.Open (UIID.GetGiftUI);
    }
	
    void BackWard()
    {
		UIManager.Instance.Close (gameObject);
		UIManager.Instance.Open (UIID.Task);
    }

}
