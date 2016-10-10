using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SearchScene : SceneBase
{

	static public SearchScene Instance = null;
	private Transform thisT;
	public ARMange armanager;
	void Awake()
	{
		Instance = this;
		#if UNITY_EDITOR
		if (!CatSceneManager.Created) {
			CatSceneManager.Create ();
			GameManager.Create ();
		}
		#endif

		thisT = transform;
		StartCoroutine(LoadScene());
	}
	void OnGUI()
	{
		if (GUI.Button (new Rect (0, 0, 100, 150),  "<size=20>back to game</size>")) {
			CatSceneManager.Instance.SetNextScene (SceneID.Game);
		}
	}
	IEnumerator LoadScene()
	{

		yield return null;
		OnSceneLoaded();
	}
	public void OnSceneLoaded()
	{
		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().InitCustomArgs();
		CatnapWebMgr.Instance.CastFor<CatnapWebMgr> ().SetCustomArg ("longitude", 113.943372f);//TestGPS.Instance.longitude);
		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().SetCustomArg("latitude",22.5f); //TestGPS.Instance.latitude);
		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().RequestByWRI(EWebRequestId.MSG_GET_NEAR_PLAYER, OnLoginResponse, OnWebError);

//		UIManager.UIData data = UIManager.Instance.Open (UIID.PlaceDetail);
//		GameObject obj = data.UIObject;
//		if (obj != null) {
//			PlaceDetailUI pdui = obj.GetComponent<PlaceDetailUI> ();
//			PlaceInfo info = new PlaceInfo ();
//			info.Name = "马古斯";
//			info.Distance = 1000;
//			pdui.SetPlace (info);
//		}
	}
	void OnLoginResponse(uint id, object obj, object localArg)
	{
		Debug.Log ("id" + id);
		if (obj == null) {
			Debug.LogError ("no response data!!!");
			return;
		}
		armanager.places.Clear ();
		armanager.location.Latitude = 22.5f;
		armanager.location.Longitude = 113.943372f;
		NearByPlayer nbp = obj as NearByPlayer;
		for (int i = 0; i < nbp.data.Count; i++) {
			PlaceInfo info = new PlaceInfo ();
			Debug.Log ("players:" + nbp.data[i].uid);
			Debug.Log ("name:" + nbp.data[i].uname);
			Debug.Log ("latitude:" + nbp.data[i].latitude);
			Debug.Log ("longitude:" + nbp.data[i].longitude);
			Debug.Log ("distance:" + nbp.data[i].distance);
			info.id = nbp.data [i].uid;
			info.Name = nbp.data [i].uname;
			info.Latitude = nbp.data [i].latitude;
			info.Longitude = nbp.data [i].longitude;
			info.Distance = nbp.data [i].distance;
			armanager.places.Add (info);
		}
		armanager.ShowPlaces ();
	}
	void OnWebError(uint id, string msg)
	{
		Debug.Log(msg);
	}
	public override void Unload ()
	{
		base.Unload ();
	}
}

