using UnityEngine;
using System.Collections;
public class Billboard : MonoBehaviour {  
	
	public Camera mCamera;  
	public bool needSynRotate = false;
	Quaternion direction=new Quaternion();  
	
	// Use this for initialization  
	void Start () {  
		if (mCamera == null) 
		{
			mCamera = Camera.main;
		}
		direction.x = transform.localRotation.x;  
		direction.y = transform.localRotation.y;  
		direction.z = transform.localRotation.z;  
		direction.w = transform.localRotation.w;  


	}  
	float deltaTime = 0.0f;
	// Update is called once per frame  
	void Update ()  
	{  
		Camera cam=null;  
		if (mCamera != null)  
		{  
			cam = mCamera;  
		}  
		else  
		{  
			cam = Camera.current;  
			if (!cam)  
				return;  
		}  
		deltaTime += Time.deltaTime;
		if(needSynRotate)
			transform.rotation = cam.transform.rotation * new Quaternion (direction.x, direction.y, direction.z-transform.localRotation.x, direction.w);  
		else
			transform.rotation = cam.transform.rotation * new Quaternion (direction.x, direction.y, direction.z, direction.w);  //-transform.localRotation.x
	}  
	
}  
