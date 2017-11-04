using UnityEngine;
using System.Collections;

public class SafeFloor : MonoBehaviour {


	void OnCollisionEnter() {
		Debug.Log ("Collision in");
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