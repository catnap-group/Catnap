using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIParam
{
	
};
public enum AIRuningState
{
	Running,        //状态正在执行
	DefaultOver,    //一般Over
	Over1,          //特殊Over1
	Over2,         //特殊Over2
}
public class AIState
{
	protected bool _Executed = false;

	protected AIRuningState _RuningState = AIRuningState.DefaultOver;

	public AIRuningState GetRunningState() { return _RuningState;  }

	public virtual void SetUserData(AIParam data) { }
	public virtual AIParam GetUserData() { return null; }

	public void Execute()
	{
		if (_Executed)
			return;

		_Executed = true;

		_RuningState = AIRuningState.Running;

		OnExecute( );
	}

	protected virtual void OnExecute()
	{

	}

	protected virtual void OnCheck()
	{

	}

	public void Check()
	{
		OnCheck();

		if (GetRunningState() == AIRuningState.DefaultOver)
		{
			if (NextState != null)
			{
				Manager.SetState(NextState.ToString(), NextState.GetUserData());
			}
		}

	}
	public virtual void Update() { }
	public virtual void FixedUpdate() { }

	public void Leave()
	{
		_Executed = false;

		OnLeave();

		_RuningState = AIRuningState.DefaultOver;
	}

	protected virtual void OnLeave()
	{

	}

	public virtual void OnMsg(SceneUnit pSender, MsgType eMsgType, System.Collections.Hashtable arParam)
	{

	}

	public bool IsKind<T>()
		where T : AIState
	{
		return this.GetType().ToString() == typeof(T).ToString();
	}

	public AIState NextState
	{
		get;
		set;
	}

	public AIStateManager Manager
	{
		get;
		set;
	}
};
public class AIStateManager
{
	protected Dictionary<string, AIState> _StateList = new Dictionary<string, AIState>();

	protected AIState _CurrentState = null;
	public void AddState(AIState state)
	{
		state.Manager = this;
		if (_StateList.ContainsKey(state.ToString()))
		{
			return;
		}
		_StateList.Add(state.ToString(), state);
	}

	public virtual void LoadAIStates() { }

	public T SetState<T>(AIParam userData = null)
		where T : AIState
	{
		return SetState(typeof(T).ToString(), userData) as T;
	}

	public T2 SetState<T1,T2>(AIParam userData1 = null,AIParam userData2 = null)
		where T1:AIState
		where T2:AIState
	{
		AIState destState = GetState<T1>();

		if (destState == null)
			return null;

		if ( _CurrentState != null )
			_CurrentState.Leave();

		_CurrentState = destState;
		_CurrentState.SetUserData(userData1);
		_CurrentState.Execute();

		T2 nextState = GetState<T2>();
		_CurrentState.NextState = nextState;

		return nextState;

	}


	public bool IsState<T>()
		where T:AIState
	{
		return _CurrentState == GetState<T>();
	}

	public virtual void Update()
	{
		if (_CurrentState != null)
			_CurrentState.Update();
	}

	public virtual void FixedUpdate()
	{
		if ( _CurrentState != null )
			_CurrentState.FixedUpdate();
	}

	//
	public virtual void Check() 
	{
	}

	public T GetState<T>()
		where T : AIState
	{
		string state = typeof(T).ToString();
		if (!_StateList.ContainsKey(state))
			return null;
		return _StateList[state] as T;
	}

	public AIState SetState(string state, AIParam userData = null)
	{
		if (!_StateList.ContainsKey(state))
			return null;

		if (_CurrentState != null)
			_CurrentState.Leave();

		_CurrentState = _StateList[state];
		_CurrentState.SetUserData(userData);
		_CurrentState.NextState = null;
		_CurrentState.Execute();

		return _CurrentState;
	}

	protected float _NextUpdateDelay = 0.5f;

	public void SetNextUpdateDelay(float delay)
	{
		_NextUpdateDelay = delay;
	}

	public virtual float GetNextUpdateDelay()
	{
		float delay = _NextUpdateDelay;

		_NextUpdateDelay = 0.2f;

		return delay;
	}

	public virtual SceneUnit GetBaseParent() { return null; }
	/*
     * 说明: 消息传递给状态机处理
     * 参数:pSender       发送者
     *          eMsgType    消息ID
     *          arParam       参数列表
     */
	public virtual void OnMsg(SceneUnit pSender, MsgType eMsgType, Hashtable arParam = null)
	{

	}
};
public class PetAIState : AIState
{
	public PetAIState()
	{

	}

	public T TParent<T>()
		where T :ScenePet
	{

		ScenePetBaseAIStateManager<T> mgr = Manager as ScenePetBaseAIStateManager<T>;
		return mgr.GetParent();
	}

	public ScenePet Parent
	{
		get
		{
			return Manager.GetBaseParent() as ScenePet;
		}
	}
}

public class ScenePetBaseAIStateManager<T> : AIStateManager
	where T : ScenePet
{

	protected T _Parent;

	public ScenePetBaseAIStateManager(T pet)
	{
		_Parent = pet;

		LoadAIStates();
	}

	public T GetParent() { return _Parent; }


	public override SceneUnit GetBaseParent() { return _Parent; }
}



