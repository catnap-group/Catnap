using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameScene : SceneBase
{
	[SerializeField]
	public Camera effCamera;
	static public GameScene Instance = null;
	private Transform thisT;
	public static float myHeight =0;
	public static float cameraHeight = 60;
	void Awake()
	{
		Instance = this;
		#if UNITY_EDITOR
		if(!CatSceneManager.Created)
		{
			CatSceneManager.Create ();
			GameManager.Create ();
		}
		#endif
		GameManager.Instance.SetGameState(GameState.CatPlay);
		UIManager.Instance.Open(UIID.CatStore);

		//        if (isCreateGalaxy)
		//            SceneBuildManager.Instance.CreateGalaxy();

		ResourcesManager.Instance.LoadSystemBaseData();
		thisT = transform;

		Time.timeScale = 1;
		Application.targetFrameRate = 45;
		StartCoroutine(LoadScene());

		//UIManager.Instance.ShowPage<WeaponUI> ();//UI
	}
	private byte[] _GameUserData = null;
	IEnumerator LoadScene()
	{
		if(!GameManager.Instance.resetGame)
		{
			//等待服务器数据到位
			yield return StartCoroutine(GetGameUserData());
		}
		else
		{
			//reset game!!
		}
		//加载基础场景
		//生成tango管理器
		ResourcesManager.Instance.LoadGameObject ("Prefabs/Tango/Tango Seivice");//临时代码，这部分以后要变成class create的模式，现在为了便于调试,这个上面起作用的类是tangoserviece
		GameObject cloudObj = ResourcesManager.Instance.LoadGameObject ("Prefabs/Tango/Tango Point Cloud");//临时代码，这部分以后要变成class create的模式，现在为了便于调试
		yield return new WaitForEndOfFrame();
		//生成tango镜头
		TangoARPoseController tarPoseCon =Camera.main.gameObject.AddComponent<TangoARPoseController>();
		tarPoseCon.m_useAreaDescriptionPose = true;
		tarPoseCon.m_syncToARScreen = true;
		TangoManager.Instance.m_pointCloud = cloudObj.GetComponent<TangoPointCloud> ();
		TangoService.Instance.m_poseController = tarPoseCon;
		//texture Method 
		TangoService.Instance.m_tangoApplication.m_videoOverlayUseTextureMethod = true;
		TangoService.Instance.m_tangoApplication.m_videoOverlayUseYUVTextureIdMethod = false;
		TangoService.Instance.m_tangoApplication.m_videoOverlayUseByteBufferMethod = false;

		//AreaDescriptions
		TangoService.Instance.m_tangoApplication.m_enableAreaDescriptions = true;
		//mode 2
		TangoService.Instance.m_tangoApplication.m_enableDriftCorrection = false;
		TangoService.Instance.m_tangoApplication.m_areaDescriptionLearningMode = false;

		//		yield return StartCoroutine (MapScene.Instance.LoadBaseSceneAsync ("demo_01"));
		//解析内容数据
		//		int index = MapScene.Instance.ParseGameContent(_GameUserData);
		//		MapScene.Instance.ParseGameContent1(_GameUserData, index);
		//
		//
		yield return null;
		//
		//		//加载游戏场景内容
		//		MapScene.Instance.LoadGameContent();
		//
		//		yield return null;
		//		MapScene.Instance.LoadGameContent2();
		//
		//
		//UIManager.Instance.ShowPage<UIGameScene>();
		//        UIManager.Instance.ShowPage<HUD>();
		//打开tango 探测器
		yield return StartCoroutine(StartTangoDetect());
		OnSceneLoaded();
		//
		//更新下载状态
		CatSceneManager.Instance.UpdateLoadingState(100, 100);
	}
	public void OnSceneLoaded()
	{
		

		 SceneCat cat = MapSceneManager.Instance.CreateSceneCat(101,Vector3.zero, Quaternion.identity);
		//投射到真实空间去
		TangoManager.Instance.SceneUnit2ARUnit(cat);
		//		TempAIManager.Create ();
		//TBIniMgr.Instance.SetPlayerData("test", "我的");
		//		MapScene.Instance.sta
		//		TempAIManager.Instance.StartBuild ();
	}
	IEnumerator StartTangoDetect()
	{
		while (!TangoService.Instance.StartGame ())
			yield return new WaitForSeconds (2);//2秒一次，直到返回列表
		while(!TangoManager.Instance.IsTangoReady())
			yield return new WaitForSeconds (1);//1秒一次，直到tango avaliable，这时候场景加载完毕
			
	}
	public override void Unload ()
	{
		//if (!isSelfOpen) {
		//    UIBase ui = UIManager.Instance.GetUIByType (UIManager.UIType.Game);
		//    if (ui != null)
		//        UIManager.Instance.DetachSceneUI (ui.GetUIObjID ());
		//}
		TangoService.Instance.GameOver();
		GameManager.Instance.EndGame();
		//UIManager.Instance.ClosePage<HUD>();//UI dispose
		//UIManager.Instance.ClosePage<UIGameScene>();
		base.Unload ();
	}
	IEnumerator GetGameUserData()
	{
		//模拟
		yield return new WaitForSeconds(0.2f);

//		_GameUserData = ResourcesManager.Instance.GetFileData(ConstantTable.TempUserGameContentFile);
//		if (_GameUserData == null)
//		{
//			//_GameUserData = ResourcesManager.Instance.GetFileData(ConstTable.DefaultUserContentFile);
//			_GameUserData = ResourcesManager.Instance.GetFileData(ConstantTable.DefaultUserContentFile);
//		}

		yield return null;
	}
	IEnumerator Start()
	{

		yield return new WaitForSeconds (2.5f);
	}
	void Update()
	{
			GameManager.Instance.StartGame ();

	}

}

