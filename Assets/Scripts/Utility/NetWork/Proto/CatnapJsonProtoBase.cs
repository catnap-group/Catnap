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

public class LBSJPTest : CatnapJsonProtoBase
{
	public string[] data;

}