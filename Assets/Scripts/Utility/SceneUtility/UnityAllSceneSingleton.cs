using UnityEngine;
using System.Collections;

public 	interface IManagerBase
{
	void Init();

	void OnInit();

	void OnTerminate();

	void OnPause();

	HideFlags GetHideFlag();

	bool DestroyOnLoad();
}

public class UnityAllSceneSingleton<T> : MonoBehaviour, IManagerBase
	where T: Component, IManagerBase
{
	public static bool Created = false;
	private bool _Initialized = false;
	public bool Initialized
	{
		get{return _Initialized;}
	}
	public static T CreateSelf<TB>()
		where TB: Component
	{
		_instance = FindObjectOfType<TB>() as T;
		if(_instance == null)
		{
			GameObject obj = new GameObject();
			obj.name = typeof(T).ToString();

			_instance = (T)obj.AddComponent(typeof(TB));
			if(_instance.DestroyOnLoad())
			{
				DontDestroyOnLoad(obj);
			}
			obj.hideFlags = _instance.GetHideFlag();
			_instance.Init();
			Created = true;
		}
		return _instance;
	}
	public static T Create()
	{
		_instance = FindObjectOfType<T>();
		if(_instance == null)
		{
			GameObject obj = new GameObject();
			obj.name = typeof(T).ToString();

			_instance = (T)obj.AddComponent(typeof(T));
			if(_instance.DestroyOnLoad())
			{
				DontDestroyOnLoad(obj);
			}
			obj.hideFlags = _instance.GetHideFlag();
			_instance.Init();
			Created = true;
		}
		return _instance;
	}
	public  TB CastFor<TB>()
	where TB:class
	{
		return  _instance as TB;
	}
	private static T _instance;
	public static T Instance
	{
		get
		{
            if (null == _instance)
            {
                Create();
            }
			return _instance;
		}
	}

	virtual public void Awake()
	{
		if(_instance == null)
		{
			_instance = this as T;
		}
	}
	virtual public void OnDestroy()
	{
	}
	virtual public void Start()
	{
		if(_instance == null)
		{
			Debug.Log(this.ToString() + "not Init, please Check");
		}
		_instance.Init();
	}
	virtual public void FixedUpdate()
	{
		
	}
	public virtual void Init()
	{
		if(!_Initialized)
		{
			Debug.Log(_instance.GetType().ToString() + "Init.");
			_instance.OnInit();
			_Initialized = true;
		}
	}

	public virtual HideFlags GetHideFlag(){return HideFlags.HideInHierarchy;}

	public virtual bool DestroyOnLoad(){return true;}

	public void Terminate()
	{
		if(Initialized && _instance != null)
		{
			_instance.OnTerminate();
		}
		_instance = null;
	}
	public void Pause()
	{
		if(Initialized && _instance != null)
		{
			_instance.OnPause();
		}
	}
	private void OnApplicationQuit()
	{
		Terminate();
	}
	private void OnApplicationPause()
	{
		Pause();
	}
	public virtual void OnPause()
	{
		
	}
	public virtual void OnInit()
	{
	}
	public virtual void OnTerminate()
	{
	}
}
