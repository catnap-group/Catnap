using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CatIdleAIState : PetAIState
{

	protected float firstTime = 0.0f;
	protected float during = 0.0f;
	protected override void OnExecute()
	{
		during= Parent.PlayAnimation( Parent.GetRandomIdle() );
		Parent.PlaySound ((int)SceneCat.SOUND_ID.SI_YELL);

		firstTime = Time.fixedTime;
	}
	protected override void OnLeave()
	{

	}
	protected override void OnCheck()
	{
		if (Time.fixedTime <= (firstTime + during))
			return;

		_RuningState = AIRuningState.DefaultOver;
	}

}

public class CatRunMaiMenState : PetAIState
{
	AIObjectParam Param;
	public override void SetUserData(AIParam data) { Param = data as AIObjectParam; }
	public override AIParam GetUserData() { return Param; }
	
	protected override void OnExecute()
	{
		Parent.PlayAnimation ("cat-run");

		Vector3 vec1 = Parent.transform.position;
		Vector3 vec2 = Param.gameObject.transform.position;
		Vector3 off = new Vector3 (0.3f, 0, 0);
		if (vec1.x > vec2.x) {
			off.x = vec2.x + off.x;
		} else {
			off.x = vec2.x - off.x;
		}
		off.y = off.x / (vec2.x - vec1.x) * (vec2.y - vec1.y) + vec1.y;
		off.z = off.x / (vec2.x - vec1.x) * (vec2.z - vec1.z) + vec1.z;


		Parent.transform.DOLookAt (Param.gameObject.transform.position, 0);
		Parent.transform.DOMove (Param.gameObject.transform.position, 2).OnComplete (delegate() {
			_RuningState = AIRuningState.DefaultOver;
			Manager.Check();
		});
	}

	protected override void OnCheck()
	{
		
	}
}

public class CatRunEatState : PetAIState
{
	AIObjectParam Param;
	public override void SetUserData(AIParam data) { Param = data as AIObjectParam; }
	public override AIParam GetUserData() { return Param; }

	protected override void OnExecute()
	{
		Parent.PlayAnimation ("cat-run");

		Vector3 vec1 = Parent.transform.position;
		Vector3 vec2 = Param.gameObject.transform.position;
		Vector3 off = new Vector3 (0.2f, 0, 0);
		if (vec1.x > vec2.x) {
			off.x = vec2.x + off.x;
		} else {
			off.x = vec2.x - off.x;
		}
		off.y = off.x / (vec2.x - vec1.x) * (vec2.y - vec1.y) + vec1.y;
		off.z = off.x / (vec2.x - vec1.x) * (vec2.z - vec1.z) + vec1.z;

		Parent.transform.DOLookAt (Param.gameObject.transform.position, 0);
		Parent.transform.DOMove (Param.gameObject.transform.position, 2).OnComplete (delegate() {
			_RuningState = AIRuningState.DefaultOver;
			Manager.Check();
		});
	}

	protected override void OnCheck()
	{

	}
}

public class CatStandState : PetAIState
{
	protected override void OnExecute()
	{
		Parent.PlayAnimation ("cat-stand");
	}

	protected override void OnCheck()
	{

	}
}

public class CatEatState : PetAIState
{
	protected override void OnExecute()
	{
		Parent.PlayAnimation ("cat-eat");
	}

	protected override void OnCheck()
	{

	}
}

public class CatMaiMenState : PetAIState
{
	protected override void OnExecute()
	{
		Parent.PlayAnimation ("cat-maimen");
	}

	protected override void OnCheck()
	{

	}
}

public class AIObjectParam : AIParam
{
	public GameObject gameObject;
	public AIObjectParam(GameObject game)
	{
		gameObject = game;
	}
}

enum ObjectEvent
{
	CallCat,
	CallEat,
	SendSound,
}

class SceneCatAIStateManager : ScenePetBaseAIStateManager<SceneCat>
{
	private bool _CallCat = false;
	private bool _CallEat = false;
	
	public SceneCatAIStateManager(SceneCat creep)
		:base(creep)
	{
		EventListener.AddListener (ObjectEvent.CallCat, delegate(GameObject gameObject) {
			AIObjectParam param = new AIObjectParam(gameObject);
			SetState<CatRunMaiMenState>(param);
			_CallCat = true;
		});

		EventListener.AddListener (ObjectEvent.CallEat, delegate(GameObject gameObject) {
			AIObjectParam param = new AIObjectParam(gameObject);
			SetState<CatRunEatState>(param);
			_CallEat = true;
		});
		
	}
	public override void LoadAIStates()
	{
		//AddState(new CatIdleAIState());
		AddState (new CatRunMaiMenState ());
		AddState (new CatRunEatState ());
		AddState (new CatStandState ());
		AddState (new CatMaiMenState ());
		AddState (new CatEatState ());
	}

	public override void Check()
	{
		if (_CurrentState == null)
		{
			SetState<CatStandState>();
		}
		else
		{
			_CurrentState.Check();
		}
			
		if (_CurrentState.GetRunningState() == AIRuningState.Running)
			return;

		if (_CurrentState.IsKind<CatRunMaiMenState> ()) {
			SetState<CatMaiMenState> ();
		} else if (_CurrentState.IsKind<CatRunEatState> ()) {
			SetState<CatEatState> ();
		}    
	}
}

public class SceneCat : ScenePet
{
	public enum SOUND_ID
	{
		SI_YELL = 101,//猫叫
	}

	public override bool IsCat() { return true; }
	public SceneCat()
	{

	}
	public override void Init(int baseID)
	{
		_BaseData = BaseDataManager.Instance.GetTableDataByID<characterBase>(baseID);

		base.Init(baseID);

		baseSpeed = 1.0f;//GetMoveSpeed();//base speed as move speed ,cause we will change it later
		//获取基本数据

		//刷新外形
		RefreshPresentation();

		_AIStateManager = CreateAIStateManager();


		//SetMoveSpeed(GetData().move_speed);
	}
	protected override AIStateManager CreateAIStateManager()
	{
		return new SceneCatAIStateManager(this);   
	}
	public override void FixedUpdate()
	{
		if (_AIStateManager != null && (!dead ))
			_AIStateManager.FixedUpdate();
	}
	public override void Update()
	{
		base.Update();

		if (_AIStateManager != null)
			_AIStateManager.Update();
		
	}
}

