using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public abstract class UNetMgr:UnityAllSceneSingleton<UNetMgr>
{
    protected float m_TimeOutInterval = 30.0f;
//	protected bool	m_IsInited;
//
//    public virtual void Init()
//	{
//		m_IsInited = true;
//	}
}

public abstract class UWebMgr : UNetMgr
{
    public delegate void OnWebResponse(uint id, object obj, object localArg);
    public delegate void OnWebError(uint id, string msg);

    public delegate void OnWebTextResponse(uint id, string msg, object localArg);

    protected UWebRequestRoutine    m_Routine;
    protected Dictionary<uint, UWebRequest> m_RequestDict = new Dictionary<uint, UWebRequest>();

    protected UWebArg           m_CustomArgs = new UWebArg();

    protected string            m_SessionToken;

    protected UWebRequestRoutine routine
    {
        get { return m_Routine; }
    }

    public override void Init()
    {
		if (Initialized)
			return;
		
        if (m_Routine == null)
        {
            GameObject container = new GameObject();
            container.name = "WebRoutine";

            m_Routine = container.AddComponent<UWebRequestRoutine>();
            m_Routine.Init(GetUrlRoot(), GetNetworkFailureMsg(), IsUrlInclArgs(), IsUseSignature(), GetSalt());

            m_Routine.SetErrorMessageReporter(ErrorMessageReporter);
        }

		base.Init ();
    }

    public void SetSessionToken(string token)
    {
        m_SessionToken = token;

        m_Routine.SetSessionToken(m_SessionToken);
    }

    public void SetHooks(UWebMgr.OnWebTextResponse responseHook, UWebMgr.OnWebError errorHook)
    {
        if (m_Routine != null)
        {
            m_Routine.SetHooks(responseHook, errorHook);
        } 
    }

    public void SetPostCallbacks(Action onPostBegin, Action<object> onPostEnd)
    {
        if(m_Routine != null)
        {
            m_Routine.SetPostCallbacks(onPostBegin, onPostEnd);
        }
    }

    public void Register(uint id, UWebRequest request)
    {
        m_RequestDict.Add(id, request);
    }

    public void Request(uint id, OnWebResponse onResponse, OnWebError onError)
    {
        UWebRequest request = GetRequest(id);
        if (request != null)
        {
            m_CustomArgs.Merge(request.args);

            routine.Post(request, m_CustomArgs, onResponse, onError, m_TimeOutInterval);
        }

        m_CustomArgs.Clear();
    }

    public void InitCustomArgs()
    {
        m_CustomArgs.Clear();
    }

    public void SetLocalArg(object localArg)
    {
        m_CustomArgs.localArg = localArg;
    }

    public void SetCustomArg(string key, object value)
    {
        m_CustomArgs.Add(key, value);
    }

    protected UWebRequest GetRequest(uint id)
    {
        UWebRequest request = null;
        m_RequestDict.TryGetValue(id, out request);

        return request;
    }

    protected abstract string GetUrlRoot();
    protected abstract string GetNetworkFailureMsg();
    protected abstract bool IsUrlInclArgs();
    protected abstract void ErrorMessageReporter(uint id, object data);
    protected abstract bool IsUseSignature();
    protected abstract string GetSalt();
}
