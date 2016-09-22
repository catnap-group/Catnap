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
        //UIManager.Instance.PushUI(UIID.CatStore);

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
		//		//加载基础场景
		//yield return StartCoroutine (MapScene.Instance.LoadBaseSceneAsync ("demo_01"));
		//加载怪物
		//yield return StartCoroutine (MapScene.Instance.LoadBaseMosterAsync ("free_lv1"));

		//		yield return StartCoroutine (MapScene.Instance.LoadBaseSceneAsync ("demo_01"));
		//解析内容数据
		//		int index = MapScene.Instance.ParseGameContent(_GameUserData);
		//		MapScene.Instance.ParseGameContent1(_GameUserData, index);
		//
		//
		MapSceneManager.Instance.CreateSceneCat(101,Vector3.zero, Quaternion.identity);
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
		OnSceneLoaded();
		//
		//更新下载状态
		CatSceneManager.Instance.UpdateLoadingState(100, 100);
	}
	public void OnSceneLoaded()
	{

		//		TempAIManager.Create ();
		//TBIniMgr.Instance.SetPlayerData("test", "我的");
		//		MapScene.Instance.sta
		//		TempAIManager.Instance.StartBuild ();
	}

	public override void Unload ()
	{
		//if (!isSelfOpen) {
		//    UIBase ui = UIManager.Instance.GetUIByType (UIManager.UIType.Game);
		//    if (ui != null)
		//        UIManager.Instance.DetachSceneUI (ui.GetUIObjID ());
		//}
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

