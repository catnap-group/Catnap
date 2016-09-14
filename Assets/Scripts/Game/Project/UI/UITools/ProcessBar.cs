using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProcessBar : MonoBehaviour {

	public Image foregroundImage;

	public int Value
	{
		get 
		{
			if(foregroundImage != null)
				return (int)(foregroundImage.fillAmount*100);	
			else
				return 0;	
		}
		set 
		{
			if(foregroundImage != null)
				foregroundImage.fillAmount = value/100f;	
		} 
	}
	public void Reset()
	{
		foregroundImage.fillAmount = 0;
	}
	void Start () {
	}	
}
