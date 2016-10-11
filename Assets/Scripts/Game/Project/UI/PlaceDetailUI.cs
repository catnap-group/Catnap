using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlaceDetailUI : MonoBehaviour
{
	public Text playerName;
	public Text distance;
	// Use this for initialization
	void Start ()
	{
	
	}
	PlaceInfo _info ;
	public void SetPlace(PlaceInfo info)
	{
		_info = info;
		playerName.text = _info.Name; 
		distance.text = _info.Distance.ToString() + "m";
	}
	public void PressHello()
	{
		UIManager.Instance.Close (gameObject);
		UIManager.UIData data = UIManager.Instance.Open (UIID.Talk);
		GameObject obj = data.UIObject;
		if (obj != null) {
			TalkUI pdui = obj.GetComponent<TalkUI> ();
			pdui.SetName (_info.Name);
		}

	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}

