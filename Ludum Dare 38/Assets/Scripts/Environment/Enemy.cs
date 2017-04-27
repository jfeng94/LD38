using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Combatant {
	public GameObject indicator;

	private Player aggroTarget = null;

	public bool isAlive = false;

	// Where the enemy spawns at
	public Vector3 spawnPosition = Vector3.zero;

	// How far away from the enemy's spawn is it willing to go.
	public float leashLength = 20f;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		spawnPosition = transform.position;
		isAlive = true;
		Respawn();
	}
	
	// Update is called once per frame
	protected override void Update () {
		if (!isDead) {
			base.Update();

			// Get distance from spawn
			float distanceFromSpawn = (transform.position - spawnPosition).magnitude;

			// Follow the player if they are holding aggro
			if (aggroTarget != null) {
				// Otherwise, move towards the player
				MoveTowardsPosition(aggroTarget.transform.position);
			}

			// If there is no aggro, move towards the spawn point
			if (aggroTarget == null) {
				MoveTowardsPosition(spawnPosition);
			}			
		}
	}

	protected void Respawn() {
		Collider2D collider = GetComponent<Collider2D>();
		if (collider != null) {
			collider.enabled = true;
		}

		Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
		for (int i = 0; i < colliders.Length; i++) {
			colliders[i].enabled = true;
		}
		

		if (rb != null) {
			rb.simulated = true;
		}

		transform.position = spawnPosition;
		transform.rotation = Quaternion.identity;
	}

	protected override void DieAHorribleDeath() {
		animator.SetState(MovingObject.AnimationState.Dead);
		isAlive = false;

		Collider2D collider = GetComponent<Collider2D>();
		if (collider != null) {
			collider.enabled = false;
		}

		if (rb != null) {
			rb.simulated = false;
		}

		Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
		for (int i = 0; i < colliders.Length; i++) {
			colliders[i].enabled = false;
		}

		rb.velocity = Vector3.zero;
		rb.angularVelocity = 0;
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
			Debug.Log("Enemy gain aggro", player);
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
}
