using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEnemy : Enemy {

	protected override void MoveTowardsPosition(Vector3 position) {
		Vector3 displacement = position - transform.position;

		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		if (rb != null) {
			Vector3 velocity = rb.velocity;
			// Don't move if "close enough". 
			if (Mathf.Abs(displacement.x) < 0.2f) {
				velocity.x = 0;
			} else {
				velocity.x += Mathf.Sign(displacement.x) * movementSpeed;

				if (Mathf.Abs(velocity.x) > maxSpeed && Mathf.Sign(velocity.x) == Mathf.Sign(displacement.x)) {
					velocity.x = Mathf.Sign(displacement.x) * maxSpeed;
				}
			}

			if (grounded) {
				velocity.y = jumpingVelocity;
			} 
			rb.velocity = velocity;



		}
	}
}
