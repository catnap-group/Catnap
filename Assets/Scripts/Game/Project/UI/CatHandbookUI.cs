using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CatHandbookUI : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        InitHandbook();
		transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
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

    private bool _IsHideMenu = false;
    private CatData[] _CatDatas = {
        new CatData("喵小白", "睡觉ing~zzz", "幼猫 7周 800克", 0.5f),
        new CatData("喵大白", "玩耍ing", "幼猫 10周 1800克", 1.0f),
    };

    public void InitHandbook()
    {
        string catPrefab = "Prefabs/UI/CatUI";
        GameObject content = transform.FindChild("Center/Scroll View/Viewport/Content").gameObject;
        foreach (CatData data in _CatDatas)
        {
            GameObject cat = Instantiate(Resources.Load(catPrefab, typeof(GameObject))) as GameObject;
            cat.transform.SetParent(content.transform, false);
            
            CatUI catScript = cat.GetComponent<CatUI>();
            catScript.SetProgress(data.Progress);
            catScript.SetDesc1(data.Desc1);
            catScript.SetDesc2(data.Desc2);
            catScript.SetName(data.Name);
        }
    }

    public void PressAR()
    {
        Debug.Log("Press AR");
    }

    public void PressMenu()
    {
        Debug.Log("Press Menu");
        _IsHideMenu = _IsHideMenu ? false : true;
		transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
    }

    public void PressBack()
    {
        Debug.Log("Press Back");
		UIManager.Instance.Close (gameObject);
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