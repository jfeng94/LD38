﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Will hold onto the player's current status, i.e. money, health, mana, etc.
////     Whatever HUD we write will probably have to look at this class.
public class PlayerStatus : MonoBehaviour {
	private int maxHealth = 100;
	private int maxMana   = 4;

	private int health = 100;
	private int mana   = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetHealth() {
		return health;
	}

	public int GetMana() {
		return mana;
	}

	public int GetMaxHealth() {
		return maxHealth;
	}

	public int GetMaxMana() {
		return maxMana;
	}	
}
