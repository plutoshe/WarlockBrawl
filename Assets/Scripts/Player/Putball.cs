using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Putball : MonoBehaviour {


	void OnTriggerEnter(Collider collider) {
		
		var hit = collider.gameObject;
		var health = hit.GetComponent<Health>();
		var movement = hit.GetComponent<Movement> ();
		if (health  != null)
		{
			Destroy (gameObject);
			health.TakeDamage(5);
			movement.Impluse (30 * transform.forward.normalized);

		}
		if (collider.gameObject.name == "ManaShield") 
			Destroy (gameObject);
	}
}
