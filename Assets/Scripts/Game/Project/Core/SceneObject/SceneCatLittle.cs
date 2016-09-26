using UnityEngine;
using System.Collections;

public class SceneCatLittle : SceneUnit
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
}

