using UnityEngine;
using System.Collections;

public class Place : MonoBehaviour
{
	public PlaceInfo info;
	// Use this for initialization
	void Start ()
	{
	
	}
	public void OpenDetail()
	{
		
		UIManager.UIData data = UIManager.Instance.Open (UIID.PlaceDetail);
		GameObject obj = data.UIObject;
		if (obj != null) {
			PlaceDetailUI pdui = obj.GetComponent<PlaceDetailUI> ();
			pdui.SetPlace (info);
		}
	
	
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}

