using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Status: NetworkBehaviour {
	public List<GameObject> statusDemo;
	public GameObject statusItemPrefab;

	void Update() {

		var players = GameObject.FindGameObjectsWithTag ("Player");
		while (players.Length > statusDemo.Count) {
			int i = statusDemo.Count;
			var children = (GameObject)Instantiate (statusItemPrefab);
			children.transform.parent = gameObject.transform;
			children.transform.localPosition = new Vector3 (0, 18.75f * i, 0);
			statusDemo.Add(children);
				
			
		}
//		Debug.Log (statusDemo.Count);
		for (int i = 0; i < players.Length; i++) {
			
			Health status = players[i].GetComponent<Health> ();

			var textArr = statusDemo[i].GetComponentsInChildren<Text>();
			var imageArr = statusDemo[i].GetComponentsInChildren<Image>();
			for (int j = 0; j < textArr.Length; j++) {
				if (textArr [j].gameObject.name == "PlayerName") 
					textArr [j].text = status.playerName;
				if (textArr [j].gameObject.name == "Score")
					textArr [j].text = "" + status.score;
				textArr [j].color = status.playerColor;
			}
			for (int j = 0; j < imageArr.Length; j++) {
				imageArr [j].color = status.playerColor;
			}
		}
	}
}
