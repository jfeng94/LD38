using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappingEnemy : Enemy {

	public float flapStrength = 2000f;

	public int flapInterval = 30; // Number of frames between flaps.

	private int numFramesAfterFlap = 100;

	private bool CanFlap() {
		if (numFramesAfterFlap > flapInterval) {
			numFramesAfterFlap = 0;
			return true;
		}
		return false;
	}

	protected override void Update() {
		numFramesAfterFlap++;
		base.Update();
	}
	protected override void MoveTowardsPosition(Vector3 position) {
		
		Vector3 displacement = position - transform.position;



		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		if (rb != null) {
			Vector3 velocity = rb.velocity;
			// Don't move if "close enough". 
			if (Mathf.Abs(displacement.x) < 0.2f) {
				velocity.x = 0;
			} else {
				Debug.Log ("trying to move");
				velocity.x = movementSpeed;
				if (displacement.x < 0f) {
					velocity.x *= -1f;
				}
			}

			rb.velocity = velocity;

			// If below target, flap.
			if (displacement.y > 1) {
				if (CanFlap ()) {
					Debug.Log ("flap");
					//velocity.y = flapStrength;
					rb.AddForce(new Vector2(0, flapStrength));
				}
			}
			


		}
	}
}
