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
	public GameObject playerBody;
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
		//UIManager.Instance.Open(UIID.CatStore);

		//        if (isCreateGalaxy)
		//            SceneBuildManager.Instance.CreateGalaxy();

		ResourcesManager.Instance.LoadSystemBaseData();
		thisT = transform;

		Time.timeScale = 1;
		Application.targetFrameRate = 45;
		StartCoroutine(LoadScene());

		//UIManager.Instance.ShowPage<WeaponUI> ();//UI
	}
	//private byte[] _GameUserData = null;
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
		GamePlayer.Me.Create ();
		GamePlayer.Me.instance.SetPlayer (playerBody.transform);
		//加载基础场景
		//生成tango管理器
		ResourcesManager.Instance.LoadGameObject ("Prefabs/Tango/Tango Seivice");//临时代码，这部分以后要变成class create的模式，现在为了便于调试,这个上面起作用的类是tangoserviece
		GameObject cloudObj = ResourcesManager.Instance.LoadGameObject ("Prefabs/Tango/Tango Point Cloud");//临时代码，这部分以后要变成class create的模式，现在为了便于调试
		ResourcesManager.Instance.LoadGameObject ("Prefabs/WordDetection/WordService");//声音探测服务启动
		GameObject WordDecCon = new GameObject ();//声音探测控制器
		WordDecCon.name = "WordDetectionController";
		WordDecCon.AddComponent<WordDetectionController> ();
		yield return new WaitForEndOfFrame();
		//生成tango镜头
		TangoARPoseController tarPoseCon =Camera.main.gameObject.AddComponent<TangoARPoseController>();
		tarPoseCon.m_useAreaDescriptionPose = true;
		tarPoseCon.m_syncToARScreen = true;
		TangoManager.Instance.m_pointCloud = cloudObj.GetComponent<TangoPointCloud> ();
		TangoManager.Instance.m_pointCloud.m_useAreaDescriptionPose = true;
		TangoService.Instance.m_poseController = tarPoseCon;
		//texture Method 
		TangoService.Instance.m_tangoApplication.m_videoOverlayUseTextureMethod = true;
		TangoService.Instance.m_tangoApplication.m_videoOverlayUseYUVTextureIdMethod = false;
		TangoService.Instance.m_tangoApplication.m_videoOverlayUseByteBufferMethod = false;
		#if UNITY_EDITOR
		TangoService.Instance.m_tangoApplication.m_doSlowEmulation = true;
		//TangoService.Instance.m_tangoApplication.m_emulationEnvironment = 
		TangoService.Instance.m_tangoApplication.m_emulationVideoOverlaySimpleLighting = true;
		#endif
		//AreaDescriptions
		TangoService.Instance.m_tangoApplication.m_enableAreaDescriptions = true;
		//mode 2
		TangoService.Instance.m_tangoApplication.m_enableDriftCorrection = false;
		TangoService.Instance.m_tangoApplication.m_areaDescriptionLearningMode = false;
		TangoARScreen arScreen = Camera.main.gameObject.GetComponent<TangoARScreen> ();
		arScreen.m_occlusionShader =Shader.Find( "Tango/PointCloud (Occlusion)");
		ARCameraPostProcess postProcess = Camera.main.gameObject.AddComponent<ARCameraPostProcess> ();
		postProcess.m_postProcessMaterial = ResourcesManager.Instance.LoadAsset<Material> ("Common\\TangoGizmos\\Materials\\ar_post_process");
		postProcess.enabled = false;
		TangoEnvironmentalLighting tel = Camera.main.gameObject.AddComponent<TangoEnvironmentalLighting> ();
		tel.m_enableEnvironmentalLighting = true;

		//tango motion  gesture
		TangoDeltaPoseController tdp = Camera.main.transform.parent.gameObject.AddComponent<TangoDeltaPoseController>();
		tdp.m_useAreaDescriptionPose = true;
		tdp.m_characterMotion = true;
		tdp.m_enableClutchUI = true;
		TangoGestureCamera gesture = Camera.main.gameObject.AddComponent<TangoGestureCamera>();
		gesture.m_targetFollowingObject = Camera.main.transform.parent.gameObject;
		gesture.m_defaultCameraMode = TangoGestureCamera.CameraType.FIRST_PERSON;

		yield return null;
		//#if !UNITY_EDITOR
		//打开tango 探测器
		yield return StartCoroutine(StartTangoDetect());
		//#endif
		OnSceneLoaded();
		//
		//更新下载状态
		CatSceneManager.Instance.UpdateLoadingState(100, 100);
	}
	public void OnSceneLoaded()
	{
		SceneCatLittle catLitter = MapSceneManager.Instance.CreateSceneCatLittle(102,Vector3.zero, Quaternion.identity);
		SceneCatLittle spade = MapSceneManager.Instance.CreateSceneCatLittle(103,Vector3.zero, Quaternion.identity, false);
		spade.target = catLitter;
		GamePlayer.Me.instance.HoldTool (spade.thisT);
		SceneCat cat = MapSceneManager.Instance.CreateSceneCat(101,Vector3.zero, Quaternion.identity);
		cat.StartWorkRoutine ();
		#if !UNITY_EDITOR
		//投射到真实空间去
		TangoManager.Instance.SceneUnit2ARUnit(cat);
		#endif
		//请求登录
		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().InitCustomArgs();
		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().SetSessionToken();
//		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().SetCustomArg("uid", uid);
//		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().SetCustomArg("lat", latitude);
//		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().SetCustomArg("lon", longitude);
		CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().RequestByWRI(EWebRequestId.MSG_TEST, OnLoginResponse, OnWebError);
	}
	void OnLoginResponse(uint id, object obj, object localArg)
	{
		Debug.Log ("id" + id);
		LBSJPTest lbs = obj as LBSJPTest;
		Debug.Log ("data" + lbs.data.ToString());
	}
	void OnWebError(uint id, string msg)
	{
		Debug.Log(msg);
	}
	void OnGUI()
	{
		if (GUI.Button (new Rect (0, 0, 50, 50), "gift")) {
			CatSceneManager.Instance.SetNextScene (SceneID.Gift);
		}
	}
//	void OnGUI()
//	{
//		GUI.Button(new Rect(0,0,100,100), "noNewArea")
//		{
//			
//		}
//
//	}
	IEnumerator StartTangoDetect()
	{
		TangoService.Instance.StartGame ();
		#if !UNITY_EDITOR
		while(!TangoService.Instance.HasAreaDescrip())
			yield return new WaitForSeconds (2);//2秒一次，直到返回列表
		while(!TangoManager.Instance.IsTangoReady())
			yield return new WaitForSeconds (1);//1秒一次，直到tango avaliable，这时候场景加载完毕
		#else
		yield return null;
		#endif
			
	}
	public override void Unload ()
	{

		TangoService.Instance.OnDispose();
		WordService.Instance.OnDispose ();
		WordDetectionController.Instance.OnDispose ();
		MapSceneManager.Instance.CleanAll ();
		GameManager.Instance.EndGame();
		UIManager.Instance.UnloadAllUI ();
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

