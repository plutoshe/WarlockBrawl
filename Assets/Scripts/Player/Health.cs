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

		if (!isDead && currentHealth <= 0)
		{
			isDead = true;
			movement.enabled = false;
			ability.enabled = false;
			anim.SetTrigger ("Die");
			currentHealth = 0;
			Debug.Log("Dead!");
			if (HealthNum <= 0) {
			} else {
				HealthNum--;
				StartCoroutine (Finale (2f));
			}

		}
	}

	void OnChangeHealth (int currentHealth)
	{
		healthBar.sizeDelta = new Vector2(currentHealth/2, healthBar.sizeDelta.y);
	}

	IEnumerator Finale(float waitTime) {

		yield return new WaitForSeconds (waitTime);
		anim.SetTrigger ("Respawn");
		yield return new WaitForSeconds (0.5f);
		movement.enabled = true;
		movement.enabled = true;
		isDead = false;
		currentHealth = 100;
	}
}
