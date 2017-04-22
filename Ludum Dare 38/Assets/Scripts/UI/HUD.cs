using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
	public TextMesh healthWhite;
	public TextMesh healthBlack;
	public TextMesh manaWhite;
	public TextMesh manaBlack;

	public Player player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		int health    = player.status.GetHealth();
		int maxHealth = player.status.GetMaxHealth();

		int mana    = player.status.GetMana();
		int maxMana = player.status.GetMaxMana();

		healthWhite.text = (int) (100f * (float) health / maxHealth) + "%";
		healthBlack.text = (int) (100f * (float) health / maxHealth) + "%";

		manaWhite.text = mana + " / " + maxMana;
		manaBlack.text = mana + " / " + maxMana;
	}
}
