using UnityEngine;
using System.Collections;
 interface IGameInput
{
	bool IsClickDown{ get;}
	bool IsClickUp{ get;}
	bool IsClicking{ get;}
	bool HasTouch{ get; }
	bool IsMove{ get; }
	Vector3 MousePosition{ get; }
	int TouchCount{ get; }
	int GetFingerID{ get; }
}
 class WinGameInput : IGameInput
{
	public bool IsClickDown
	{
		get{
			return Input.GetMouseButtonDown (0);
		}
	}
	public bool IsClickUp
	{
		get{
			return Input.GetMouseButtonUp (0);
		}
	}
	public bool IsClicking
	{
		get{
			return Input.GetMouseButton (0);
		}
	}
	public bool IsMove
	{
		get{
			return IsClicking;
		}
	}
	public int GetFingerID
	{
		get{ return -1;}
	}
	public Vector3 MousePosition
	{
		get{ return Input.mousePosition;}
	}
	public bool HasTouch{ get {  return true; } }
	public int TouchCount{ get { return 1; } }
}
public class SingleTouchGameInput : IGameInput
{
	public bool IsClickDown
	{
		get{
			return Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began;
		}
	}
	public bool IsClickUp
	{
		get{
			return Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Ended;
		}
	}
	public bool IsMove
	{
		get{
			return Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved;
		}
	}
	public bool IsClicking
	{
		get{
			return Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Stationary;
		}
	}
	public int GetFingerID
	{
		get{ return Input.GetTouch (0).fingerId;}
	}
	public Vector3 MousePosition
	{
		get {
			if (Input.touchCount == 1) {
				return Input.GetTouch (0).position;
			} else {
				return Input.mousePosition;
			}
		}
	}
	public bool HasTouch
	{
		get{ 
			return Input.touchCount > 0;
		}
	}
	public int TouchCount
	{
		get{
			return Input.touchCount;
		}
	}
}

public class TouchListener : UnityAllSceneSingletonVisible<TouchListener>
{

	float touchTimer = 0f;
	private static IGameInput _GameInput;
	public static bool IsTouchDevice{
		get {
			return Application.platform == RuntimePlatform.IPhonePlayer
			|| Application.platform == RuntimePlatform.Android;
		}
	}

	public override void OnInit ()
	{
		if (IsTouchDevice) {
			_GameInput = new SingleTouchGameInput ();
		} else {
			_GameInput = new WinGameInput ();
		}
	}
	public override void FixedUpdate ()
	{
		if (!_GameInput.HasTouch)
			return;
        //if (UIManager.Instance.IsCursorOnUI (_GameInput.GetFingerID))//exclude the ui
        //    return;
		//input lestener

	}

}

 