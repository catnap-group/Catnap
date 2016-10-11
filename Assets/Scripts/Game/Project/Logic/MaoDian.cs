using UnityEngine;
using System.Collections;

public class MaoDian : MonoBehaviour {

	// Use this for initialization
	void Start () {
		EventListener.AddListener (ObjectEvent.SendSound, delegate() {
			EventListener.Broadcast (ObjectEvent.CallCat, gameObject);
		});
	}

}
