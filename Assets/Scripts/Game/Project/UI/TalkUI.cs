using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class TalkUI : MonoBehaviour
{
	public Text name;
	public GameObject panel;
	public InputField ifield;
	public GameObject metalk;
	public GameObject othertalk;
	// Use this for initialization
	void Start ()
	{
		
	}
	public void SetName(string aName)
	{
		name.text = aName;
	}
	public void OnPressBack()
	{
		CatSceneManager.Instance.SetNextScene (SceneID.Game);
	}
	public void OnSend()
	{
		GameObject meTalk = GameObject.Instantiate(metalk);
		MeTalk me = meTalk.GetComponent<MeTalk> ();
		meTalk.transform.parent = panel.transform;
		meTalk.transform.localScale = new Vector3 (1, 1, 1);
		meTalk.SetActive (true);
		me.SetMessage (ifield.text);
		ifield.text = "";
		StartCoroutine (OnResponse ());
		Check ();
	}
	IEnumerator OnResponse()
	{
		yield return new WaitForSeconds (2);
		GameObject otherTalk = GameObject.Instantiate(othertalk);
		OtherTalk other = otherTalk.GetComponent<OtherTalk> ();
		otherTalk.transform.parent = panel.transform;
		otherTalk.transform.localScale = new Vector3 (1, 1, 1);
		otherTalk.SetActive (true);
		other.SetMessage ("您好");
		Check ();
	}
	void Check()
	{
		if (panel.transform.childCount > 6) {
			Transform obj =  panel.transform.GetChild (0);
			obj.parent = null;
			GameObject.Destroy (obj.gameObject);
		}
	}
	// Update is called once per frame
	void Update ()
	{
	}
}

