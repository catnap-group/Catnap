using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CatStoreUI : MonoBehaviour 
{
	private int _TotalNumberToggle = 0;
	private int _TotalNumberGoodies = 0;
	private int _CurrentToggleNumber = 0;
	private Goodies _CurrentGoodies = null;
    private bool _IsHideMenu = false;

	public class GoodiesData 
	{
		public int Price = 0;
		public string Name = "";
		public string IconPath = "";
		public int GoodiesID;
		public GoodiesData(string name, int price, string iconPath, int goodiesID)
		{
			Name = name;
			Price = price;
			IconPath = iconPath;
			GoodiesID = goodiesID;
		}
	}

	private  GoodiesData[] _GoodiesDatas = 
    { 
		new GoodiesData("普通猫盆", 100, "UI/Goodies/img_cat_bowl01", 105),
		new GoodiesData("普通猫窝", 100, "UI/Goodies/img_cat_house", 106),
		new GoodiesData("普通猫粮", 100, "UI/Goodies/img_cat_box", 0),
		new GoodiesData("高级猫粮", 100, "UI/Goodies/img_cat_box", 0),
		new GoodiesData("高级猫窝", 100, "UI/Goodies/img_cat_house", 106),
	};

	// Use this for initialization
	void Start () 
	{
		_TotalNumberGoodies = _GoodiesDatas.Length;
		_TotalNumberToggle = _TotalNumberGoodies / 4;
		if (_TotalNumberGoodies % 4 > 0) {
			_TotalNumberToggle++;
		}
		InitGoodies ();
		transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
		
	void InitGoodies() 
	{
		GameObject goodiesGroup = transform.FindChild ("Center/GoodiesGroup").gameObject;
		for (int i = 1; i <= _TotalNumberToggle; i++) 
		{
			int toggleNum = i;
			GameObject toggle = Instantiate (Resources.Load ("Prefabs/UI/GoodiesToggle", typeof(GameObject))) as GameObject;
			toggle.transform.SetParent (goodiesGroup.transform, false);
			toggle.name = string.Format("Toggle{0}", i);
			Toggle toggleComp = toggle.GetComponent<Toggle> ();
			toggleComp.group = goodiesGroup.GetComponent<ToggleGroup>();
			toggleComp.onValueChanged.AddListener (delegate(bool arg0) {
				this.PressToggle(arg0, toggle, toggleNum);	
			});
		}

		if (_TotalNumberToggle > 0) 
		{
			string toggleName = "Center/GoodiesGroup/Toggle1";
			Toggle toggle1 = transform.FindChild (toggleName).GetComponent<Toggle> ();
			toggle1.isOn = true;
		}
	}

	public void SwitchToggle(int toggleNum)
	{
		Debug.Log (toggleNum);
		Debug.Log (_TotalNumberToggle);
		if (toggleNum > 0 && toggleNum <= _TotalNumberToggle) 
		{
			_CurrentToggleNumber = toggleNum;
			RefreshGoodies ();
		}
	}

	private void RefreshGoodies()
	{
		int count = 1;
		int beginGoodiesNumber = (_CurrentToggleNumber - 1) * 4;
		for (int i = beginGoodiesNumber; i < beginGoodiesNumber + 4 && i < _TotalNumberGoodies; i++, count++) {
			GameObject goodies = transform.FindChild (string.Format ("Center/Goodies{0}", count)).gameObject;
			GoodiesData data = _GoodiesDatas [i];
			Goodies goodiesComp = goodies.GetComponent<Goodies>();
			goodies.SetActive (true);

			Button btn = goodies.transform.FindChild ("Icon").GetComponent<Button> ();
			btn.onClick.AddListener (delegate() {
				this.PressGoodies(goodiesComp);
			});

			ColorBlock colors = new ColorBlock ();
			colors.colorMultiplier = 1.0f;
			colors.normalColor = Color.white;
			colors.disabledColor = Color.gray;
			colors.pressedColor = new UnityEngine.Color (225.0f / 255.0f, 75.0f / 255.0f, 75.0f / 255.0f, 1.0f);
			colors.highlightedColor = new UnityEngine.Color (225.0f / 255.0f, 75.0f / 255.0f, 75.0f / 255.0f, 1.0f);
			btn.colors = colors;

			goodiesComp.SetName (data.Name);
			goodiesComp.SetPrice (data.Price);
			goodiesComp.SetIconPath (data.IconPath);
		}

		for (int i = count; i <= 4; i++) 
		{
			GameObject goodies = transform.FindChild (string.Format ("Center/Goodies{0}", i)).gameObject;
			goodies.SetActive (false);
		}
	}

	public void PressBuyGoodies()
	{
		Debug.Log ("Press Buy Goodies");
		if (_CurrentGoodies) {
			Debug.Log (string.Format ("Buy Goodies {0}", _CurrentGoodies.GetName ()));
			Debug.Log (GameGuideManager.Instance.GetState ());
			if (GameGuideManager.Instance.GetState () == GameGuideManager.GuideState.GotoStore) {
				GameGuideManager.Instance.SetState (GameGuideManager.GuideState.PutGoodies);
				PressBack ();
			}
		}
	}

	public void PressBack()
	{
		Debug.Log ("Press Back");
		UIManager.Instance.Close (gameObject);
	}

	public void PressGoodies(Goodies goodies)
	{
		_CurrentGoodies = goodies;
		Text desc = transform.FindChild ("Down/Number").GetComponent<Text> ();
		desc.text = string.Format("鱼干数：{0}", goodies.GetPrice ());
	}

	public void PressToggle(bool isOn, GameObject toggle, int toggleNum)
	{
		Debug.Log ("Press Toggle");
		_CurrentGoodies = null;
		Text desc = transform.FindChild ("Down/Number").GetComponent<Text> ();
		desc.text = "鱼干数：XXX";
		SwitchToggle (toggleNum);
	}

    public void PressMenu()
    {
        _IsHideMenu = _IsHideMenu ? false : true;
        transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
    }
}
