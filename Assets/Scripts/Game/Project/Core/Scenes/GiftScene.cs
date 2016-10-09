using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GiftScene : SceneBase
{

	static public GiftScene Instance = null;
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
	}
	IEnumerator LoadScene()
	{

		yield return null;
		OnSceneLoaded();
	}
	public void OnSceneLoaded()
	{
        UIManager.Instance.Open(UIID.GiftUI);

	}
	public override void Unload ()
	{
		base.Unload ();
        UIManager.Instance.Close(UIID.GiftUI);
        Resources.UnloadUnusedAssets();
	}

}

