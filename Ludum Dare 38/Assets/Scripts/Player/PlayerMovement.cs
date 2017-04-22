using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles taking input from the user and translating that into motion for the avatar.
public class PlayerMovement : MonoBehaviour {
	private Rigidbody2D rb;
	private float jumpingVelocity = 20f;

	private bool grounded = true;

	private Interactable currentInteractable = null;

	// Direction from center of player game object to feet.
	private Vector3 directionTowardsFeet = Vector3.down;

	// Distance from center of player game object to feet.
	private float distanceToFeet = 0.8f;

	private float movementSpeed = 5f;
	private float aerialDrift   = 0.5f;

	public GameObject colliderVisualizer;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
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





		//# RaycastHit2D hit = Physics2D.Raycast(transform.position, directionTowardsFeet,
		//#                                      distanceToFeet, layerMask);
		//# if (hit.collider != null) {
			
		//# 	// Disallow boost jumping
		//# 	if (rb.velocity.y > 0) {
		//# 		grounded = false;
		//# 	}
		//# 	else {
		//# 		grounded = true;
		//# 	}
		//# }
		//# else {
		//# 	grounded = false;
		//# }


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
		if (grounded) {
			velocity.x = -1f * movementSpeed;
		}
		else {
			velocity.x += -1f * aerialDrift;

			if (velocity.x < -1f * movementSpeed) {
				velocity.x = -1f * movementSpeed;
			}
		}
		rb.velocity = velocity;
	}

	public void MoveRight() {
		Vector3 velocity = rb.velocity;
		if (grounded) {
			velocity.x = movementSpeed;
		}
		else {
			velocity.x += aerialDrift;

			if (velocity.x > movementSpeed) {
				velocity.x = movementSpeed;
			}
		}
		rb.velocity = velocity;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////
	//// SPRITE ANIMATION
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void TurnLeft()  {}
	public void TurnRight() {}
	public void Crouch()    {}


	void OnCollisionEnter2D(Collision2D collision)
	{
		//# ContactPoint2D[] contacts = collision.contacts;
		//# for (int i = 0; i < contacts.Length; i++)  {
		//# 	Vector2 normal = contacts[i].normal;
		//# 	if (normal == Vector2.up) {
		//# 		Debug.Log("OnCollisionEnter2D");
		//# 		grounded = true;
		//# 		break;
		//# 	}
		//# }

	 	Interactable interactable = collision.gameObject.GetComponent<Interactable>();
	 	if (interactable != null) {
	 		currentInteractable = interactable;
	 	}
	}

	//# void OnCollisionStay2D(Collision2D collision)
	//# {
	//# 	ContactPoint2D[] contacts = collision.contacts;
	//# 	for (int i = 0; i < contacts.Length; i++)  {
	//# 		Vector2 normal = contacts[i].normal;
	//# 		if (normal == Vector2.up) {
	//# 			Debug.Log("OnCollisionStay2D");
	//# 			grounded = true;
	//# 			break;
	//# 		}
	//# 	}
	//# }

	void OnCollisionExit2D(Collision2D collision)
	{
	 	Interactable interactable = collision.gameObject.GetComponent<Interactable>();
	 	if (interactable == currentInteractable) {
	 		currentInteractable = null;
	 	}
	}

}
