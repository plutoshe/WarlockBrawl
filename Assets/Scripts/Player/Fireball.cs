using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

	void OnTriggerEnter(Collider collider) {
		Debug.Log (collider.gameObject.name);
		var hit = collider.gameObject;
		var health = hit.GetComponent<Health>();
		if (health  != null)
		{
			health.TakeDamage(10);
			Destroy(gameObject);
		}
		if (collider.gameObject.name == "ManaShield") 
			Destroy (gameObject);
		
	}
}