using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Health : NetworkBehaviour {
	// SynVar setting
	[SyncVar]
	public string playerName = "";
	[SyncVar (hook = "OnChangeColor")]
	public Color playerColor = Color.blue;
	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;
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

	void Start() {
		movement = GetComponent <Movement> ();
		ability = GetComponent <Ability> ();
		anim = GetComponent <Animator> ();

		score = 0;
	}

	public void OnChangeHealth (int currentHealth)
	{
		healthBar.sizeDelta = new Vector2(currentHealth/2, healthBar.sizeDelta.y);
	}

	public void OnChangeColor(Color playerColor) {
		var childrenMaterial = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(var children in childrenMaterial)
		{
			if (children.name == "Player") {
				children.GetComponent<SkinnedMeshRenderer>().material.color = playerColor;
			}
		}

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
		

	void Update() { //only local trigger
		if (!isLocalPlayer) {
			return;
		}

		// update damage per second
		waitTime+=Time.deltaTime;
		if (waitTime>=incrementTime) 
		{
			waitTime-=incrementTime;
			currentHealth -= DamagePerSecond;

		}
		// update score
		waitStatusTime += Time.deltaTime;
		if (waitStatusTime >= incrementStatusTime) {
			waitStatusTime -= incrementStatusTime;
			score += currentHealth;
		}

		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			currentHealth = 0;
		}
		if (!isDead && currentHealth <= 0)
		{
			isDead = true;
			movement.enabled = false;
			ability.enabled = false;
			movement.navMeshAgent.isStopped = true;
			anim.SetTrigger ("Die");
			currentHealth = 0;
			Debug.Log("Dead!");
			if (healthNum <= 0) {
			} else {
				healthNum--;
				StartCoroutine (Finale (5f));
			}

		}
	}

	IEnumerator Finale(float waitTime) {

		yield return new WaitForSeconds (waitTime);

		RpcRespawn ();
	}

	[ClientRpc]
	void RpcRespawn()
	{
		if (isLocalPlayer)
		{	
			movement.enabled = true;
			ability.enabled = true;
			isDead = false;
			anim.SetTrigger ("Respawn");
			currentHealth = 100;
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
