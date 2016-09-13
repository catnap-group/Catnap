
public class CatnapJsonProtoBase : UJsonProtoBase {

	public bool Success;
	public int Code;
	public string Msg;

	public override bool IsSucceeded()
	{
		return Success;
	}

	public override string GetErrorMessage()
	{
		return Msg;
	}
}

public class LBSJPTest : CatnapJsonProtoBase
{
	public string valStr;
	public int valInt;
	public double valDouble;
}