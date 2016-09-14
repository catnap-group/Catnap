﻿using UnityEngine;
using System.Collections;

public class ScreenShot : MonoBehaviour
{
	[HideInInspector]
	public bool CanGrab =false;
	void OnPostRender()
	{
		if (CanGrab) {
			Texture2D tex = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, false);		
			tex.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
			tex.Apply ();

			CanGrab = false;
			StartCoroutine (UFileUtil.Inst.SaveTextureToFile(tex));
		}
	}
}
