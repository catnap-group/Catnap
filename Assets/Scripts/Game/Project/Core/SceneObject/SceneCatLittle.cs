using UnityEngine;
using System.Collections;

public class SceneCatLittle : SceneUnit
{

	public override bool IsCat() { return false; }
	protected characterBase _BaseData;
	public SceneCatLittle target;
	private GameObject cureff;
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
	public void StopAnimation()
	{
		if(_BaseData.idle_list != null && _BaseData.idle_list.Length > 0)
			StopAnimation (_BaseData.idle_list [0]);
	}
	public void ShowAnimation()
	{
		if(_BaseData.idle_list != null && _BaseData.idle_list.Length > 0)
			PlayAnimation (_BaseData.idle_list [0]);
	}
	public void StopEffect()
	{
		if (cureff == null)
			return;
		cureff.gameObject.SetActive (false);
		//GameObject.Destroy (cureff);
	}
	public void PlayEffect()
	{
		if (_BaseData.effname == null)
			return;
		int count = _BaseData.effname.Length;
		if (count >0) {
			if (!string.IsNullOrEmpty (_BaseData.effname [0])) {
			
				if (cureff == null) {//暂时只有一个
					cureff = GameObject.Instantiate (ResourcesManager.Instance.LoadAsset<GameObject> ("Prefabs/Effects/shitroll"));
					cureff.transform.parent = thisT;
					cureff.transform.localPosition = Vector3.zero;
					cureff.transform.localRotation = Quaternion.identity;
				}
				//Transform eff = Helper.FindChild (_BaseData.effname [i], thisT);
				cureff.gameObject.SetActive (true);
			}
		}
	}
}

