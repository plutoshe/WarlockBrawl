﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Status: NetworkBehaviour {
	public List<GameObject> statusDemo;
	public GameObject statusItemPrefab;
	public bool isDisplay;
	public RectTransform rect;
	void Start() {
		transform.position = new Vector3 (0, Screen.height, 0);
		rect = GetComponent<RectTransform>();
		isDisplay = true;
	}

	public void ToggleVisibility() {
		isDisplay = !isDisplay;
		transform.GetChild (1).gameObject.SetActive (isDisplay);
		for (int i = 0; i < statusDemo.Count; i++) {
			statusDemo [i].SetActive (isDisplay);
		}

		if (!isDisplay) 
			rect.sizeDelta = new Vector2 (rect.rect.width, 20);
		else 
			rect.sizeDelta = new Vector2 (rect.rect.width, 40 + statusDemo.Count * 20);
		
	}

	void Update() {

		var players = GameObject.FindGameObjectsWithTag ("Player");

//		Debug.Log (statusDemo.Count);
		var statusList = new List<KeyValuePair<KeyValuePair<string, Color>, int>>();
		for (int i = 0; i < players.Length; i++) {
			Health status = players[i].GetComponent<Health> ();

			statusList.Add(new KeyValuePair<KeyValuePair<string, Color>, int>(
				new KeyValuePair<string, Color>(status.playerName, status.playerColor), 
				status.score + status.healthNum * 500));
		}
		statusList.Sort((pair1,pair2) => -pair1.Value.CompareTo(pair2.Value));

		if (isDisplay) {
			if (players.Length != statusDemo.Count) {
				while (players.Length > statusDemo.Count) {
					int i = statusDemo.Count;
					var children = (GameObject)Instantiate (statusItemPrefab);
					children.transform.parent = gameObject.transform;
					children.transform.localPosition = new Vector3 (0, -20 * i - 40, 0);
					statusDemo.Add (children);
				}
				while (players.Length < statusDemo.Count) {
					Destroy (statusDemo [statusDemo.Count - 1]);
					statusDemo.RemoveAt (statusDemo.Count - 1);
				}
				rect.sizeDelta = new Vector2 (rect.rect.width, 40 + statusDemo.Count * 20);
			}
			

			for (int i = 0; i < statusList.Count; i++) {
				var textArr = statusDemo [i].GetComponentsInChildren<Text> ();
				var imageArr = statusDemo [i].GetComponentsInChildren<Image> ();
				for (int j = 0; j < textArr.Length; j++) {
					if (textArr [j].gameObject.name == "PlayerName")
						textArr [j].text = statusList [i].Key.Key;
					if (textArr [j].gameObject.name == "Score")
						textArr [j].text = "" + statusList [i].Value;
					textArr [j].color = statusList [i].Key.Value;
				}
				for (int j = 0; j < imageArr.Length; j++) {
					imageArr [j].color = statusList [i].Key.Value;
				}
			}
		}
	}
}
