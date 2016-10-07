using UnityEngine;
using System.Collections;

public class BoxGuide : MonoBehaviour {

	private bool _FirstPress = false;

	// Use this for initialization
	void Start () 
	{
		Box box = gameObject.GetComponent<Box> ();
		box.SetState (Box.BoxState.DragBox);
		UIManager.Instance.ShowMessage ("请拖动猫盆进行摆放");
	}
	
	void OnMouseExit()
	{
		if (_FirstPress == false) {
			Box box = gameObject.GetComponent<Box>();
			box.SetState (Box.BoxState.EmptyBox);
			UIManager.Instance.ShowMessage ("请为猫盆添加猫粮");
		}
		_FirstPress = true;
	}
}
