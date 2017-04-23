using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves as a public interface for all the various components that make up a player.
public class Player : MonoBehaviour {
	public PlayerStatus   status;
	public PlayerAnimator animator;

	private Rigidbody2D rb;
	private float jumpingVelocity = 20f;

	private bool grounded = true;

	private Interactable currentInteractable = null;

	// Direction from center of player game object to feet.
	private Vector3 directionTowardsFeet = Vector3.down;

	// Distance from center of player game object to feet.
	private float distanceToFeet = 0.8f;

	private float maxSpeed      = 5f;
	private float movementSpeed = 0.5f;
	private float aerialDrift   = 0.5f;

	public GameObject colliderVisualizer;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateGroundedness();

		//------------------------------------------------------------------------------------------
		// ON KEY PRESS DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.LeftArrow))  TurnLeft();
		if (Input.GetKeyDown(KeyCode.RightArrow)) TurnRight();
		if (Input.GetKeyDown(KeyCode.F))          Interact();

		//------------------------------------------------------------------------------------------
		// WHILE HOLDING KEY DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKey(KeyCode.Space))      Jump();
		if (Input.GetKey(KeyCode.LeftArrow))  MoveLeft();
		if (Input.GetKey(KeyCode.RightArrow)) MoveRight();

	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// GROUNDEDNESS
	////////////////////////////////////////////////////////////////////////////////////////////////
	private void UpdateGroundedness() {
		LayerMask groundLayer = LayerMask.NameToLayer("Ground");
		int layerMask = 1 << groundLayer.value;

		// Check for groundedness
		Vector3 footPosition = transform.position + (directionTowardsFeet * distanceToFeet);
		Collider2D collider = Physics2D.OverlapBox(new Vector2(footPosition.x, footPosition.y),
		                                           new Vector2(0.6f, 0.2f), 0f, layerMask);

		colliderVisualizer.transform.position = footPosition;
		colliderVisualizer.transform.localScale = new Vector3(0.6f, 0.2f, 1f);


		if (collider != null) {
			// Disallow boost jumping
			if (rb.velocity.y > 0) {
				grounded = false;
			}
			else {
				grounded = true;
			}
		}
		else {
			grounded = false;
		}

	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COMBAT AND DAMAGE
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Heal(int heal) {
		status.GainHealth(heal);
	}

	public void InflictDamage(int damage, Vector2 direction, float strength) {
		status.SpendHealth(damage);

		Debug.Log("InflictDamage force added: " + direction + " - " + strength);
		rb.AddForce(direction * strength, ForceMode2D.Impulse);
		// TODO -- Check for game overs?
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// INTERACTION
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Interact() {
		if (currentInteractable != null) {
			currentInteractable.Interact();
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MOVEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Jump() {
		// Only allow jumping if we're grounded
		if (grounded) {
			Vector2 initialVelocity = rb.velocity;
			//# initialVelocity += new Vector2(0, jumpingVelocity);
			initialVelocity.y = jumpingVelocity;
			rb.velocity = initialVelocity;			
			// TODO: Start animating jump cycle
		}
	}

	public void MoveLeft() {
		Vector3 velocity = rb.velocity;
		if (grounded) velocity.x += -1f * movementSpeed;
		else          velocity.x += -1f * aerialDrift;
		
		if (velocity.x < -1f * maxSpeed) velocity.x = -1f * maxSpeed;

		Debug.Log("Velocity " + velocity);

		rb.velocity = velocity;
	}

	public void MoveRight() {
		Vector3 velocity = rb.velocity;
		if (grounded) velocity.x += movementSpeed;
		else          velocity.x += aerialDrift;
		
		if (velocity.x > maxSpeed) velocity.x = maxSpeed;

		Debug.Log("Velocity " + velocity);

		rb.velocity = velocity;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// SPRITE ANIMATION
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void TurnLeft()  {
		animator.TurnLeft();
	}
	public void TurnRight() {
		animator.TurnRight();
	}
	public void Crouch()    {
		animator.Crouch();
	}

	void OnTriggerEnter2D(Collider2D collider) {
	 	Interactable interactable = collider.gameObject.GetComponent<Interactable>();
	 	if (interactable != null) {
	 		Debug.Log("currentInteractable set to " + interactable, interactable);
	 		currentInteractable = interactable;
	 	}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
	 	Interactable interactable = collider.gameObject.GetComponent<Interactable>();
	 	if (interactable != null && interactable == currentInteractable) {
	 		Debug.Log("currentInteractable unset from  " + interactable, interactable);
	 		currentInteractable = null;
	 	}
	}

}
