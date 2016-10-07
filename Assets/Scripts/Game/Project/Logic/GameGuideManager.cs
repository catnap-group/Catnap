using UnityEngine;
using System.Collections;

public class GameGuideManager : UnityAllSceneSingleton<GameGuideManager> {

	public enum GuideState 
	{
		None,
		PutBox,
		GotoStore,
		BuyGoodies,
		PutGoodies,
		GotoStorage,
		PutCatHouse,
	}

	private GuideState _State;

	void OnInit()
	{
		_State = GuideState.None;
	}

	public void SetState(GuideState state)
	{
		_State = state;
		if (_State == GuideState.PutBox) {
			UIManager.Instance.ShowMessage ("请拖动猫盆进行摆放");
		} else if (_State == GuideState.GotoStore) {
			UIManager.Instance.ShowMessage ("前往商店购买猫粮");
		} else if (_State == GuideState.PutGoodies) {
			UIManager.Instance.ShowMessage ("点击猫盆放置猫粮");
		}
	}

	public GuideState GetState()
	{
		return _State;
	}
}
