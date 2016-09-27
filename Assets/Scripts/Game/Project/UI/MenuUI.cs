using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuUI : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {

	}

    public void PressStorage()
    {
        UIManager.Instance.Open(UIID.Storage);
    }
    public void PressStore()
    {
        UIManager.Instance.Open(UIID.CatStore);
    }

    public void PressTask()
    {
        UIManager.Instance.Open(UIID.Task);
    }

    public void PressHandbook()
    {
        UIManager.Instance.Open(UIID.CatHandbook);
    }
}
