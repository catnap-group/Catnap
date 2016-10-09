using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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