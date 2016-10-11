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
		if (StorageUI.CurrentGoodiesData != null && StorageUI.CurrentGoodiesData.GoodiesID != 0) {
			Vector3 touchPosInScreen = GameGuideManager.Instance.GetTouchPosition ();
			Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position); 
			Vector3 touchPosInWorld = Camera.main.ScreenToWorldPoint (new Vector3 (touchPosInScreen.x, touchPosInScreen.y, 1));
			if (StorageUI.CurrentGoodiesData.GoodiesID == 105) {
				//	little.thisT.rotation = new Quaternion (Camera.main.transform.rotation.x + Quaternion.Euler (-90, 0, 0).x, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z, Camera.main.transform.rotation.w);
				SceneCatLittle little = MapSceneManager.Instance.CreateSceneCatLittle (StorageUI.CurrentGoodiesData.GoodiesID, Vector3.zero, Quaternion.identity, false);
				little.thisT.rotation = Camera.main.transform.rotation;
				little.thisT.position = touchPosInWorld;
				little.thisT.gameObject.SetActive (false);
				#if !UNITY_EDITOR
				TangoManager.Instance.SceneUnit2ARUnit (little);
				#endif
			} else if (StorageUI.CurrentGoodiesData.GoodiesID == 107 || StorageUI.CurrentGoodiesData.GoodiesID == 108) {
				SceneCat little = MapSceneManager.Instance.CreateSceneCat (StorageUI.CurrentGoodiesData.GoodiesID, Vector3.zero, Quaternion.identity);
				little.thisT.rotation = Camera.main.transform.rotation;
				little.thisT.position = touchPosInWorld;
				little.thisT.gameObject.SetActive (false);
				#if !UNITY_EDITOR
				TangoManager.Instance.SceneUnit2ARUnit (little);
				#endif
			} else {
				SceneCatLittle little = MapSceneManager.Instance.CreateSceneCatLittle (StorageUI.CurrentGoodiesData.GoodiesID, Vector3.zero, Quaternion.identity, false);
				little.thisT.rotation = Camera.main.transform.rotation;
				little.thisT.position = touchPosInWorld;
				little.thisT.gameObject.SetActive (false);
				#if !UNITY_EDITOR
				TangoManager.Instance.SceneUnit2ARUnit (little);
				#endif
			}

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

	public void PressSound()
	{
		Invoke("SentEvent", 1);
	}

	void SentEvent()
	{
		EventListener.Broadcast (ObjectEvent.SendSound);
	}
}
