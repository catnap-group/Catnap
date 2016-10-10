using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskUI : MonoBehaviour {

	public class TaskData
	{
		public int ID;
		public string Name = "";
		public string Desc = "";
		public string IconPath = "";
		public UIID UIName;

		public TaskData(int id, string name, string desc, string iconPath, UIID uiName)
		{
			ID = id;
			Name = name;
			Desc = desc;
			IconPath = iconPath;
			UIName = uiName;
		}
	}

	private bool _IsHideMenu = false;
	private int _CurrentTaskID = 1;
	private TaskData _CurrentTaskData = null;
	private TaskData[] _TaskDatas = {
		new TaskData(1, "任务1", "规范登记规范的会计拿放大镜看是否是", "UI/task1", UIID.Main),
		new TaskData(2, "任务2", "发动机你发的国家的反馈给你交电费", "UI/task2", UIID.Main),
		new TaskData(3, "任务3", "股份的每个快递发两个多麻烦了看过没考虑对方", "UI/task2", UIID.Main),
	};

	void Start()
	{
		OpenTask (1);
		transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
	}

	void OpenTask(int taskID)
	{
		if (taskID != _CurrentTaskID && 0 < taskID && taskID < _TaskDatas.Length) {
			_CurrentTaskID = taskID;
			_CurrentTaskData = _TaskDatas[taskID - 1];

			Text desc = transform.FindChild ("Center/bg/desc").GetComponent<Text> ();
			desc.text = _CurrentTaskData.Desc;

			Image icon = transform.FindChild ("Down/Icon").GetComponent<Image> ();
			icon.sprite = UGUIUtility.LoadSprite (_CurrentTaskData.IconPath);
		}
	}

	public void PressLeft()
	{
		OpenTask (_CurrentTaskID - 1);	
	}

	public void PressRight()
	{
		OpenTask (_CurrentTaskID + 1);
	}

	public void PressTask()
	{
		UIManager.Instance.Open (_CurrentTaskData.UIName);
	}

	public void PressBack()
	{
		UIManager.Instance.Close (gameObject);
	}

	public void PressMenu()
	{
		_IsHideMenu = _IsHideMenu ? false : true;
		transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
	}
}
