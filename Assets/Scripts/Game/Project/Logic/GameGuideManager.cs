using UnityEngine;
using System.Collections;

public class GameGuideManager : UnityAllSceneSingleton<GameGuideManager> {

	public enum GuideState 
	{
		None,
		StartGuide,
		PutBox,
		GotoStore,
		PutGoodies,
		GotoStorage,
		PutCatHouse,
		OK,
	}

	public GameObject MainUI;
	private GuideState _State;

	void OnInit()
	{
		_State = GuideState.None;
	}

	public void SetState(GuideState state)
	{
		_State = state;
		Debug.Log (_State);
		if (_State == GuideState.StartGuide) {
			UIManager.UIData uiData = UIManager.Instance.Open (UIID.Main);
			MainUI = uiData.UIObject;
		} else if (_State == GuideState.PutBox) {
			UIManager.Instance.ShowMessage ("点击屏幕摆放猫盆");
		} else if (_State == GuideState.GotoStore) {
			UIManager.Instance.ShowMessage ("前往商店购买猫粮");
			MainUI ui = MainUI.GetComponent<MainUI> ();
			ui.ShowStoreBtn ();
		} else if (_State == GuideState.PutGoodies) {
			UIManager.Instance.ShowMessage ("点击猫盆放置猫粮");
		} else if (_State == GuideState.GotoStorage) {
			UIManager.Instance.ShowMessage ("前往仓库使用猫窝");
		} else if (_State == GuideState.PutCatHouse) {
			UIManager.Instance.ShowMessage ("点击屏幕摆放猫窝");
		} else if (_State == GuideState.OK) {
			MainUI ui = MainUI.GetComponent<MainUI>();
			ui.ShowHandbookBtn ();
			/*
			SceneCat cat = MapSceneManager.Instance.CreateSceneCat (104, Vector3.zero, Quaternion.identity);
			cat.thisT.localRotation = Quaternion.Euler (90, 0, 0);
			cat.thisT.position = Vector3.zero;
			#if !UNITY_EDITOR
			TangoManager.Instance.SceneUnit2ARUnit (cat);
			#endif*/
		}
	}

	public GuideState GetState()
	{
		return _State;
	}

	public Vector3 GetTouchPosition()
	{
		Vector3 vector3 = new Vector3 (0, 0, 0);
		#if UNITY_EDITOR
		vector3 = Input.mousePosition;
		#else
		vector3 = Input.GetTouch(0).position;
		#endif
		return vector3;
	}
}
