using UnityEngine;
using System.Collections;
using System.Net;

public class CatnapWebMgr : UWebMgr
{
	public override void Init ()
	{
		if (Initialized)
			return;
		m_TimeOutInterval = 30.0f;
		ProtoCommand.Register (this);
		base.Init ();
	}
	public void RegisterByWRI(EWebRequestId id, UWebRequest request)
	{
		Register((uint)id ,request);
	}
	public void RequestByWRI(EWebRequestId id, OnWebResponse onResponse, OnWebError onError)
	{
		Request ((uint)id, onResponse, onError);
	}
	protected override string GetUrlRoot ()
	{
		
		return "http://" + GetIpByHost(ConstantTable.serverIp) + ":" + ConstantTable.port + "/";// "http://192.168.119.208:8088/";
	}

	protected override string GetNetworkFailureMsg()
	{
		return "Network Failure";
	}
	private string GetIpByHost(string name)
	{
		System.Net.IPHostEntry Hosts = Dns.GetHostEntry (name);
		if (Hosts.AddressList.Length > 0)
			return Hosts.AddressList [0].ToString ();
		else
			return name;
	 
	}
	protected override void ErrorMessageReporter (uint id, object data)
	{
		if (data is CatnapJsonProtoBase) {
			var jp = data as CatnapJsonProtoBase;
			if (!jp.IsSucceeded ()) {
				//bulabula ....错误处理
				if (jp.Code == 2001) {
					Debug.LogError ("uid错误, 用户不存在.uid:" + GamePlayer.Me.instance.id);
				}
			}
		}
	}
	protected override bool IsUrlInclArgs ()
	{
		return true;
	}
	protected override bool IsUseSignature()
	{
		return false;//没有用到
	}
	protected override string GetSalt ()
	{
		return "123456";//暂时没有用到
	}
	void OnWebErrorMsgBoxClose(params object[] objs)
	{
	}
}
