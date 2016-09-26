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
	//开始工作线程
	public virtual void StartWorkRoutine()
	{
		StartCoroutine("WorkRoutine");
	}
	//结束工作线程
	public virtual void StopWorkRoutine()
	{
		StopCoroutine("WorkRoutine");
	}
	public virtual IEnumerator WorkRoutine()
	{
		yield return null;

		float currentUpdateTime = 0.5f;
		WaitForSeconds waitForSeconds = null;
		while (!dead && _AIStateManager != null)
		{
			_AIStateManager.Check();

			float nextUpdateDelay = _AIStateManager.GetNextUpdateDelay();
			if (waitForSeconds == null || Mathf.Abs(nextUpdateDelay - currentUpdateTime) < 0.001f)
			{
				waitForSeconds = new WaitForSeconds(nextUpdateDelay);
				currentUpdateTime = nextUpdateDelay;
			}

			yield return waitForSeconds;
		}

		yield return null;
	}
	public override void Dead ()
	{
		base.Dead ();
		//停止工作线程
		StopCoroutine("WorkRoutine");
	}
}

