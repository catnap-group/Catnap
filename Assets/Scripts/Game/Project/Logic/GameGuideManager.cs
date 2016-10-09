using UnityEngine;
using System.Collections;

public class GameGuideManager : UnityAllSceneSingleton<GameGuideManager> {

	public enum GuideState 
	{
		None,
		PutBox,
		GotoStore,
		PutGoodies,
		GotoStorage,
		PutCatHouse,
		OK,
	}

	private GuideState _State;

	void OnInit()
	{
		_State = GuideState.None;
	}

	public void SetState(GuideState state)
	{
		_State = state;
		Debug.Log (_State);
		if (_State == GuideState.PutBox) {
			UIManager.Instance.ShowMessage ("请拖动猫盆进行摆放");
		} else if (_State == GuideState.GotoStore) {
			UIManager.Instance.ShowMessage ("前往商店购买猫粮");
		} else if (_State == GuideState.PutGoodies) {
			UIManager.Instance.ShowMessage ("点击猫盆放置猫粮");
		} else if (_State == GuideState.GotoStorage) {
			UIManager.Instance.ShowMessage ("前往仓库使用猫窝");
		} else if (_State == GuideState.PutCatHouse) {
			UIManager.Instance.ShowMessage ("摆放猫窝");
			GameObject obj = GameObject.Instantiate (Resources.Load("Prefabs/Cat/CatHome", typeof(GameObject))) as GameObject;
			obj.transform.localPosition = new Vector3 (0, 0, 1);
			#if !UNITY_EDITOR
			//投射到真实空间去
			//TangoManager.Instance.SceneUnit2ARUnit(obj);
			#endif
		} else if (_State == GuideState.OK) {
			GameObject obj = GameObject.Instantiate (Resources.Load ("Prefabs/Cat/Cat", typeof(GameObject))) as GameObject;
			obj.transform.localPosition = new Vector3(0, 0, 1);

			UIManager.UIData uiData = UIManager.Instance.Open (UIID.Main);
			MainUI ui = uiData.UIObject.gameObject.GetComponent<MainUI>();
			ui.ShowHandbookBtn ();

			#if !UNITY_EDITOR
			//投射到真实空间去
			//TangoManager.Instance.SceneUnit2ARUnit(obj);
			#endif
		}
	}

	public GuideState GetState()
	{
		return _State;
	}
}
