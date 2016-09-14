using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	public float shakeTime =0.4f;
	public float waitTime =1.05f;
	public float shakeStrength =0.45f;
	//void Start()
	//{
	//	StartCoroutine(DelayShake());
	//}
	public void OnEffectDelay()
	{
		StartCoroutine(DelayShake ());
	}
	IEnumerator DelayShake()
	{
		yield return new WaitForSeconds(waitTime);
		OnEffect(shakeTime);
	}
	public bool cancelShake;
	public void OnEffect(float shakeTime)
	{
		StartCoroutine (ShakeCamera (shakeStrength, 100, shakeTime));
	}
	/// <summary>
	/// 摄像机震动
	/// </summary>
	/// <param name="shakeStrength">震动幅度</param>
	/// <param name="rate">震动频率</param>
	/// <param name="shakeTime">震动时长</param>
	/// <returns></returns>
	IEnumerator ShakeCamera(float shakeStrength = 0.2f, float rate = 14, float shakeTime = 0.4f){
		float t = 0;
		float speed = 1 / shakeTime;
		Vector3 orgPosition = transform.localPosition;
		while (t < 1 && !cancelShake){
			t += Time.deltaTime * speed;
			transform.localPosition = orgPosition + new Vector3(Mathf.Sin(rate * t), Mathf.Cos(rate * t), 0) * Mathf.Lerp(shakeStrength, 0, t);
			yield return null;
		}
		cancelShake = false;
		transform.localPosition = orgPosition;
	}
}

