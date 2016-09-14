using UnityEngine;
using System.Collections;
public enum GameState
{
	None,
	GamePlaying,
	GamePause,          //pause game
	CatEdit,
	CatPlay,
};
public class GameManager : UnityAllSceneSingleton<GameManager> {

	public bool _GameStarted = false;
	public bool resetGame = false;
	public bool showInfoEnable = false;
	private bool _GameLowFramed = false;
	private bool _OldGameLowFramed= false;

	public bool IsGameStarted()
	{
		return _GameStarted;//real game start
	}
	private GameState _GameState = GameState.None;
	public GameState GetGameState()
	{
		return _GameState;
	}
	public void StartGame()
	{
		//if game is not yet started, start it now
		_GameStarted = true;
	}
	public void EndGame()
	{
		_GameStarted = false;
	}
	public void SetGameState(GameState state)
	{
		_GameState = state;
	} 
	public void PauseGame()
	{
		SetGameState(GameState.GamePause);
		Time.timeScale = 0;
		Application.targetFrameRate = 5;
	}
	public override void OnInit ()
	{
		Application.runInBackground = true;
		Debug.logger.logEnabled = true;//打开log，发布正式版的时候关闭

		//基本数据访问接口
		BaseDataManager.Create();

		TouchListener.Create();

		//内存池创建
		MemoryDataManager.Create();

		//资源管理器创建
		ResourcesManager.Create();


		UIManager.Create();


	}
	public override void OnTerminate ()
	{
	}
	public override void OnPause ()
	{
	}
	#if SHOWINFO
	float updateInterval = 0.5f;
	private float lastInterval = 0.0f; // Last interval end time
	private int frames = 0; // Frames over current interval
	private float fps = 0.0f;     // Current FPS
	private float sum = 0.0f;
	private float num = 0.0f;
	public static int verts;
	public static int tris;
	ParticleSystem.Particle[] pars ;
	private static int parCnts = 0;
	#endif
	void GetParticles(GameObject obj)
	{

		#if SHOWINFO
		Component[] filters;
		filters = obj.GetComponentsInChildren<ParticleSystem>();

		foreach(ParticleSystem p in filters)
		{
		parCnts = p.particleCount;
		//p.particleCount
		}
		#endif
	}
	void GetObjectStats()
	{
		#if SHOWINFO
		verts = 0;
		tris = 0;
		parCnts = 0;

		GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		Camera _camera = Camera.main;
		if (_camera == null)
		return;

		foreach (GameObject go in ob)
		{
		if (go.GetComponent<Renderer>() != null && go.GetComponent<Renderer>().isVisible && GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(_camera),go.GetComponent<Renderer>().bounds))
		{
		GetObjectStats(go);
		}
		GetParticles(go);
		}
		#endif
	}
	void GetObjectStats(GameObject obj)
	{
		#if SHOWINFO
		Component[] filters;
		filters = obj.GetComponentsInChildren<MeshFilter>();

		foreach(MeshFilter f in filters)
		{
		if(f.GetComponent<Renderer>()!= null
		&& (f.GetComponent<Renderer>().isPartOfStaticBatch || f.sharedMesh == null)) continue;
		tris+= f.sharedMesh.triangles.Length / 3;
		verts+= f.sharedMesh.vertexCount;
		}
		#endif
	}
	#if !UNITY_IPHONE 
	//RunningTimeInfo _RunningTimeInfo = null;
	#endif 
	#if SHOWINFO
	void OnGUI()
	{
	if (fps < 30)
	GUI.color = Color.red;
	else
	GUI.color = Color.green;
	GUILayout.Label("fps:" + fps.ToString("f0") + "\n" + "average:" + (sum / num).ToString("f0"));
	GUI.color = Color.white;
	string vertsdisplay = verts.ToString("#,##0 verts");
	GUILayout.Label(vertsdisplay);
	string trisdisplay = tris.ToString("#,##0 tris");
	GUILayout.Label(trisdisplay);
	string parCntsdisplay = parCnts.ToString("#,##0 particles");
	GUILayout.Label(parCntsdisplay);
	#if UNITY_IPHONE     
	//	if (_RunningTimeInfo != null)
	//	{
	//				int memoryTotal = (int)_RunningTimeInfo.PluginGetFreeMemory();
	//				GUILayout.Label("free memory count:" + memoryTotal.ToString());
	//	}
	#endif
	}
	#endif
	public void OnApplicationPause(bool pauseStatus)
	{
		_GameLowFramed = pauseStatus;
	}
	void Update()
	{
		_GameLowFramed = _GameLowFramed || Application.runInBackground;
		if (_OldGameLowFramed != _GameLowFramed) {
			if (_GameLowFramed)
				EventListener.Broadcast (MsgType.ApplicationPaused);
			else
				EventListener.Broadcast (MsgType.ApplicationActived);
			_OldGameLowFramed = _GameLowFramed;
		}


	}
	//先屏蔽信息更新，太耗效率
	#if SHOWINFO
	public override void FixedUpdate()
	{
	++frames;
	float timeNow = Time.realtimeSinceStartup;
	if (timeNow > lastInterval + updateInterval)
	{
	fps = frames / (timeNow - lastInterval);
	frames = 0;
	lastInterval = timeNow;
	sum += fps;
	num++;
	GetObjectStats();
	}
	}
	#endif
	public override void Start()
	{

		#if SHOWINFO
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;


		#if UNITY_IPHONE        
		//_RunningTimeInfo = this.gameObject.AddComponent<RunningTimeInfo>();
		#endif
		#endif
	}
}
