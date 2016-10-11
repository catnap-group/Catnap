using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GiftShopScene : SceneBase
{

	static public GiftShopScene Instance = null;
	public GameObject giftbox;
	private Transform thisT;
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
//		if (GUI.Button (new Rect (0, 0, 100, 150),  "<size=20>back to game</size>")) {
//			CatSceneManager.Instance.SetNextScene (SceneID.Game);
//		}

		if (GUI.Button (new Rect (0, 200, 100, 150),  "<size=20>search</size>")) {
			CatSceneManager.Instance.SetNextScene (SceneID.Search);
		}
	}
	IEnumerator LoadScene()
	{

		yield return null;
		OnSceneLoaded();
	}
	public void DropDown()
	{
		Animation ani = giftbox.GetComponentInChildren<Animation> ();
		ani.Play ();
		giftbox.SetActive (true);
		StartCoroutine (delayPlay (2));
	}
	IEnumerator delayPlay(float time)
	{
		yield return new WaitForSeconds (time);
		giftbox.SetActive (false);
		UIManager.Instance.Close(UIID.GiftShopUI);
		UIManager.Instance.Open(UIID.GetGiftUI);
	}
	public void OnSceneLoaded()
	{
		UIManager.Instance.Open(UIID.GiftShopUI);

	}
	public override void Unload ()
	{
		base.Unload ();
		UIManager.Instance.Close(UIID.GiftShopUI);
        Resources.UnloadUnusedAssets();
	}

}

