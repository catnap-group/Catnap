using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class CatHandbookUI : MonoBehaviour 
{
	private bool _IsHideMenu = false;
	private bool _IsHideSceneMenu = false;
	private List<AreaDescription> _AreaDescription = new List<AreaDescription> ();
	private List<GameObject> _CatList = new List<GameObject>();

	// Use this for initialization
	void Start () 
    {
		transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
		transform.FindChild ("Top/SceneBtn/Scroll View").gameObject.SetActive (_IsHideSceneMenu);
		SetScene ();
		InitTangoScene ();
	}
		
	public class CatData
	{
		public float Progress = 1;
		public string Name = "";
		public string Desc1 = "";
		public string Desc2 = "";

		public CatData(string name, string desc1, string desc2, float progress)
		{
			Name = name;
			Desc1 = desc1;
			Desc2 = desc2;
			Progress = progress;
		}
	}

	private CatData[] _CatDatas = {
		new CatData("喵小白", "睡觉ing~zzz", "幼猫 7周 800克", 0.5f),
		new CatData("喵大白", "玩耍ing", "幼猫 10周 1800克", 1.0f),
	};

    public void InitTangoScene()
    {
		string prefabPath = "Prefabs/UI/SceneUI";
		GameObject content = transform.FindChild ("Top/SceneBtn/Scroll View/Viewport/Content").gameObject;
		AreaDescription[] areaDescriptionList = AreaDescription.GetList();
		if (areaDescriptionList == null)
			return;
		for (int i = 0; i < areaDescriptionList.Length; i++) {
			AreaDescription desc = areaDescriptionList [i];
			_AreaDescription.Add (desc);//保存所有的描述

			GameObject data = Instantiate (Resources.Load (prefabPath, typeof(GameObject))) as GameObject;
			data.transform.SetParent (content.transform, false);

			Text name = data.transform.FindChild ("desc").GetComponent<Text> ();
			name.text = desc.GetMetadata ().m_name;

			int pos = i;
			Button button = data.gameObject.GetComponent<Button> ();
			button.onClick.AddListener (delegate() {
				this.PressLoad(pos);
			});
		}
    }

	public void SetScene()
	{
		foreach (GameObject cat in _CatList) {
			Destroy (cat);
		}
		_CatList.Clear ();

		string catPrefab = "Prefabs/UI/CatUI";
		GameObject content = transform.FindChild("Center/Scroll View/Viewport/Content").gameObject;
		foreach (CatData data in _CatDatas)
		{
			GameObject cat = Instantiate(Resources.Load(catPrefab, typeof(GameObject))) as GameObject;
			cat.transform.SetParent(content.transform, false);
			_CatList.Add(cat);

			CatUI catScript = cat.GetComponent<CatUI>();
			catScript.SetProgress(data.Progress);
			catScript.SetDesc1(data.Desc1);
			catScript.SetDesc2(data.Desc2);
			catScript.SetName(data.Name);
			_CatList.Add (cat);
		}
	}

	public void PressLoad(int pos)
	{
		SetScene ();
		PressSceneMenu ();
		AreaDescription desc = _AreaDescription [pos];
		TangoService.Instance.m_curAreaDescriptionUUID = desc.m_uuid;
		GameScene.Instance.StartTango (false);
		UIManager.Instance.Close(gameObject);
		UIManager.Instance.Open (UIID.Main);
		//desc.GetMetadata().
	}

    public void PressAR()
    {
        Debug.Log("Press AR");
		GameScene.Instance.StartTango (true);
		UIManager.Instance.Close(gameObject);
		UIManager.Instance.Open (UIID.Main);
    }

    public void PressMenu()
    {
        Debug.Log("Press Menu");
        _IsHideMenu = _IsHideMenu ? false : true;
		transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
    }

	public void PressSceneMenu()
	{
		Debug.Log ("Press Scene Menu");
		_IsHideSceneMenu = _IsHideSceneMenu ? false : true;
		transform.FindChild ("Top/SceneBtn/Scroll View").gameObject.SetActive (_IsHideSceneMenu);
	}

    private void SetDesc1(string desc)
    {
        Text text = transform.FindChild("Down/desc1").GetComponent<Text>();
        text.text = desc;
    }

    private void SetDesc2(string desc)
    {
        Text text = transform.FindChild("Down/desc2").GetComponent<Text>();
        text.text = desc;
    }
}