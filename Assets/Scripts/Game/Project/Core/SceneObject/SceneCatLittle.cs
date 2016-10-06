using UnityEngine;
using System.Collections;

public class SceneCatLittle : SceneUnit
{

	public override bool IsCat() { return false; }
	protected characterBase _BaseData;
	public override characterBase GetBaseData()
	{
		return _BaseData;
	}
	public override string GetPrefab ()
	{
		return GetBaseData ().prefab;
	}
	public override void Init(int baseID)
	{
		_BaseData = BaseDataManager.Instance.GetTableDataByID<characterBase> (baseID);

		base.Init (baseID);
		//刷新外形
		RefreshPresentation();
	}
}

