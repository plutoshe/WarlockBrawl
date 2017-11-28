using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Health : NetworkBehaviour {
	// SynVar setting
	[SyncVar]
	public string playerName = "";
	[SyncVar(hook="OnChangeColor")]
	public Color playerColor = Color.blue;
	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth;
	[SyncVar]
	public int healthNum = 2;
	[SyncVar]
	public int score = 0;
	public bool isDead = false;

	// Local status variables
	public RectTransform healthBar;
	public const int maxHealth = 100;


	// for Damage per incrementTime second
	private float waitTime = 0f;
	private float incrementTime = 2f;
	public int DamagePerInterval = 0;

	// for Score per interval
	private float waitStatusTime = 0f;
	private float incrementStatusTime = 10f;

	// dead action setting
	private Movement movement;
	private Ability ability;
	private NetworkAnimator anim;
	public GameObject Popup;

	void changeFacade() {
		var childrenMaterial = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(var children in childrenMaterial)
		{
			if (children.name == "Player") {
				children.GetComponent<SkinnedMeshRenderer>().material.color = playerColor;
			}
		}
	}

	public void Start() {
		anim = GetComponent <NetworkAnimator> ();
		changeFacade ();
	}

	public override void OnStartLocalPlayer() {
		movement = GetComponent <Movement> ();
		ability = GetComponent <Ability> ();
		CmdScoreSet (0);
		CmdHealthSet(maxHealth);

		Popup = GameObject.FindGameObjectWithTag ("Popup");
		var proceed = Popup.transform.GetChild (1);
		proceed.gameObject.SetActive (true);
		CmdColorSet (playerColor);

	}

	public void OnChangeColor(Color newColor) {
		playerColor = newColor;
		changeFacade ();
	}


	public void OnChangeHealth (int newCurrentHealth)
	{
		currentHealth = newCurrentHealth;
		healthBar.sizeDelta = new Vector2(currentHealth/2, healthBar.sizeDelta.y);
	}


	public void AlterDamgePerSecond(int damage) {
		DamagePerInterval = damage;
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
	void CmdColorSet(Color color) {
		playerColor = color;
	}

	[Command] 
	void CmdHealthAddition(int delta) {
		currentHealth += delta;
	}
	[Command]
	void CmdScoreSet(int value) {
		score = value;
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
		if (movement != null) {
			movement.enabled = !status;
			ability.enabled = !status;
			movement.navMeshAgent.isStopped = status;
		}
	}

		
	void Update() { //only local trigger
		if (currentHealth > 0)
			isDead = false;
			
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
			CmdHealthAddition (-DamagePerInterval);
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
		if (currentHealth <= 0)
		{
			isDead = true;
			StopModel (true);

			anim.SetTrigger ("Die");
			anim.animator.ResetTrigger ("Die");
			CmdHealthSet (0);
			Debug.Log("Dead!");
			if (healthNum <= 0) 
				Popup.SetActive (true);
			else 
				StartCoroutine (Finale (5f));
			
			CmdHealthNumDecrease ();

		}
	}

	IEnumerator Finale(float waitTime) {

		yield return new WaitForSeconds (waitTime);

		Respawn ();
	}

	void Respawn()
	{
		CmdHealthSet (maxHealth);
		StopModel (false);
		anim.SetTrigger ("Respawn");
		anim.animator.ResetTrigger ("Respawn");
		var safeFloor = GameObject.FindGameObjectsWithTag ("SafeFloor")[0];
		var angle = Random.Range (0, 360) * Mathf.PI / 180;
		var x = Random.Range (0, safeFloor.transform.localScale.x) / 2;

		var z = x * Mathf.Sin (angle);
		x = x * Mathf.Cos(angle);
		var navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		transform.position = new Vector3 (x, 0, z);
		navMeshAgent.destination = transform.position;
		navMeshAgent.isStopped  = true;

		Camera.main.GetComponent<CameraFollow> ().FollowTarget ();
	}


		
}
