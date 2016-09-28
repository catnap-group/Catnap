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
	RelocationOverlay,      //开始重定位
	TouchEffect,			//点击特效
	Message,			//提示泡泡
};

public class UIManager : UnityAllSceneSingleton<UIManager> 
{
    public class UIData
    {
        public UIID ID = UIID.NULL;
        public string PrefabPath = "";
        public GameObject UIObject;
		public long count = 0;

        public UIData(UIID id, string prefabPath, GameObject uiObject)
        {
            ID = id;
            UIObject = uiObject;
            PrefabPath = prefabPath;
        }
    }
	private static long count = 0;
    private GameObject _UIRoot;
    private GameObject _UICanvas;
    private UIData[] _UIDatas = {
        new UIData(UIID.CatStore, "Prefabs/UI/CatStoreUI", null),
		new UIData(UIID.Storage, "Prefabs/UI/StorageUI", null),
		new UIData(UIID.Task, "Prefabs/UI/TaskUI", null),
		new UIData(UIID.CatHandbook, "Prefabs/UI/CatHandbookUI", null),
		new UIData(UIID.RelocationOverlay, "Prefabs/UI/RelocalizationOverlay", null),
		new UIData(UIID.TouchEffect, "Prefabs/UI/TouchEffect", null),
		new UIData(UIID.Message, "Prefabs/UI/Msg", null),
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
	public void UnloadAllUI()
	{
		//需要在切换场景的时候删掉所有的,不然内存就太大了
		while (_UIStack.Count > 0)
        {
			UIData uiData = _UIStack.Pop();
			GameObject.Destroy(uiData.UIObject);
        }
	}
	public override void OnTerminate ()
	{
	}

	public override void OnPause ()
	{

	}

//    public void PushUI(UIID id)
//    {
//		foreach (UIData data in _UIDatas)
//        {
//            if (data.ID == id)
//            {
//                GameObject ui = Instantiate(Resources.Load(data.PrefabPath, typeof(GameObject))) as GameObject;
//                ui.transform.SetParent(_UICanvas.transform, false);
//                UIData uiData = new UIData(data.ID, data.PrefabPath, ui);
//                _UIStack.Push(uiData);
//                return;
//            }
//        }
//    }
	public void ShowMessage(string msg)
	{
		UIData data = Open (UIID.Message);
		Text text = data.UIObject.GetComponent<Text> ();
		text.text = msg;
	}
	public UIData Open(UIID id)
	{
		UIData pData = null;
		count++;
		//单个页面的显示与否，和push无关
		foreach (UIData data in _UIDatas) {
			if (data.ID == id) {
				data.count = count;//重新刷新计数
				pData = data;
				break;
			} 
		}
		if (pData == null
			|| pData.UIObject == null) {
			GameObject ui = Instantiate (Resources.Load (pData.PrefabPath, typeof(GameObject))) as GameObject;
			ui.transform.SetParent (_UICanvas.transform, false);
			UIData uiData = new UIData (pData.ID, pData.PrefabPath, ui);
			uiData.count = count;
			_UIStack.Push (uiData);
			pData = uiData;
		}

		pData.UIObject.name = Enum.GetName (typeof(UIID), pData.ID) + "_"+count;//计数和名字一致
		pData.UIObject.SetActive (true);
		return pData;
	}
	public void SetRectAnchor(UIID id, Vector2 anchorPos)
	{
        foreach (UIData data in _UIStack)
        {
			if (data.ID == id) {
					data.UIObject.GetComponent<RectTransform> ().anchorMin = 
						data.UIObject.GetComponent<RectTransform> ().anchorMax = anchorPos;
				break;
			} 
		}
	}
	public void Close(UIID id)//这将关闭第一个遇到的界面，用于游戏中只存一份的界面
	{
		count--;//由于页面也有上限，所以此处，需要减少，以免只开启不关闭后超越上限
		//单个页面的显示与否，和push无关
        foreach (UIData data in _UIStack)
        {
			if (data.ID == id) {
				data.UIObject.SetActive (false);
				break;
			} 
		}
	}
	public void Close(GameObject obj)//从UI实体上关闭该界面，比如点击了关闭按钮
	{
		if (obj.name.IndexOf ("_") == -1)
			return;
		string [] nmFull = obj.name.Split ('_');
		Close ((UIID)Enum.Parse (typeof(UIID), nmFull [0]), long.Parse (nmFull [1]));
	}
	public void Close(UIID id, long cnt)//这将关闭指定的界面，用于游戏中存在多份的界面，关闭时从gameobject操作
	//,这个count从名字上截取出来
	{
		count--;//由于页面也有上限，所以此处，需要减少，以免只开启不关闭后超越上限
		//单个页面的显示与否，和push无关
        foreach (UIData data in _UIStack)
        {
			if (data.ID == id && data.count == cnt) {
				data.UIObject.SetActive (false);
				break;
			} 
		}
	}
	public void CloseTo(GameObject obj)//从UI实体上关闭该界面，比如点击了关闭按钮
	{
		if (obj.name.IndexOf ("_") == -1)
			return;
		string [] nmFull = obj.name.Split ('_');
		CloseTo ((UIID)Enum.Parse (typeof(UIID), nmFull [0]), long.Parse (nmFull [1]));
	}
	public void CloseTo(UIID id, long cnt)
	{
        foreach (UIData data in _UIStack)
        {
			count--;
			data.UIObject.SetActive (false);
			if (data.ID == id && data.count == cnt) {
				break;
			} 
		}
	}
//    public void Pop()
//    {
//        UIData uiData = _UIStack.Pop();
//        GameObject.Destroy(uiData.UIObject);
//    }
//
//    public void PopTo(UIID uiid)
//    {
//        while (_UIStack.Count > 0)
//        {
//			UIData uiData = _UIStack.Pop();
//			GameObject.Destroy(uiData.UIObject);
//            if (uiData.ID == uiid)
//            {
//                break;
//			}
//        }
//    }
}
