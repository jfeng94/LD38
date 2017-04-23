using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Hitbox {
	protected override bool IsValidTarget(IHittable hitObject) {
		// Note the difference between the explicit cast operator and the "as" keyword:
		// http://stackoverflow.com/questions/132445/direct-casting-vs-as-operator
		if ( (hitObject as Player) != null) {
			return true;
		}
		return false;
	}

	public override void OnCollisionEnter2D(Collision2D collision) {
		//# if (collision.gameObject.name != "Floor") {
		//# 	Debug.LogError("EnemyHitbox::OnCollisionEnter2D: " + collision.gameObject.name, collision.rigidbody);
		//# }
		base.OnCollisionEnter2D(collision);
	}

	public override void OnCollisionStay2D(Collision2D collision) {
		//# if (collision.gameObject.name != "Floor") {
		//# 	Debug.Log("EnemyHitbox::OnCollisionStay2D: " + collision.gameObject.name, collision.rigidbody);
		//# }
		base.OnCollisionStay2D(collision);
	}
}
