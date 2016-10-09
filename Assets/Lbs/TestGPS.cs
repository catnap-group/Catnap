using UnityEngine;
using System.Collections;

public class TestGPS : UnityAllSceneSingletonVisible<TestGPS> {
	public string gps_info = "";  
	public int flash_num = 1;  
	bool gpsInitialed = false;
	bool stopGps = false;
	void OnGUI () {  
		GUI.skin.label.fontSize = 28;  
		GUI.Label(new Rect(20,20,600,48),this.gps_info);   
		GUI.Label(new Rect(20,50,600,48),"gps flash num:" + this.flash_num.ToString());   

		GUI.skin.button.fontSize = 50;  
		if (GUI.Button(new Rect(Screen.width-210,0,220,85),"GPS定位"))  
		{  
			stopGps = false;
			if (gpsInitialed)
				return;
			// 这里需要启动一个协同程序  
			StartCoroutine(StartGPS());  
			StartCoroutine(UploadGPS());  
		}  
		if (GUI.Button(new Rect(Screen.width-210,100,220,85),"刷新GPS"))  
		{  
			this.gps_info = "N:" + Input.location.lastData.latitude + " E:"+Input.location.lastData.longitude;  
			this.gps_info = this.gps_info + " Time:" + Input.location.lastData.timestamp;  
			this.flash_num += 1;   
		}  
		if (GUI.Button (new Rect (Screen.width - 210, 200, 220, 85), "关闭GPS")) { 
			StopGPS();
		}
	}  
	void StopGPS () {  
		stopGps = true;
		Input.location.Stop();  
		gpsInitialed = false;
	} 
	IEnumerator UploadGPS()
	{
		while(!gpsInitialed)
			yield return new WaitForSeconds(1); 
		while (!stopGps) {
			CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().InitCustomArgs();
			CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().SetCustomArg("uid", GamePlayer.Me.instance.id);
			CatnapWebMgr.Instance.CastFor<CatnapWebMgr> ().SetCustomArg ("longitude",  Input.location.lastData.latitude);
			CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().SetCustomArg("latitude", Input.location.lastData.longitude);
			CatnapWebMgr.Instance.CastFor<CatnapWebMgr>().RequestByWRI(EWebRequestId.MSG_LBS_UPLOAD_LOCATION, OnUploadResponse, OnWebError);
			yield return new WaitForSeconds(1); 
		}	
	}
	void OnUploadResponse(uint id, object obj, object localArg)
	{
		Debug.Log ("id" + id);
		if (obj == null) {
			Debug.LogError ("no response data!!!");
			return;
		}
	}
	void OnWebError(uint id, string msg)
	{
		Debug.Log(msg);
	}
	IEnumerator StartGPS () {  
//		#if UNITY_EDITOR
//			gpsInitialed = true;
//			yield break; 
//		#endif
		// Input.location 用于访问设备的位置属性（手持设备）, 静态的LocationService位置  
		// LocationService.isEnabledByUser 用户设置里的定位服务是否启用  
		if (!Input.location.isEnabledByUser) {  
			this.gps_info = "isEnabledByUser value is:"+Input.location.isEnabledByUser.ToString()+" Please turn on the GPS";   
			yield break; 
		}  

		// LocationService.Start() 启动位置服务的更新,最后一个位置坐标会被使用  
		Input.location.Start(10.0f, 10.0f);  

		int maxWait = 20;  
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {  
			// 暂停协同程序的执行(1秒)  
			yield return new WaitForSeconds(1);  
			maxWait--;  
		}  

		if (maxWait < 1) {  
			this.gps_info = "Init GPS service time out";  
			yield break;   
		}  
		while (!stopGps) {
			if (Input.location.status == LocationServiceStatus.Failed) {  
				this.gps_info = "Unable to determine device location";  
				yield break;   
			} else {  
//			if (!Input.gyro.enabled) {
//				Input.gyro.enabled = true; 
//			}
				//Input.gyro.attitude
				this.gps_info = "N:" + Input.location.lastData.latitude + " E:" + Input.location.lastData.longitude;  
				this.gps_info = this.gps_info + " Time:" + Input.location.lastData.timestamp;  
				gpsInitialed = true;
				yield return new WaitForSeconds (100);  
			}  
		}

	}  
}
