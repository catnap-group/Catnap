using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ARMange : MonoBehaviour {

	public List<PlaceInfo> places = new List<PlaceInfo>();
	public GameObject perfab;
	public PlaceInfo location = new PlaceInfo ();
	public void ShowPlaces(){
		ClearPlace ();

		for (int i = 0; i < places.Count; i++) {

			GameObject newPlace = Instantiate<GameObject> (perfab);
			newPlace.transform.parent = this.transform;

			double posZ = places [i].Latitude - location.Latitude;
			double posX = places [i].Longitude - location.Longitude;

			float z = 0;
			float x = 0;
			float y = 0;

			if (posZ > 0) {
				z = 500f;
			} else {
				z = -500f;
			}

			if (posX > 0) {
				x = 500f;
			} else {
				x = -500f;
			}

			z = z + (float)(posZ * 1000);
			x = x + (float)(posX * 1000);
			y = y + i * 20;

			newPlace.transform.position = new Vector3 (-x, y, -z);
			newPlace.transform.LookAt (this.transform);
			newPlace.transform.Rotate (new Vector3 (0f, 180f, 0f));
			newPlace.gameObject.GetComponentsInChildren<Text> ()[0].text = places [i].Name;
			newPlace.gameObject.GetComponentsInChildren<Text> ()[1].text = places [i].Distance.ToString() + "m";
			Place place =newPlace.GetComponent<Place> ();
			place.info = places [i];
		}
	}

	private void ClearPlace(){
		GameObject[] oldPlaces = GameObject.FindGameObjectsWithTag ("Place");
		for (int i = 0; i < oldPlaces.Length; i++) {
			Destroy (oldPlaces [i].gameObject);
		}
	}

}
