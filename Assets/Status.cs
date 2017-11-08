using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Status : NetworkBehaviour {
	public struct PlayerStats 
	{
		public string name;
		public int score;
		public Color color;
	}

	public class PlayerStatsSyncClass : SyncListStruct<PlayerStats>{}

	public PlayerStatsSyncClass playerStatsList = new PlayerStatsSyncClass();


	public Text tempPrint;

	public int AddPlayer(string playerName, Color playerColor) {
		if (!isServer)
			return -1;
//		new MyStruct { s = "Hello", length = 5 };
		playerStatsList.Add (new PlayerStats{name = playerName, score = 0, color = playerColor});
		return playerStatsList.Count - 1;

	}

	public void Print() {
		Debug.Log ("-----Print Score----");
		foreach (var i in playerStatsList) {
			Debug.Log (i.ToString());
		}
	}

	public void AddScore(int numId) {
		
	}

	// Update is called once per frame
	void Update () {
		string ans = "";
		for (var i = 0; i < playerStatsList.Count; i++) {
			ans += playerStatsList[i];
		}
		tempPrint.text = ans;
	}
}

