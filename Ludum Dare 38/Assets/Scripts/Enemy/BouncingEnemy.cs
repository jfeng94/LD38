using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEnemy : Enemy {

	public float jumpingVelocity = 20f;

	//# private bool grounded = true;
	//# public override void Update() {
	//# 	UpdateGroundedness();
	//# 	base.Update();
	//# }

	//# private void UpdateGroundedness() {
	//# 	LayerMask groundLayer = LayerMask.NameToLayer ("Ground");
	//# 	int layerMask = 1 << groundLayer.value;

	//# 	// Check for groundedness.
	//# 	Collider2D collider = Physics2D.OverlapBox (new Vector2 (transform.position.x, 
	//# 	                                                         // Note we want to create the box
	//# 	                                                         // at the bottom of the enemy, 
	//# 	                                                         // not the enemy's center!
	//# 	                                                         transform.position.y - 0.5f),
	//# 	                                            new Vector2 (0.6f, 0.2f), 0f, layerMask);
		
	//# 	if (collider != null) {
	//# 		grounded = true;
	//# 		Debug.Log ("grounded on " + collider.gameObject.name, collider.gameObject);
	//# 	} else {
	//# 		grounded = false;
	//# 	}
	//# }
	
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

			if (grounded) {
				Debug.Log ("jumping");
				velocity.y = jumpingVelocity;
			} 
			rb.velocity = velocity;



		}
	}
}
