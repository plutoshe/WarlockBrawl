using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitToLobby : MonoBehaviour {
	Prototype.NetworkLobby.LobbyManager lobby;
	// Use this for initialization
	void Start () {
		var topPanel = GameObject.FindGameObjectWithTag ("Trial");
		lobby = topPanel.transform.parent.GetComponent<Prototype.NetworkLobby.LobbyManager> ();

		var click = GetComponent<Button> ();	
		click.onClick.AddListener (delegate {
			Exit();
		});
	}

	void Exit() {
		lobby.GoBackButton ();
	}


}
