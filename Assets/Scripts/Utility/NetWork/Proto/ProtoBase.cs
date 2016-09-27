using UnityEngine;
using System.Collections;

public abstract class UProtoBase
{
	public abstract bool IsSucceeded();
	public abstract string GetErrorTitle();
	public abstract string GetErrorMessage();
}

public class UJsonProtoErrorBase
{
	public string   title;
	public string   message;
}

public class UJsonProtoBase : UProtoBase
{
	//public UJsonProtoErrorBase error;

	public override bool IsSucceeded()
	{
		throw new System.NotImplementedException();
	}

	public override string GetErrorTitle()
	{
//		if (error != null && error.title != null)
//			return error.title;

		return string.Empty;
	}

	public override string GetErrorMessage()
	{
//		if (error != null && error.message != null)
//			return error.message;

		return string.Empty;
	}
}

