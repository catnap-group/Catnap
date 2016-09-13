using UnityEngine;
using System.Collections;

public class UnityAllSceneSingletonVisible<T> : UnityAllSceneSingleton<T>
	where T : Component, IManagerBase
{    //初始化
	public override void OnInit()
	{

	}


	public override HideFlags GetHideFlag() { return HideFlags.None; }

	public override bool DestroyOnLoad() { return true; }
}


