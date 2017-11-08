using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Status: NetworkBehaviour {
	public Text TempText;
	void Update() {
		string ans = "";
		var players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in players) {
			Health status = player.GetComponent<Health> ();
			ans += status.playerName + " " + (status.score + status.healthNum * 500) + "\n";
		}
		TempText.text = ans;
	}
}
