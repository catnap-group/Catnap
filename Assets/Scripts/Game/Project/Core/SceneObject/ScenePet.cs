using UnityEngine;
using System.Collections;

public class ScenePet : SceneUnit
{
	protected characterBase _BaseData;
	public override characterBase GetBaseData()
	{
		return _BaseData;
	}
	public override string GetPrefab ()
	{
		return GetBaseData ().prefab;
	}
	public float baseSpeed = 3;

	public AIStateManager GetStateManager() { return _AIStateManager; }

	protected AIStateManager _AIStateManager = null;
}

