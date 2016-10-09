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

		Button saveBtn = transform.FindChild ("SaveTangoDataBtn").GetComponent<Button> ();
		saveBtn.onClick.AddListener (delegate() {
			this.PressSaveTangoData();	
		});

		Button loadBtn = transform.FindChild ("LoadTangoDataBtn").GetComponent<Button> ();
		loadBtn.onClick.AddListener (delegate() {
			this.PressLoadTangoData();
		});

		GameGuideManager.Instance.SetState (GameGuideManager.GuideState.PutBox);
	}

	void PressSaveTangoData()
	{
		TangoManager.Instance.Save ();
	}

	void PressLoadTangoData()
	{
		//TangoManager.Instance.Load ();
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
		if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.PutGoodies) {
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

	public void PressScreen()
	{
		Debug.Log (GameGuideManager.Instance.GetState ());
		if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.PutBox) {
			Vector3 touchPosInScreen = GameGuideManager.Instance.GetTouchPosition ();
			Vector3 touchPosInWorld = Camera.main.ScreenToWorldPoint (new Vector3 (touchPosInScreen.x, touchPosInScreen.y, 1));
			SceneCatLittle box = MapSceneManager.Instance.CreateSceneCatLittle (105, Vector3.zero, Quaternion.identity, false);
			//box.thisT.localRotation = Quaternion.Euler (90, 0, 0);
			Quaternion quaternion = Camera.main.transform.rotation;
			box.thisT.rotation = Quaternion.Euler(-90, 0, 0);
			box.thisT.position = touchPosInWorld;
			#if !UNITY_EDITOR
			TangoManager.Instance.SceneUnit2ARUnit (box);
			#endif
			GameGuideManager.Instance.SetState (GameGuideManager.GuideState.GotoStore);
		} else if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.PutCatHouse) {
			Vector3 touchPosInScreen = GameGuideManager.Instance.GetTouchPosition ();
			Vector3 touchPosInWorld = Camera.main.ScreenToWorldPoint (new Vector3 (touchPosInScreen.x, touchPosInScreen.y, 1));
			SceneCatLittle catHome = MapSceneManager.Instance.CreateSceneCatLittle (106, Vector3.zero, Quaternion.identity, false);
			catHome.thisT.localRotation = Camera.main.transform.rotation;
			catHome.thisT.position = touchPosInWorld;
			#if !UNITY_EDITOR
			TangoManager.Instance.SceneUnit2ARUnit(catHome);
			#endif
			GameGuideManager.Instance.SetState (GameGuideManager.GuideState.OK);
		}
	}
}
