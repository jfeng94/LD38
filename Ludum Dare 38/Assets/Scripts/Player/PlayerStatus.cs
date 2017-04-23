using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Will hold onto the player's current status, i.e. money, health, mana, etc.
////     Whatever HUD we write will probably have to look at this class.
public class PlayerStatus : MonoBehaviour {
	private int maxHealth = 5;
	private int maxMana   = 4;

	private int health = 5;
	private int mana   = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// HEALTH
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void GainHealth(int amount) {
		health += amount;
		EnsureHealthIsWithinBounds();
	}

	public void SpendHealth(int amount) {
		health -= amount;
		EnsureHealthIsWithinBounds();
	}

	private void EnsureHealthIsWithinBounds() {
		if (health <= 0) {
			Debug.Log("Game Over!");
		}
		if (health > maxHealth) {
			health = maxHealth;
		}
	}

	public int GetHealth() {
		return health;
	}

	public int GetMaxHealth() {
		return maxHealth;
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MANA
	////////////////////////////////////////////////////////////////////////////////////////////////

	public int GetMana() {
		return mana;
	}
	public int GetMaxMana() {
		return maxMana;
	}	
}
