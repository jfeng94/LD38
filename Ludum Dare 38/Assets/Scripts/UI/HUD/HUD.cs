using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
	public ResourceBar healthBar;
	public ResourceBar manaBar;

	public GameObject gameOverScreen;

	public Player player;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		int health = player.GetHealth();
		int mana   = player.GetMana();

		healthBar.SetValue(health);
		manaBar.SetValue(mana, player.GetManaRegenProgress());

		if (health <= 0) {
			gameOverScreen.SetActive(true);
		}
	}
}
