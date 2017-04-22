using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Interactable {
	private Player aggroTarget = null;

	// Where the enemy spawns at
	public Vector3 spawnPosition = Vector3.zero;

	// How far away from the enemy's spawn is it willing to go.
	public float leashLength = 5f;

	public float movementSpeed = 0.1f;

	// Use this for initialization
	public override void Start () {
		spawnPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// Get distance from spawn
		float distanceFromSpawn = (transform.position - spawnPosition).magnitude;

		// Follow the player if they are holding aggro
		if (aggroTarget != null) {
			// If the enemy is sufficiently far from the spawn position, lose aggro
			if (distanceFromSpawn > leashLength) {
				aggroTarget = null;
			}

			// Otherwise, move towards the player
			else {
				MoveTowardsPosition(aggroTarget.transform.position);
			}
		}

		// If there is no aggro, move towards the spawn point
		if (aggroTarget == null) {
			MoveTowardsPosition(spawnPosition);
		}
	}

	public override void OnPlayerCanInteract(Player player) {
		base.OnPlayerCanInteract(player);

		if (player != null) {
			aggroTarget = player;
		}
	}


	protected virtual void MoveTowardsPosition(Vector3 position) {
		Vector3 displacement = position - transform.position;

		// Get rid of any vertical component
		displacement.y = 0;

		// Turn displacement into a direction
		Vector3 direction = displacement.normalized;

		// Move towards the player
		transform.position += (direction * movementSpeed);
	}
}
