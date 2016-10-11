using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour {

	public enum BoxState {
		None,
		EmptyBox,
		FullBox,
	}

	private BoxState _State;
	private bool _FirstDrag = true;

	// Use this for initialization
	void Start () 
	{
		_State = BoxState.EmptyBox;
	}

	public void SetState(BoxState state)
	{
		_State = state;
		if (state == BoxState.EmptyBox) {
			//gameObject.GetComponent<FollowMouse> ().enabled = false;
		} else if (state == BoxState.FullBox) {
			//gameObject.GetComponent<FollowMouse> ().enabled = false;
		}
	}

	public BoxState GetState()
	{
		return _State;
	}

	void Update()
	{
		if (Input.GetMouseButton (0)) {
			if (GetState () == BoxState.EmptyBox && StorageUI.CurrentGoodiesData != null) {
				if (StorageUI.CurrentGoodiesData.GoodiesID == 0) {
					SetState (BoxState.FullBox);
					EventListener.Broadcast (ObjectEvent.CallEat, gameObject);
				}
			}
		}
	}
}
