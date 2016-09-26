using UnityEngine;
using System.Collections;

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
class SceneCatAIStateManager : ScenePetBaseAIStateManager<SceneCat>
{
	public SceneCatAIStateManager(SceneCat creep)
		:base(creep)
	{

	}
	public override void LoadAIStates()
	{
		AddState(new CatIdleAIState());
	}

	public override void Check()
	{
		if (_CurrentState == null)
		{
			SetState<CatIdleAIState>();
		}
		else
		{
			_CurrentState.Check();
		}

		if (_CurrentState.GetRunningState() == AIRuningState.Running)
			return;

		if (_CurrentState.IsKind<CatIdleAIState>() )
		{
			//SetState<CatIdleAIState>();
		}//做其他事情
//		else if (_CurrentState.IsKind<MoveToTargetAIState>())
//		{
//			SetState<CatIdleAIState>();
//		}       
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

