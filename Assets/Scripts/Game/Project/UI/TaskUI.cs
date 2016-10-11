using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskUI : MonoBehaviour {

	public class TaskData
	{
		public int ID;
		public string Name = "";
		public string Title = "";
		public string Desc = "";
		public string IconPath = "";
		public UIID UIName;

		public TaskData(int id, string name, string desc, string iconPath, UIID uiName, string title)
		{
			ID = id;
			Name = name;
			Desc = desc;
			IconPath = iconPath;
			UIName = uiName;
			Title = title;
		}
	}

	private bool _IsHideMenu = false;
	private int _CurrentTaskID = 1;
	private TaskData _CurrentTaskData = null;
	private TaskData[] _TaskDatas = {
		new TaskData(1, "任务1", "一只从长白山莫崖泉来的青蛙抢走了猫咪的逗猫棒。找到它猫咪将给你丰厚的回报。", "UI/task1", UIID.Main, "猫咪任务 （1/2）"),
		new TaskData(2, "任务2", "猫咪将她得铃铛丢在了一家总理都去过的创业咖啡馆快去帮猫咪找回来吧", "UI/task2", UIID.Main, "猫咪任务（2/2）"),
	};

	void Start()
	{
		OpenTask (1);
		transform.FindChild("Top/MenuBtn/MenuUI").gameObject.SetActive(_IsHideMenu);
	}

	void OpenTask(int taskID)
	{
		if (taskID != _CurrentTaskID && 0 < taskID && taskID <= _TaskDatas.Length) {
			_CurrentTaskID = taskID;
			_CurrentTaskData = _TaskDatas[taskID - 1];

			Text desc = transform.FindChild ("Center/bg/desc").GetComponent<Text> ();
			desc.text = _CurrentTaskData.Desc;

			Image icon = transform.FindChild ("Down/Icon").GetComponent<Image> ();
			icon.sprite = UGUIUtility.LoadSprite (_CurrentTaskData.IconPath);

			Text title = transform.FindChild ("Top/Title").GetComponent<Text> ();
			title.text = _CurrentTaskData.Title;
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
		//UIManager.Instance.Open (_CurrentTaskData.UIName);
		if (_CurrentTaskID == 1) {
			CatSceneManager.Instance.SetNextScene (SceneID.Gift);
		} else if (_CurrentTaskID == 2) {
		
		}
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
