using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour {

	public const int maxHealth = 100;
	public int DamagePerSecond = 0;
	private float waitTime = 0f;
	private float incrementTime = 1f;
	public int HealthNum = 2;
	public Movement movement;
	public Ability ability;
	public Animator anim;
	bool isDead = false;


	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;

	public RectTransform healthBar;

	void Start() {
		movement = GetComponent <Movement> ();
		ability = GetComponent <Ability> ();
		anim = GetComponent <Animator> ();
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
		

	void Update() {
		waitTime+=Time.deltaTime;
		while(waitTime>incrementTime)
		{
			waitTime-=incrementTime;
			currentHealth -= DamagePerSecond;
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
			if (HealthNum <= 0) {
			} else {
				HealthNum--;
				StartCoroutine (Finale (5f));
			}

		}
	}

	void OnChangeHealth (int currentHealth)
	{
		healthBar.sizeDelta = new Vector2(currentHealth/2, healthBar.sizeDelta.y);
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
			// move back to zero location
//			transform.position = Vector3.zero;
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
