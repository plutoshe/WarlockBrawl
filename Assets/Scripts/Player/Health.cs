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


	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;

	public RectTransform healthBar;


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
//			OnChangeHealth (currentHealth);
		}
	}

	void OnChangeHealth (int currentHealth)
	{
		healthBar.sizeDelta = new Vector2(currentHealth/2, healthBar.sizeDelta.y);
	}
}
