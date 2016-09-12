using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PedometerDemo : MonoBehaviour {
	
	private PedometerPlugin pedometerPlugin;
	private string demoName="[PedometerDemo] ";
	private UtilsPlugin utilsPlugin;

	public Text hasStepDetectorStatusText;
	public Text stepCountText;
	public Text stepDetectText;

	
	// Use this for initialization
	void Start (){
		//get the instance of pedometer plugin
		pedometerPlugin = PedometerPlugin.GetInstance();

		//set to zero to hide debug toast messages
		pedometerPlugin.SetDebug(0);

		utilsPlugin = UtilsPlugin.GetInstance();
		utilsPlugin.SetDebug(0);

		//check if step detector is supported
		bool hasStepDetector = utilsPlugin.HasStepDetector();

		if(hasStepDetector){
			UpdateStepDetectorStatus("available");

			//initialze pedometer
			pedometerPlugin.Init();
			
			//set this to true to always starts at zero steps, else set to false to continue steps
			pedometerPlugin.SetAlwaysStartAtZero(true);
			
			//set call back listener for pedometer events
			pedometerPlugin.SetCallbackListener(OnStepCount,OnStepDetect);
			
			//register sensor event listener and pass sensor delay that you want
			pedometerPlugin.RegisterSensorListener(SensorDelay.SENSOR_DELAY_FASTEST);
		}else{
			UpdateStepDetectorStatus("not available");
		}
	}

	public void ResetTotalStep(){
		//reset total step to zero
		if(pedometerPlugin!=null){
			pedometerPlugin.ResetTotalStep();
			UpdateStepCount(0);
			Debug.Log( demoName + "ResetTotalStep ");
		}
	}	
				 
	private void OnApplicationPause(bool val){
		if(val){
			//game is pause
			//remove sensor event listener
			if(pedometerPlugin!=null){
				pedometerPlugin.RemoveSensorListener();
			}
		}else{
			//game is resume
			//register sensor event listener
			if(pedometerPlugin!=null){
				pedometerPlugin.ResetTotalStep();
				//register sensor event listener and pass sensor delay that you want
				pedometerPlugin.RegisterSensorListener(SensorDelay.SENSOR_DELAY_FASTEST);
			}
		}
	}

	//step count event is triggered
	private void OnStepCount(int count){
		UpdateStepCount(count);
		Debug.Log( demoName + "OnStepCount count " + count);
	}

	//step detect event is triggered
	private void OnStepDetect(){
		UpdateStepDetect("Detect!");
		Debug.Log( demoName + "OnStepDetect");
	}


	private void UpdateStepDetectorStatus(string val){
		if(hasStepDetectorStatusText!=null){
			hasStepDetectorStatusText.text = String.Format("Status: {0}", val);
		}
	}

	//for updating step count for demo purpose
	private void UpdateStepCount(int count){
		if(stepCountText!=null){
			stepCountText.text = String.Format("Step Count: {0}",count);
		}
	}

	//for showing step is detected for demo purpose
	private void UpdateStepDetect(string val){
		if(stepDetectText!=null){
			stepDetectText.text = String.Format("Step Detect: {0}",val);
		}
	}
}