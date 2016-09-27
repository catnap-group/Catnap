using UnityEngine;
using System.Collections;

public enum EWebRequestId
{
	MSG_TEST = 10000,
	MSG_LBS_UPLOAD_LOCATION = 10001,
}
public class ProtoCommand : MonoBehaviour {
	public static void Register(CatnapWebMgr webMgr)
	{
		UWebRequest request = null;
		{
			request = new UWebJsonRequest ();
			request.id = (uint)EWebRequestId.MSG_TEST;
			request.remoteMethod = "register";
			request.remoteFolder = "user";
			request.remoteName = "index.php";
			request.runtimeClass = "LBSJPTest";
			request.isCompressed = false;
			request.AddArgument ("mac",GameManager.Instance.GetMacAddress() );
//			request.AddArgument("valStr", "abcd");
//			request.AddArgument("valInt", 1234);
//			request.AddArgument("valDouble", 12.34);
			webMgr.RegisterByWRI(EWebRequestId.MSG_TEST, request);
		}
	}
}
