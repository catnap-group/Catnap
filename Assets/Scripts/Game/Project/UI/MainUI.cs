using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainUI : MonoBehaviour 
{
	private GameObject _Box;

	public enum GuideState
	{
		None,
		PutBox,
		GotoStore,
		BuyGoodies,
		ChooseGoodies,
		PutGoodies,
		PutCatHouse,
	}

	private GuideState _State = GuideState.None;

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
		box.transform.localScale = new Vector3 (5, 5, 5);
		box.transform.position = new Vector3(1, 1, 0);
		_Box = box;
		GameGuideManager.Instance.SetState (GameGuideManager.GuideState.PutBox);
	}

	void PressStore()
	{
		Debug.Log ("Press Store");
		UIManager.Instance.Open (UIID.CatStore);
		GameGuideManager.Instance.SetState (GameGuideManager.GuideState.BuyGoodies);
	}

	void PressHandbook()
	{
		Debug.Log ("Press hand book");
		UIManager.Instance.Open (UIID.CatHandbook);
	}

	void PressPut()
	{
		if (_State == GuideState.PutBox) {
			Box box = _Box.GetComponent<Box> ();
			Debug.Log (box.GetState ());
			box.SetState (Box.BoxState.EmptyBox);
			ShowPutBtn (false);
			ShowStoreBtn ();
			GameGuideManager.Instance.SetState (GameGuideManager.GuideState.GotoStore);
		} else if (_State == GuideState.PutCatHouse) {
			
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
		Debug.Log ("Put Put");
		transform.FindChild ("PutBtn").gameObject.SetActive (active);
	}
}
