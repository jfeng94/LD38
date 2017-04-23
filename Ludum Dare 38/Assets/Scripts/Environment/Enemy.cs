using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public GameObject indicator;

	private Player aggroTarget = null;

	// Where the enemy spawns at
	public Vector3 spawnPosition = Vector3.zero;

	// How far away from the enemy's spawn is it willing to go.
	public float leashLength = 20f;

	public float movementSpeed = 4f;

	// Use this for initialization
	public void Start () {
		spawnPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// Get distance from spawn
		float distanceFromSpawn = (transform.position - spawnPosition).magnitude;

		// Follow the player if they are holding aggro
		if (aggroTarget != null) {
			// If the enemy is sufficiently far from the spawn position, lose aggro
			//# if (distanceFromSpawn > leashLength) {
			//# 	aggroTarget = null;
			//# }

			// Otherwise, move towards the player
			//# else {
				MoveTowardsPosition(aggroTarget.transform.position);
			//# }
		}

		// If there is no aggro, move towards the spawn point
		if (aggroTarget == null) {
			MoveTowardsPosition(spawnPosition);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// PHYSICS
	////////////////////////////////////////////////////////////////////////////////////////////////
	public virtual void OnTriggerEnter2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			SetAggro(player);
			ShowIndicator();
			Invoke("HideIndicator", 1f);
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// AGGRO MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public float aggroRange = 10f;

	public void SetAggro(Player player) {
		if (player != null) {
			aggroTarget = player;
			aggroTarget.AddEnemyAggro(this);
		}
	}

	public void RemoveAggro(Player player) {
		if (player == aggroTarget) {
			Debug.Log("Enemy lost aggro", this);
			aggroTarget = null;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// 
	////////////////////////////////////////////////////////////////////////////////////////////////
	private void ShowIndicator() {
		if (indicator != null) {
			indicator.SetActive(true);
		}
	}

	private void HideIndicator() {
		if (indicator != null) {
			indicator.SetActive(false);
		}
	}

	protected virtual void MoveTowardsPosition(Vector3 position) {
		float displacement = position.x - transform.position.x;

		if (Mathf.Abs(displacement) < 0.2f) {
			return;
		}

		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		if (rb != null) {
			Vector3 velocity = rb.velocity;
			velocity.x = movementSpeed;
			if (displacement < 0f) {
				velocity.x *= -1f;
			}

			rb.velocity = velocity;
		}

	}
}
