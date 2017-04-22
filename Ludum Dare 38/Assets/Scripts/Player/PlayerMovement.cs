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
	private Vector2 directionTowardsFeet = Vector2.down;

	// Distance from center of player game object to feet.
	private float distanceToFeet = 0.55f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		LayerMask groundLayer = LayerMask.NameToLayer("Ground");
		int layerMask = 1 << groundLayer.value;

		// Check for groundedness
		RaycastHit2D hit = Physics2D.Raycast(transform.position, directionTowardsFeet,
		                                     distanceToFeet, layerMask);
		if (hit.collider != null) {
			
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


		//------------------------------------------------------------------------------------------
		// ON KEY PRESS DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.UpArrow))    Jump();
		if (Input.GetKeyDown(KeyCode.Space))      Jump();
		if (Input.GetKeyDown(KeyCode.LeftArrow))  TurnLeft();
		if (Input.GetKeyDown(KeyCode.RightArrow)) TurnRight();
		if (Input.GetKeyDown(KeyCode.F))          Interact();

		//------------------------------------------------------------------------------------------
		// WHILE HOLDING KEY DOWN
		//------------------------------------------------------------------------------------------
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
			Debug.Log("Jump");

			Vector2 initialVelocity = rb.velocity;
			//# initialVelocity += new Vector2(0, jumpingVelocity);
			initialVelocity.y = jumpingVelocity;
			rb.velocity = initialVelocity;			
			// TODO: Start animating jump cycle
		}
	}

	public void MoveLeft() {
		transform.position += (Vector3.left * 0.1f); 
	}

	public void MoveRight() {
		transform.position += (Vector3.right * 0.1f); 
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
