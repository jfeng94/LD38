using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IHittable {
	//----------------------------------------------------------------------------------------------
	// Resources
	//----------------------------------------------------------------------------------------------
	public int maxHealth = 5;
	public int maxMana   = 4;

	public int health = 5;
	public int mana   = 3;

	//----------------------------------------------------------------------------------------------
	// Groundedness
	//----------------------------------------------------------------------------------------------
	// Whether the character is touching the ground
	protected bool grounded = true;
	// Direction from center of player game object to feet.
	protected Vector3 directionTowardsFeet = Vector3.down;
	// Distance from center of player game object to feet.
	protected float distanceToFeet = 0.8f;

	public GameObject colliderVisualizer;

	//----------------------------------------------------------------------------------------------
	// Movement
	//----------------------------------------------------------------------------------------------
	// How fast the player can possibly move
	protected float maxSpeed = 5f;

	// How much the user accelerates per frame while holding a direction
	protected float movementSpeed = 1f;

	// How much the user can affect their aerial trajectory.
	protected float aerialDrift = 0.5f;

	//----------------------------------------------------------------------------------------------
	// Physics
	//----------------------------------------------------------------------------------------------
	protected Rigidbody2D rb;

	// Use this for initialization
	protected virtual void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	protected virtual void Update() {
		CheckGrounded();
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// GROUNDEDNESS
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected virtual int GetGroundedLayerMask() {
		LayerMask groundLayer = LayerMask.NameToLayer("Ground");
		int layerMask = (1 << groundLayer.value);
		return layerMask;
	} 

	protected void CheckGrounded() {
		// We can only be grounded if the y velocity isn't positive (going up);
		if (rb.velocity.y <= 0) {
			// Get all the box-colliders on this object
			BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
			for (int i = 0; i < colliders.Length; i++) {

				// Don't use trigger colliders to determine groundedness	
				if ( ! colliders[i].isTrigger) {

					// If that collider touches a layer we consider to be ground
					if (colliders[i].IsTouchingLayers(GetGroundedLayerMask())) {
						grounded = true;
						return;
					}
				}
			}

			// Get all the box-colliders on childed to this object
			// Note this does not retrieve any colliders that are on inactive objects.
			colliders = GetComponentsInChildren<BoxCollider2D>();
			for (int i = 0; i < colliders.Length; i++) {

				// Don't use trigger colliders to determine groundedness	
				if ( ! colliders[i].isTrigger) {

					// If that collider touches a layer we consider to be ground
					if (colliders[i].IsTouchingLayers(GetGroundedLayerMask())) {
						grounded = true;
						return;
					}
				}
			}

		}
		
		grounded = false;
		//# // Check for groundedness
		//# Vector3 footPosition = transform.position + (directionTowardsFeet * distanceToFeet);
		//# Collider2D collider = Physics2D.OverlapBox(new Vector2(footPosition.x, footPosition.y),
		//#                                            new Vector2(0.6f, 0.2f), 0f, layerMask);

		//# colliderVisualizer.transform.position = footPosition;
		//# colliderVisualizer.transform.localScale = new Vector3(0.6f, 0.2f, 1f);

		//# if (collider != null && rb.velocity.y <= 0) {
		//# 	grounded = true;
		//# }
		//# else grounded = false;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// HEALTH
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void GainHealth(int amount) {
		health += amount;
		EnsureHealthIsWithinBounds();
	}

	public void DealDamage(int amount) {
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

	public void Heal(int heal) {
		GainHealth(heal);
	}

	public virtual void InflictDamage(int damage, Vector2 direction, float strength) {
		DealDamage(damage);

		if (rb != null) {
			rb.AddForce(direction * strength, ForceMode2D.Impulse);
		}
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
