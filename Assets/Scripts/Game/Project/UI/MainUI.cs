using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainUI : MonoBehaviour 
{
	private GameObject _Box;

	void Start()
	{
		transform.FindChild ("CatStoreBtn").gameObject.SetActive (false);
		transform.FindChild ("CatHandbookBtn").gameObject.SetActive (false);
		transform.FindChild ("PutBtn").gameObject.SetActive (false);

		Button storeBtn = transform.FindChild ("CatStoreBtn").GetComponent <Button>();
		storeBtn.onClick.AddListener (delegate() {
			this.PressStore();	
		});

		Button handbookBtn = transform.FindChild ("CatHandbookBtn").GetComponent<Button> ();
		handbookBtn.onClick.AddListener (delegate() {
			this.PressHandbook();	 
		});

		Button putBtn = transform.FindChild ("PutBtn").GetComponent<Button> ();
		putBtn.onClick.AddListener (delegate() {
			this.PressPut();	
		});

		//GameGuideManager.Instance.SetState (GameGuideManager.GuideState.FirstGuide);
		GameObject box = GameObject.Instantiate(Resources.Load ("Prefabs/Cat/Box", typeof(GameObject))) as GameObject;
		box.transform.localScale = new Vector3 (1, 1, 1);
		box.transform.position = new Vector3(0, 0, 1);
		_Box = box;
		GameGuideManager.Instance.SetState (GameGuideManager.GuideState.PutBox);
		#if !UNITY_EDITOR
		//投射到真实空间去
		//TangoManager.Instance.SceneUnit2ARUnit(box);
		#endif
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
	{
		if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.PutBox) {
			Box box = _Box.GetComponent<Box> ();
			Debug.Log (box.GetState ());
			box.SetState (Box.BoxState.EmptyBox);
			ShowPutBtn (false);
			ShowStoreBtn ();
			GameGuideManager.Instance.SetState (GameGuideManager.GuideState.GotoStore);
		} else if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.PutGoodies) {
			GameGuideManager.Instance.SetState (GameGuideManager.GuideState.GotoStorage);
			ShowPutBtn (false);
		} else if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.PutCatHouse) {
			GameGuideManager.Instance.SetState (GameGuideManager.GuideState.OK);
			ShowPutBtn (false);
		}
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
}
