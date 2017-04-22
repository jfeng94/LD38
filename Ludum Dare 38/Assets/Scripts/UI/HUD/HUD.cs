﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
	public ResourceBar healthBar;
	public ResourceBar manaBar;

	public Player player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		int health = player.status.GetHealth();
		int mana   = player.status.GetMana();

		healthBar.SetValue(health);
		manaBar.SetValue(mana);
	}
}
