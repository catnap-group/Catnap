using System.Collections.Generic;
using Newtonsoft.Json;
public class CatnapJsonProtoBase : UJsonProtoBase {
	
	public int Code;
	public string Msg;

	public override bool IsSucceeded()
	{
		return Code == 0;
	}

	public override string GetErrorMessage()
	{
		return Msg;
	}
}
public class Data
{
	public string uid;
	public string uname;
}
public class LBSJPTest : CatnapJsonProtoBase
{
	public Data data;
}
public class PlayerData
{
	public int uid{ get; set;}
	public string uname{ get; set;}
	public float longitude{ get; set;}
	public float latitude{ get; set;}
	public int distance{ get; set;}
}
public class NearByPlayer:  CatnapJsonProtoBase
{
	public List<PlayerData> data;
}