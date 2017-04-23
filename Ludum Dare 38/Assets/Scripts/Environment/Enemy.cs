using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Interactable {
	private Player aggroTarget = null;
	private Player aggroCandidate = null;
	// Where the enemy spawns at
	public Vector3 spawnPosition = Vector3.zero;

	// How far away from the enemy's spawn is it willing to go.
	public float leashLength = 5f;

	public float movementSpeed = 0.1f;

	// Use this for initialization
	void Start () {
		spawnPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// Get distance from spawn
		float distanceFromSpawn = (transform.position - spawnPosition).magnitude;
		
		// Check aggro candidate.
		if (aggroCandidate != null) {
			float targetDistanceFromSpawn = (aggroCandidate.transform.position - spawnPosition).magnitude;
			// Follow target if they hold aggro.
			if (aggroTarget != null) {

				// If the target is sufficiently far from the spawn position, lose aggro
				if (targetDistanceFromSpawn > leashLength) {
					
					base.OnPlayerCannotInteract (aggroTarget);
					aggroTarget = null;
				}

				// Otherwise, move towards the player
				else {
					MoveTowardsPosition (aggroTarget.transform.position);
				}
			}
			// Check if aggro candidate is back in leash range.
			else if (targetDistanceFromSpawn <= leashLength) {
				base.OnPlayerCanInteract (aggroCandidate);
				aggroTarget = aggroCandidate;
				
			} else {
				
				MoveTowardsPosition(spawnPosition);
			}
		}

		// If there is no aggro candidate, return to spawn point.
		if (aggroCandidate == null) {
			MoveTowardsPosition(spawnPosition);
		}
	}

	public override void OnPlayerCanInteract(Player player) {
		base.OnPlayerCanInteract(player);

		if (player != null) {
			aggroTarget = player;
			aggroCandidate = player;
		}
	}

	public override void OnPlayerCannotInteract(Player player) {
		base.OnPlayerCannotInteract (player);

		aggroTarget = null;
		aggroCandidate = null;
	}

	protected virtual void MoveTowardsPosition(Vector3 position) {
		Vector3 displacement = position - transform.position;

		// Get rid of any vertical component
		displacement.y = 0;

		// Always push left?
		displacement.x = -1;
		// Turn displacement into a direction
		Vector3 direction = displacement.normalized;

		// Move towards the player
		transform.position += (direction * movementSpeed);
	}
}
