using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public enum SceneID
{
	NULL,
	//空
	Initialize,
	//下载、基础数据加载,初始化
	Loading,
	//加载
	Login,
	//登录界面
	Game,
	//游戏
	Gift,
	//礼物
	Edit,
	//编辑
};
public class CatSceneManager : UnityAllSceneSingleton<CatSceneManager> {

	public class SceneTransitionData
	{
		public SceneID CurrentSceneID;
		public string SceneName;
		public bool IsAdditive;
		public SceneID LoadingScene;
		public string UiPfb;

		public SceneTransitionData (SceneID sid, string name, bool additive, SceneID loading, string uiPfbName)
		{
			CurrentSceneID = sid;
			SceneName = name;
			IsAdditive = additive;
			LoadingScene = loading;
			UiPfb = uiPfbName;
		}
	}

	private static SceneTransitionData[] _SceneDatas = {
		new SceneTransitionData(SceneID.NULL, "", false, SceneID.NULL,null),        
		new SceneTransitionData(SceneID.Initialize, "Initialize", false, SceneID.NULL,null), 
		new SceneTransitionData(SceneID.Login, "LoginScene", false, SceneID.NULL,null),   
		new SceneTransitionData(SceneID.Game, "Game", false, SceneID.NULL,null),   
		new SceneTransitionData(SceneID.Gift, "Gift", false, SceneID.NULL,null),   
		new SceneTransitionData(SceneID.Edit, "EditScene", false, SceneID.NULL,null), 

	};

	//下个场景
	private SceneID _NextSceneID = SceneID.NULL;
	//已经Loaded的场景
	private SceneID _LoadedSceneID = SceneID.NULL;
	//当前场景
	private SceneID _CurrSceneID = SceneID.NULL;
	//当前场景
	public SceneID CurrSceneID
	{
		get { return _CurrSceneID; }
	}

	//异步对象
	private AsyncOperation _AsyncObject = null;

	//获取加载进度
	public AsyncOperation GetLoadingProgress()
	{
		return _AsyncObject;
	}

	private int _CurrentLoadingCount = 0;
	private int _TotalLoadingCount = 1;
	//动态场景加载的时候调用
	public void UpdateLoadingState(int currentCount, int totalCount)
	{
		//加载完成
		if (currentCount == totalCount)
		{
			//			Loading loading = (Loading)FindObjectOfType(typeof(Loading));
			//			if (loading)
			//			{
			//				loading.Over();
			//			}
		}
		_CurrentLoadingCount = currentCount;
		_TotalLoadingCount = totalCount;
	}
	//获取加载进度
	public int GetLoadingProcess()
	{
		if (_AsyncObject == null) {
			return 100;
		}
		int split = 50;
		int tempProcess = (int)(_AsyncObject.progress * split); 
		return tempProcess + _CurrentLoadingCount * (100 - split) / _TotalLoadingCount;
	}
	//场景是否加载好
	public bool LoadedReady
	{
		get { return _SceneLoadedReady; }
		set { _SceneLoadedReady = value; }
	}
	private bool _SceneLoadedReady = true;
	public void SetNextScene(SceneID sceneID)
	{
		if (sceneID != _LoadedSceneID)
		{
			EventListener.Broadcast(MsgType.SceneBeginLoad, sceneID);
			_NextSceneID = sceneID;
		}
	}
	void Update()
	{
		if (_NextSceneID != SceneID.NULL && _AsyncObject == null) {

			//加载场景
			AsyncLoadScene(_NextSceneID);
			Debug.Log ("START LOAD SCENE" + _NextSceneID.ToString ());
		}
		//异步加载场景OK
		if (_AsyncObject != null && _AsyncObject.isDone && LoadedReady)
		{
			SceneTransitionData sd = GetSceneData(_NextSceneID);

			if (sd.CurrentSceneID == _LoadedSceneID)
			{
				Debug.Log("Scene " + sd.SceneName + " AsyncLoaded.");
				_NextSceneID = SceneID.NULL;
				_LoadedSceneID = SceneID.NULL;
				_CurrSceneID = sd.CurrentSceneID;
				_AsyncObject = null;
			}
			else
			{
				Debug.Log("Scene Loading AsyncLoaded. Load Next Scene");
				AsyncLoadScene(_NextSceneID);
			}
		}
	}
	public SceneTransitionData GetSceneData(SceneID scenID)
	{
		foreach (SceneTransitionData data in _SceneDatas)
		{
			if (data.CurrentSceneID == scenID)
				return data;
		}

		return null;
	}

	private void AsyncLoadScene(SceneID id)
	{
		SceneTransitionData sd = GetSceneData(id);
		if (sd == null)
			return;

		//卸载当前场景内容
		SceneBase sceneBase = (SceneBase)FindObjectOfType(typeof(SceneBase));
		if (sceneBase != null)
		{
			sceneBase.Unload();
			Resources.UnloadUnusedAssets ();//去除不用的资源
		}

		//如果有LoadingScene则先加载LoadingScene
		if (sd.LoadingScene != SceneID.NULL && _LoadedSceneID != sd.LoadingScene)
		{

			//加载loadingScene
			AsyncLoadScene(sd.LoadingScene);
		}
		else
		{
			Debug.Log("Begin Load Scene " + sd.SceneName);
			//直接加载nextScene
			if (sd.IsAdditive)
				_AsyncObject = SceneManager.LoadSceneAsync(sd.SceneName, LoadSceneMode.Additive);
			else
				_AsyncObject = SceneManager.LoadSceneAsync(sd.SceneName);

			_LoadedSceneID = sd.CurrentSceneID;

		}
	}
}
