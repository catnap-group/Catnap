using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneUnit : MonoBehaviour {
	/// <summary>
	///这些值先写死，以后从表格里读
	/// </summary>
	public int id;
	public float intimacy;//亲密度
	public float hungry;//饱食度
	public float m_timestamp;
	public Transform thisT;
	public GameObject thisObj;
	public UnitClassType m_Type;
	public int baseId;
	public bool dead = false;
	/// <summary>
	/// 这是游戏对象每帧对于设备的transform
	/// </summary>
	public Matrix4x4 m_deviceT = new Matrix4x4();

	public virtual bool IsCat() { return false; }
	public virtual void Init(int baseID)
	{
		BaseInit(baseID);
	}

	public virtual characterBase GetBaseData() { return null; }
	protected void BaseInit(int baseID)
	{
		baseId = baseID;
		thisT = transform;
		thisObj = gameObject;
		LoadAnimationData();
		LoadSoundData ();
		dead = false;
	}
	protected virtual AIStateManager CreateAIStateManager()
	{
		return null;
	}
	protected Animation _animation;
	protected Dictionary<string, animationBase> _AnimationData;
	protected Dictionary<int, soundBase> _SoundData;
	public void LoadAnimationData()
	{
		_AnimationData = BaseDataManager.Instance.GetAnimationBase(baseId);
	}
	public void LoadSoundData()
	{
		_SoundData = BaseDataManager.Instance.GetSoundBase (baseId);
	}
	public animationBase GetAnimationBaseData(string ani)
	{
		if (_AnimationData == null)
			return null;

		if (!_AnimationData.ContainsKey(ani))
			return null;
		return _AnimationData[ani];
	}
	struct AniInterval
	{
		public string name;
		public float itv;
	}

	public virtual void PlayAnimationSeq(params object[] list)//1.name 2.leap
	{
		List<AniInterval> anis = new List<AniInterval>();
		for (int i = 0; i < list.Length; i += 2)
		{
			AniInterval ainv = new AniInterval();
			string aname = (string)list[i];
			float leapTime = float.Parse(list[i + 1].ToString());
			ainv.name = aname;
			ainv.itv = leapTime;
			anis.Add(ainv);
		}
		StartCoroutine(AutoPlayAnimation(anis));
	}
	IEnumerator AutoPlayAnimation(List<AniInterval> anis)
	{
		for (int i = 0; i < anis.Count; i++)
		{
			if (_animation == null)
			{
				_animation = GetComponentInChildren<Animation>();
			}
			if (_animation.GetClip(anis[i].name) == null) continue;
			float during = PlayAnimation(anis[i].name);
			if (_animation[anis[i].name].wrapMode == WrapMode.Loop) break;
			yield return new WaitForSeconds(during + anis[i].itv);
		}
		yield return null;
	}
	public string[] GetIdleAnimationList()
	{
		return GetBaseData ().idle_list;//idle aniamtions，从表格里读，先写死
	}


	public string GetRandomIdle()
	{
		string[] idles = GetIdleAnimationList();
		if (idles == null)
		{
			return "";
		}
		return idles[Random.Range(0, idles.Length)];
	}
	public virtual void PlaySound(int soundId)
	{
		if (!_SoundData.ContainsKey (soundId))
			return;
		SoundManager.PlaySFX (_SoundData [soundId].file_name,_SoundData [soundId].is_loop);

	}
	public virtual void StopAnimation(string ani)
	{
		if (_animation == null)
		{
			_animation = GetComponentInChildren<Animation>();
		}
		if (_animation == null)
			return;

		_animation.Stop ();
	}
	public virtual float PlayAnimation(string ani)
	{
		if (_animation == null)
		{
			_animation = GetComponentInChildren<Animation>();
		}
		if (_animation == null)
			return 0.0f;
		
		AnimationClip clip = _animation.GetClip(ani);
		if (clip == null)
		{
			//Debug.Log("Can find any clip " + ani);

			return 0;
		}

		animationBase baseData = GetAnimationBaseData(ani);
		if (baseData != null)
		{
			_animation[ani].speed = baseData.speed_rate;
		}
		_animation.Play ();
		_animation.CrossFade(ani);

		return _animation.clip.length;
	}

	public virtual float PlayAnimationQueued(string strAnimation)
	{
		if (_animation == null)
		{
			_animation = GetComponentInChildren<Animation>();
		}
		if (_animation == null)
			return 0.0f;

		AnimationClip clip = _animation.GetClip(strAnimation);
		if (clip == null)
		{
			Debug.Log("Can find any clip " + strAnimation);

			return 0;
		}

		animationBase baseData = GetAnimationBaseData(strAnimation);
		if (baseData != null)
		{
			_animation[strAnimation].speed = baseData.speed_rate;
		}

		_animation.CrossFadeQueued(strAnimation);

		return _animation.clip.length;
	}

	protected GameObject _PresentationObject = null;

	public GameObject GetPresentObject() { return _PresentationObject; }
	public virtual string GetPrefab() { return GetBaseData().prefab;}//角色的prefab//GetBaseData().prefab; }
	public void RefreshPresentation()
	{
		OnPresentObjectReady(MemoryDataManager.Instance.LoadGameObject(GetPrefab()));
	}
	public virtual void OnPresentObjectReady(GameObject obj)
	{
		_PresentationObject = obj;

		if (_PresentationObject == null) {
			//int iiii = 0;
		}
		_PresentationObject.transform.parent = thisT;

		_PresentationObject.transform.localPosition = Vector3.zero;
	}
	public virtual void OnUnInit(bool immediatly = true)
	{
		//RemoveAllAttachEffect();删除特效

		MemoryDataManager.Instance.UnloadGameObject(GetPresentObject());

		if (immediatly)
			DestroyImmediate(gameObject);
		else
			MonoBehaviour.Destroy(gameObject);
	}
	public virtual void Update()
	{
	}
	public virtual void FixedUpdate()
	{
	}
	public virtual void Dead()
	{
		dead = true;


	}

}
