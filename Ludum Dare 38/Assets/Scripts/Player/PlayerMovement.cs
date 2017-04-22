using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles taking input from the user and translating that into motion for the avatar.
public class PlayerMovement : MonoBehaviour {
	private Rigidbody2D rb;
	private float jumpingVelocity = 20f;

	private bool grounded = true;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		//------------------------------------------------------------------------------------------
		// ON KEY PRESS DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.Space))      Jump();
		if (Input.GetKeyDown(KeyCode.LeftArrow))  TurnLeft();
		if (Input.GetKeyDown(KeyCode.RightArrow)) TurnRight();

		//------------------------------------------------------------------------------------------
		// WHILE HOLDING KEY DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKey(KeyCode.LeftArrow))  MoveLeft();
		if (Input.GetKey(KeyCode.RightArrow)) MoveRight();
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MOVEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Jump() {
		// Only allow jumping if we're grounded
		if (grounded) {
			Vector2 initialVelocity = rb.velocity;
			initialVelocity += new Vector2(0, jumpingVelocity);
			rb.velocity = initialVelocity;

			grounded = false;
			
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
	 	Debug.Log("OnCollisionEnter2D");

	 	Floor floor = collision.gameObject.GetComponent<Floor>();
	 	if (floor != null) {
	 		grounded = true;
	 	}
	}
}
