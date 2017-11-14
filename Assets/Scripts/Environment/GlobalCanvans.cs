using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCanvans : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.GetChild (1).gameObject.SetActive (false);
	}
}
