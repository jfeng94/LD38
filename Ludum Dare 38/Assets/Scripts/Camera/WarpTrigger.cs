using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTrigger : WarpObject {
	public GameObject target;

	public Player playerToWarp = null;

	private static float leniency = 0.1f;

	void Update() {
		if (playerToWarp != null) {
			if ( Mathf.Abs(playerToWarp.transform.position.x - transform.position.x) < leniency ) {
				Warp(playerToWarp);
			}
		}
	}

	public virtual void OnTriggerEnter2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			playerToWarp = player;
		}
	}

	// Moves the player to the linked target. Also moves the camera so the player's relative 
	// position on screen doesn't change.
	private void Warp(Player player) {
		// Cache the old position of the player. We'll use this to calculate a total displacement
		// in order to move the camera after moving the player
		Vector3 oldPosition = player.transform.position;

		// Move the player to the same x-position as the warp target
		Vector3 position = oldPosition;
		position.x = target.transform.position.x;
		player.transform.position = position;

		// Move the camera the same amount we moved the player
		Vector3 displacement = position - oldPosition;
		Camera.main.transform.position += displacement;

		// Move all enemies with aggro on the character.
		List<Enemy> enemies = player.GetAggroEnemies();
		for (int i = 0; i < enemies.Count; i++) {
			enemies[i].transform.position += displacement;
		}

		// Clean up any references to the player we may be holding onto.
		playerToWarp = null;
	}

}
