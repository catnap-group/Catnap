using UnityEngine;
using System.Collections;
using System;

public class IPedometerCallback :  AndroidJavaProxy {
	
	public Action <int>OnStepCount;
	public Action OnStepDetect;
	
	public IPedometerCallback() : base("com.gigadrillgames.androidsensor.pedometer.IPedometerCallback") {}

	void onStepCount(int count){
		OnStepCount(count);
	}

	void onStepDetect(){
		OnStepDetect();
	}
}