﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Health : NetworkBehaviour {
	// SynVar setting
	[SyncVar]
	public string playerName = "";
	[SyncVar]
	public Color playerColor = Color.blue;
	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth;
	[SyncVar]
	public int healthNum = 2;
	[SyncVar]
	public int score = 0;

	// Local status variables
	public RectTransform healthBar;
	public const int maxHealth = 100;
	public int DamagePerSecond = 0;

	// for Damage per second
	private float waitTime = 0f;
	private float incrementTime = 1f;

	// for Score per interval
	private float waitStatusTime = 0f;
	private float incrementStatusTime = 10f;

	// dead action setting
	private Movement movement;
	private Ability ability;
	private Animator anim;
	bool isDead = false;
	public GameObject Popup;

	void Start() {
		movement = GetComponent <Movement> ();
		ability = GetComponent <Ability> ();
		anim = GetComponent <Animator> ();
		score = 0;
		currentHealth = maxHealth;

		Popup = GameObject.FindGameObjectWithTag ("Popup");
		var proceed = Popup.transform.GetChild (1);
		proceed.gameObject.SetActive (true);
		Popup.SetActive (false);


		var childrenMaterial = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(var children in childrenMaterial)
		{
			if (children.name == "Player") {
				children.GetComponent<SkinnedMeshRenderer>().material.color = playerColor;
			}
		}
	}

	public void OnChangeHealth (int newCurrentHealth)
	{
		currentHealth = newCurrentHealth;
		healthBar.sizeDelta = new Vector2(currentHealth/2, healthBar.sizeDelta.y);
	}


	public void AlterDamgePerSecond(int damage) {
		DamagePerSecond = damage;
		waitTime = 0f;

	}

	public void TakeDamage(int amount) {
		if (!isServer)
		{
			return;
		}
		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Debug.Log("Dead!");
		}
	}

	[Command] 
	void CmdHealthAddition(int delta) {
		currentHealth += delta;
	}

	[Command]
	void CmdHealthSet(int value) {
		currentHealth = value;
	}

	[Command] 
	void CmdScoreAddition(int delta) {
		score += delta;
	}

	[Command]
	void CmdHealthNumDecrease() {
		healthNum--;
	}

	public void StopModel(bool status) {
		isDead = status;
		movement.enabled = !status;
		ability.enabled = !status;
		movement.navMeshAgent.isStopped = status;
	}

		
	void Update() { //only local trigger
		if (!isLocalPlayer || isDead) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			Popup.SetActive (true);
		}

		// update damage per second
		waitTime+=Time.deltaTime;
		if (waitTime>=incrementTime) 
		{
			waitTime-=incrementTime;
			CmdHealthAddition (-DamagePerSecond);
		}
		// update score
		waitStatusTime += Time.deltaTime;
		if (waitStatusTime >= incrementStatusTime) {
			waitStatusTime -= incrementStatusTime;
			CmdScoreAddition(currentHealth);
		}

		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			CmdHealthSet(0);
		}
		if (!isDead && currentHealth <= 0)
		{
			StopModel (true);
			anim.SetTrigger ("Die");
			CmdHealthSet (0);
			Debug.Log("Dead!");
			if (healthNum <= 0) {
				Popup.SetActive (true);
				CmdHealthNumDecrease ();
			} else {
				CmdHealthNumDecrease ();
				StartCoroutine (Finale (5f));
			}

		}
	}

	IEnumerator Finale(float waitTime) {

		yield return new WaitForSeconds (waitTime);

		RpcRespawn ();
	}
		
	void RpcRespawn()
	{
		if (isLocalPlayer)
		{	
			StopModel (false);
			anim.SetTrigger ("Respawn");
			CmdHealthSet (100);
			var safeFloor = GameObject.FindGameObjectsWithTag ("SafeFloor")[0];
			var angle = Random.Range (0, 360) * Mathf.PI / 180;
			var x = Random.Range (0, safeFloor.transform.localScale.x) / 2;

			var z = x * Mathf.Sin (angle);
			x = x * Mathf.Cos(angle);

			transform.position = new Vector3 (x, 0, z);
			Camera.main.GetComponent<CameraFollow> ().FollowTarget ();
		}
	}
		
}
