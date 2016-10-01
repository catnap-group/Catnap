using UnityEngine;
using System.Collections;
using System;

public class UnitySingleton<T> : UnityAllSceneSingleton<T>
	where T : Component, IManagerBase
{
	public override HideFlags GetHideFlag() { return HideFlags.None; }

	public override bool DestroyOnLoad() { return false; }

	public override void OnDestroy ()
	{
		OnTerminate();
		base.OnDestroy();
	}
}

public class UnitySingletonVisible<T> : MonoBehaviour
	where T : Component
{
	public static T Instance;
	public  void Awake()
	{
		Instance = this as T;
		OnInit ();
	}


	public virtual void OnInit (){}
	public virtual void OnDispose(){}
}
public interface IInitClass
{
	void Ctor();
}
public class SingletonVisible<T>:IDisposable,IInitClass
	where T: class , IInitClass,IDisposable,new()
	
{
	protected static T  i_instance;
	public static T GetInstance()
	{
		if (i_instance!=null) {
			return null;
		}
		i_instance = new T ();
		i_instance.Ctor ();
		return i_instance;
	}
    public virtual void Ctor()
	{
		
	}
	public virtual void Dispose()
	{
		i_instance = null;
	}
    
}

