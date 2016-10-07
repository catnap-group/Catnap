using UnityEngine;
using System.Collections;

public class CatHome : MonoBehaviour {

	public enum HomeState {
		None,
		DragBox,
		PutState,
	}

	private HomeState _State;
	private bool _FirstDrag = true;

	// Use this for initialization
	void Start () 
	{
		SetState (HomeState.DragBox);
		StartCoroutine(OnMouseDown());
	}

	public void SetState(HomeState state)
	{
		_State = state;
	}

	public HomeState GetState()
	{
		return _State;
	}

	IEnumerator OnMouseDown()
	{
		Debug.Log (GetState ());
		if (GetState() == HomeState.DragBox) {
			UIManager.UIData uiData = UIManager.Instance.Open (UIID.Main);
			MainUI ui = uiData.UIObject.GetComponent<MainUI> ();
			ui.ShowPutBtn (false);

			Vector3 screenPos = GetPressPosition ();
			//将物体由世界坐标系转换为屏幕坐标系
			Vector3 screenSpace = Camera.main.WorldToScreenPoint (transform.position);//三维物体坐标转屏幕坐标
			//完成两个步骤 1.由于鼠标的坐标系是2维，需要转换成3维的世界坐标系 
			//    //             2.只有3维坐标情况下才能来计算鼠标位置与物理的距离，offset即是距离
			//将鼠标屏幕坐标转为三维坐标，再算出物体位置与鼠标之间的距离
			Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (screenPos.x, screenPos.y, screenSpace.z));
			while (Input.GetMouseButton (0)) {
				screenPos = GetPressPosition ();
				//得到现在鼠标的2维坐标系位置
				Vector3 curScreenSpace = new Vector3 (screenPos.x, screenPos.y, screenSpace.z);
				//将当前鼠标的2维位置转换成3维位置，再加上鼠标的移动量
				Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace) + offset;
				//curPosition就是物体应该的移动向量赋给transform的position属性
				transform.position = curPosition;
				yield return new WaitForFixedUpdate (); //这个很重要，循环执行
			}


			ui.ShowPutBtn (!_FirstDrag);
			_FirstDrag = false;
		}
	}

	private Vector3 GetPressPosition()
	{
		Vector3 screenPos;
		#if UNITY_EDITOR
		screenPos = Input.mousePosition;
		#else
		screenPos = Input.GetTouch(0).position;
		#endif
		return screenPos;
	}
}
