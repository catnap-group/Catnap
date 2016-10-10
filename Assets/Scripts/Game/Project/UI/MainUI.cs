using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainUI : MonoBehaviour 
{
	private bool _IsHideMenu = false;

	void Start()
	{
		transform.FindChild ("PutBtn").gameObject.SetActive (false);
		transform.FindChild ("MenuBtn/MenuUI").gameObject.SetActive (false);
	}

	public void PressSaveTangoData()
	{
		TangoManager.Instance.Save ();
	}

	public void PressLoadTangoData()
	{
		TangoManager.Instance.Load ();
	}

	void PressStore()
	{
		Debug.Log ("Press Store");
		UIManager.Instance.Open (UIID.CatStore);
	}

	void PressHandbook()
	{
		Debug.Log ("Press hand book");
		UIManager.Instance.Open (UIID.CatHandbook);
	}

	void PressPut()
	{/*
		if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.PutGoodies) {
			GameGuideManager.Instance.SetState (GameGuideManager.GuideState.GotoStorage);
			ShowPutBtn (false);
		} else if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.PutCatHouse) {
			GameGuideManager.Instance.SetState (GameGuideManager.GuideState.OK);
			ShowPutBtn (false);
		}*/
	}

	public void ShowStoreBtn()
	{
		transform.FindChild ("CatStoreBtn").gameObject.SetActive (true);
	}

	public void ShowHandbookBtn()
	{
		transform.FindChild ("CatHandbookBtn").gameObject.SetActive (true);
	}

	public void ShowPutBtn(bool active)
	{
		transform.FindChild ("PutBtn").gameObject.SetActive (active);
	}

	public void PressScreen()
	{
		if (StorageUI.CurrentGoodiesData != null) {
			Vector3 touchPosInScreen = GameGuideManager.Instance.GetTouchPosition ();
			Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position); 
			Vector3 touchPosInWorld = Camera.main.ScreenToWorldPoint (new Vector3 (touchPosInScreen.x, touchPosInScreen.y, 1));
			SceneCatLittle little = MapSceneManager.Instance.CreateSceneCatLittle (StorageUI.CurrentGoodiesData.GoodiesID, Vector3.zero, Quaternion.identity, false);
			if (StorageUI.CurrentGoodiesData.GoodiesID == 105) {
			//	little.thisT.rotation = new Quaternion (Camera.main.transform.rotation.x + Quaternion.Euler (-90, 0, 0).x, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z, Camera.main.transform.rotation.w);
				little.thisT.rotation = Camera.main.transform.rotation;
			} else {
				little.thisT.rotation = Camera.main.transform.rotation;
			}
			little.thisT.position = touchPosInWorld;
			Debug.Log (touchPosInScreen);
			Debug.Log (touchPosInWorld);
			#if !UNITY_EDITOR
			TangoManager.Instance.SceneUnit2ARUnit (little);
			#endif
			StorageUI.CurrentGoodiesData = null;
		}
	}

	public void PressBack()
	{
		UIManager.Instance.Close (gameObject);
	}

	public void PressMenu()
	{
		_IsHideMenu = _IsHideMenu ? false : true;
		transform.FindChild("MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
	}
}
