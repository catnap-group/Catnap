using UnityEngine;
using System.Collections;
using System;

public class PedometerPlugin : MonoBehaviour {
	
	private static PedometerPlugin instance;
	private static GameObject container;
	private static AUPHolder aupHolder;
	
	#if UNITY_ANDROID
	private static AndroidJavaObject jo;
	#endif	
	
	public bool isDebug =true;
	
	public static PedometerPlugin GetInstance(){
		if(instance==null){
			container = new GameObject();
			container.name="PedometerPlugin";
			instance = container.AddComponent( typeof(PedometerPlugin) ) as PedometerPlugin;
			DontDestroyOnLoad(instance.gameObject);
			aupHolder = AUPHolder.GetInstance();
			instance.gameObject.transform.SetParent(aupHolder.gameObject.transform);
		}
		
		return instance;
	}
	
	private void Awake(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo = new AndroidJavaObject("com.gigadrillgames.androidsensor.pedometer.PedometerPlugin");
		}
		#endif
	}
	
	/// <summary>
	/// Sets the debug.
	/// 0 - false, 1 - true
	/// </summary>
	/// <param name="debug">Debug.</param>
	public void SetDebug(int debug){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("SetDebug",debug);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}


	public void Init(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("init");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	public void SetCallbackListener(Action <int>OnStepCount,Action OnStepDetect){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			IPedometerCallback ipedometerCallback = new IPedometerCallback();
			ipedometerCallback.OnStepCount = OnStepCount;
			ipedometerCallback.OnStepDetect = OnStepDetect;
			jo.CallStatic("setCallbackListener",ipedometerCallback);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Sets the step to always start at zero when app is started.
	/// </summary>
	/// <param name="val">If set to <c>true</c> value.</param>
	public void SetAlwaysStartAtZero(bool val){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("setAlwaysStartAtZero",val);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Resets the total step. because steps is counted when the device booted to manually reset it restart the device
	/// </summary>
	public void ResetTotalStep(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("resetTotalStep");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	public void RegisterSensorListener(SensorDelay sensorDelay){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("registerSensorListener",(int)sensorDelay);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	
	public void RemoveSensorListener(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("removeSensorListener");
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}

	
	/// <summary>
	/// Shows the native alert popup.
	/// </summary>
	/// <param name="title">Title.</param>
	/// <param name="message">Message.</param>
	public void ShowAlertPopup(String title,String message){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("showNativePopup",title,message);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	
	
	public void ShowMessage(string message){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.Call("ShowMessage", message);
		}else{
			Message("warning: must run in actual android device");
		}
		#endif
	}
	
	private void Message(string message){
		if(isDebug){
			Debug.LogWarning(message);
		}
	}
}