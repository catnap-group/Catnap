using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public enum UIID 
{
    NULL,
    CatStore,               //猫咪商店
    Storage,                //物品仓库
    Task,                   //猫咪任务
    CatHandbook,            //猫咪图鉴
};

public class UIManager : UnityAllSceneSingleton<UIManager> 
{
    public class UIData
    {
        public UIID ID = UIID.NULL;
        public string PrefabPath = "";
        public GameObject UIObject;

        public UIData(UIID id, string prefabPath, GameObject uiObject)
        {
            ID = id;
            UIObject = uiObject;
            PrefabPath = prefabPath;
        }
    }

    private GameObject _UIRoot;
    private GameObject _UICanvas;
    private UIData[] _UIDatas = {
        new UIData(UIID.CatStore, "Prefabs/CatStoreUI", null),
        new UIData(UIID.Storage, "Prefabs/StorageUI", null),
        new UIData(UIID.Task, "Prefabs/TaskUI", null),
        new UIData(UIID.CatHandbook, "Prefabs/CatHandbookUI", null),
    };
    private Stack<UIData> _UIStack = new Stack<UIData>();

	public override void OnInit ()
	{
        _UIRoot = GameObject.Find("UI");
        _UICanvas = GameObject.Find("UI/Canvas");

        if (_UIRoot == null || _UICanvas == null)
        {
            Debug.LogError("UIRoot not exist!!!");
        }

        if (_UIRoot)
        {
            DontDestroyOnLoad(_UIRoot);
        }
	}

	public override void OnTerminate ()
	{

	}

	public override void OnPause ()
	{

	}

    public void PushUI(UIID id)
    {
		foreach (UIData data in _UIDatas)
        {
            if (data.ID == id)
            {
                GameObject ui = Instantiate(Resources.Load(data.PrefabPath, typeof(GameObject))) as GameObject;
                ui.transform.SetParent(_UICanvas.transform, false);
                UIData uiData = new UIData(data.ID, data.PrefabPath, ui);
                _UIStack.Push(uiData);
                return;
            }
        }
    }

    public void Pop()
    {
        UIData uiData = _UIStack.Pop();
        GameObject.Destroy(uiData.UIObject);
    }

    public void PopTo(UIID uiid)
    {
        while (_UIStack.Count > 0)
        {
            UIData uiData = _UIStack.Pop();
            if (uiData.ID == uiid)
            {
                GameObject.Destroy(uiData.UIObject);
                break;
            }
        }
    }
}
