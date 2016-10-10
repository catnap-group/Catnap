using UnityEngine;
using System.Collections;

public class GiftGetUI : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	}
	public void PressBack()
	{
		Debug.Log ("Press Back");
		UIManager.Instance.Close (gameObject);
	}
	// Update is called once per frame
	void Update ()
	{
	}
	public void PressHello()
	{
		CatSceneManager.Instance.SetNextScene (SceneID.Search);
	}
	public void PressCancle()
	{
		//CatSceneManager.Instance.SetNextScene (SceneID.Game);
	}
}

