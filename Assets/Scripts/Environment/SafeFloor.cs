using UnityEngine;
using System.Collections;

public class SafeFloor : MonoBehaviour {
	public float[] WaitTime;
	public float[] ChangeTime;
	public float[] ChangeSize;
	private float startTime;
	private float changeTotalTime;
	bool shrinkScale = false;
	Vector3 startMarker, endMarker;


	void Start() {
		StartCoroutine (IntervalChange());
	}

	IEnumerator IntervalChange() {
		for (int i = 0; i < WaitTime.Length; i++) {
			yield return new WaitForSeconds (WaitTime [i]);
			while (shrinkScale);
			
			startMarker = transform.localScale;
			endMarker = new Vector3(ChangeSize [i], transform.localScale.y, ChangeSize[i]);
			startTime = Time.time;
			changeTotalTime = ChangeTime [i];
			shrinkScale = true;
			yield return new WaitForSeconds (ChangeTime [i] + 0.5f);
		}
	}

	void OnCollisionEnter() {
		Debug.Log ("Collision in");
	}
		
	void Update() {
		if (shrinkScale) {
			float frac = (Time.time - startTime) / changeTotalTime;
			if (frac >= 1) {
				frac = 1;
				shrinkScale = false;
			}
			transform.localScale = Vector3.Lerp(startMarker, endMarker, frac);

		}
	}

	void OnTriggerEnter(Collider collider) {
		var hit = collider.gameObject;
		var health = hit.GetComponent<Health> ();
		if (health != null) {
			health.AlterDamgePerSecond(0);
		}

	}

	void OnTriggerExit(Collider collider) {

		var hit = collider.gameObject;
		var health = hit.GetComponent<Health> ();
		if (health != null) {
			health.AlterDamgePerSecond(5);
		}

	}


		
}