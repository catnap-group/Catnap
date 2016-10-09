using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class TangoService : UnitySingletonVisible<TangoService>, ITangoLifecycle {
	
	public TangoARPoseController m_poseController;
	public bool m_enableLearningToggle;//是否开启学习模式,例子里面是个checkbox
	public bool m_needNewAreaDescription;//是否需要生成新区域，例子里面是两个按钮，这里只要设置一个开关
	public TangoApplication m_tangoApplication;
	/// <summary>
	/// The UUID of the selected Area Description.
	/// </summary>
	private string m_curAreaDescriptionUUID;
	private List<AreaDescription> vAreaDescription;

	public override void OnInit ()
	{
		m_tangoApplication = GetComponent<TangoApplication> ();
		vAreaDescription = new List<AreaDescription> ();
		m_needNewAreaDescription = true;
		if (m_tangoApplication != null)
		{
			m_tangoApplication.Register(this);
			if (AndroidHelper.IsTangoCorePresent())
			{
				m_tangoApplication.RequestPermissions();
			}
		}
		else
		{
			Debug.Log("No Tango Manager found in scene.");
		}
	} 
	public override void OnDispose ()
	{
		m_tangoApplication.Shutdown ();
	}

	public bool HasAreaDescrip()
	{
		return vAreaDescription.Count != 0;
	}
	public bool StartGame()//需要不停探测，直到打开一个区域，或者由界面打开
	{
		if (!m_needNewAreaDescription)
		{
			if (string.IsNullOrEmpty(m_curAreaDescriptionUUID))
			{
				AndroidHelper.ShowAndroidToastMessage("没有区域描述.");
				return false;
			}
		}
		else
		{
			m_curAreaDescriptionUUID = null;
		}

		// Dismiss Area Description list, footer and header UI panel.
		//gameObject.SetActive(false);

		if (m_needNewAreaDescription)
		{
			// Completely new area description.
			//m_guiController.m_curAreaDescription = null;
			TangoManager.Instance.m_curAreaDescription = null;
			m_tangoApplication.m_areaDescriptionLearningMode = true;
		}
		else
		{
			// Load up an existing Area Description.
			AreaDescription areaDescription = AreaDescription.ForUUID(m_curAreaDescriptionUUID);
			TangoManager.Instance.m_curAreaDescription = areaDescription;
			m_tangoApplication.m_areaDescriptionLearningMode = m_enableLearningToggle;
		}

		m_tangoApplication.Startup(TangoManager.Instance.m_curAreaDescription);

		// Enable GUI controller to allow user tap and interactive with the environment.
		m_poseController.gameObject.SetActive(true);
		return true;
		//m_guiController.enabled = true;
		//m_gameControlPanel.SetActive(true);
	}
	/// <summary>
	/// Internal callback when a permissions event happens.
	/// </summary>
	/// <param name="permissionsGranted">If set to <c>true</c> permissions granted.</param>
	public void OnTangoPermissions(bool permissionsGranted)
	{
		if (permissionsGranted)
		{
			_PopulateList();
			GameScene.Instance.PermissionTango = true;
			//返回列表
		}
		else
		{
			AndroidHelper.ShowAndroidToastMessage("需要运动跟踪和区域学习权限.");
			//Application.Quit();//没有必要关闭应用，由玩家杀掉进程就好
		}
	}
	void OnGUI()
	{

		GUI.skin.button.fontSize = 28;  
		for(int i = 0 ; i <vAreaDescription.Count ; i ++){
			GUI.Label (new Rect (0, 60 + i * 10, 100, 100), vAreaDescription [i].GetMetadata ().m_name);
		}
	}
	/// <summary>
	/// This is called when successfully connected to the Tango service.
	/// </summary>
	public void OnTangoServiceConnected()
	{
	}

	/// <summary>
	/// This is called when disconnected from the Tango service.
	/// </summary>
	public void OnTangoServiceDisconnected()
	{
	}
	private void _PopulateList()
	{
		vAreaDescription.Clear ();
		// Update Tango space Area Description list.
		AreaDescription[] areaDescriptionList = AreaDescription.GetList();

		if (areaDescriptionList == null)
		{
			return;
		}
		for (int i = 0; i < areaDescriptionList.Length; i++) {
			vAreaDescription.Add (areaDescriptionList [i]);//保存所有的描述
		}
//		if (vAreaDescription.Count > 0){
//			m_curAreaDescription = vAreaDescription [0];//默认选第一个
//			m_curAreaDescriptionUUID = vAreaDescription[0].m_uuid;
//		}
	}
}
