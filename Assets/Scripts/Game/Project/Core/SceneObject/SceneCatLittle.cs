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
	public void ShowAnimation()
	{
		if(_BaseData.idle_list.Length > 0)
			PlayAnimation (_BaseData.idle_list [0]);
	}
	public void PlayEffect()
	{
		int count = _BaseData.effname.Length;
		for (int i = 0; i < count; i++) {
			if (!string.IsNullOrEmpty (_BaseData.effname [i])) {
			
				Transform eff = Helper.FindChild (_BaseData.effname [i], thisT);
				eff.gameObject.SetActive (true);
			}
		}
	}
}

