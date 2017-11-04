using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public const int maxHealth = 100;
	public int currentHealth = maxHealth;
	public int DamagePerSecond = 0;
	public RectTransform healthBar;
	private float waitTime = 0f;
	private float incrementTime = 1f;

	public void AlterDamgePerSecond(int damage) {
		DamagePerSecond = damage;
		waitTime = 0f;

	}
	public void TakeDamage(int amount) {
		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Debug.Log("Dead!");
		}
		healthBar.sizeDelta = new Vector2(currentHealth/2, healthBar.sizeDelta.y);
	}

	void Update() {
		
		waitTime+=Time.deltaTime;
		while(waitTime>incrementTime)
		{
			waitTime-=incrementTime;
			currentHealth -= DamagePerSecond;
		}
		healthBar.sizeDelta = new Vector2(currentHealth/2, healthBar.sizeDelta.y);
	}
}
