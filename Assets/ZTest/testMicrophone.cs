using UnityEngine;
using System.Collections;

public class testMicrophone : MonoBehaviour {


		void Start() {
		int num =  Microphone.devices.Length;
			foreach (string device in Microphone.devices) {
				Debug.Log("Name: " + device);

			}
		}
}
