using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Status: NetworkBehaviour {
	public GameObject statusItemPrefab;
	private GameObject Popup;
	private bool isDisplay;
	private RectTransform rect;
	private List<GameObject> statusDemo;
	int playerNum = 0;
	void Start() {
		statusDemo = new List<GameObject> ();
		transform.position = new Vector3 (0, Screen.height, 0);
		rect = GetComponent<RectTransform>();
		Popup = transform.parent.GetChild (1).gameObject;
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
		var statusList = new List<KeyValuePair<int, int>>();
		var players = GameObject.FindGameObjectsWithTag ("Player");
		if (players.Length > playerNum)
			playerNum = players.Length;
//		Debug.Log (statusDemo.Count);

		int remainPlayer = 0;
		int winner = 0;
		for (int i = 0; i < players.Length; i++) {
			Health status = players[i].GetComponent<Health> ();
			if (status.healthNum >= 0) {
				remainPlayer++;
				winner = i;
			}
			statusList.Add(new KeyValuePair<int, int>(i,  status.score + status.healthNum * 500));
		}
		statusList.Sort((pair1,pair2) => -pair1.Value.CompareTo(pair2.Value));
	
		if (remainPlayer < playerNum && remainPlayer <= 1 || statusList [0].Value >= 4000) {
			if (statusList [0].Value >= 4000 && remainPlayer > 1) 
				winner = statusList [0].Key;
			var message = Popup.transform.GetChild (0);
			message.gameObject.GetComponent<Text> ().text = players[winner].GetComponent<Health> ().playerName + " Win!";
			var proceed = Popup.transform.GetChild (1);
			proceed.gameObject.SetActive (false);
			for (int i = 0; i < players.Length; i++) 
				players [i].GetComponent<Health> ().StopModel (true);
			Popup.SetActive (true);

		}
			
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
					var item =  players [statusList [i].Key].GetComponent<Health>();
					if (textArr [j].gameObject.name == "PlayerName")
						textArr [j].text = item.playerName;
					if (textArr [j].gameObject.name == "Health")
						textArr [j].text = "" + item.currentHealth;
					if (textArr [j].gameObject.name == "HealthNum") 
						if (item.healthNum < 0)
							textArr [j].text = "Die";
						else
							textArr [j].text = "" + item.healthNum;
					if (textArr [j].gameObject.name == "Score")
						textArr [j].text = "" + statusList [i].Value;
					textArr [j].color = players[statusList [i].Key].GetComponent<Health> ().playerColor;
				}
				for (int j = 0; j < imageArr.Length; j++) {
					imageArr [j].color = players[statusList [i].Key].GetComponent<Health> ().playerColor;
				}
			}
		}
	}
}
