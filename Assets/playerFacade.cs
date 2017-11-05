using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class playerFacade : MonoBehaviour {
	public Material LocalMaterial;
	void Start() 
	{
		GetComponent<SkinnedMeshRenderer>().material = LocalMaterial;
	}
}
