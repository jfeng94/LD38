using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : Hitbox {
	// List of objects we've hit while this box was active
	private List<GameObject> hitObjects = new List<GameObject>();

	protected override bool IsValidTarget(IHittable hitObject) {
		// Note the difference between the explicit cast operator and the "as" keyword:
		// http://stackoverflow.com/questions/132445/direct-casting-vs-as-operator
		if ( (hitObject as Enemy) != null) {
			return true;
		}

		return false;
	}

	// As it is right now, we're simply enabling this hitbox every time we do an attack animation.
	// So for now, let's set things up correctly here...
	// TBH this is probably horrible, hacky design
	public void OnEnable() {
		hitObjects.Clear();
	}

	public override void OnCollisionEnter2D(Collision2D collision) {
		if ( ! hitObjects.Contains(collision.gameObject) ) {
			base.OnCollisionEnter2D(collision);
			hitObjects.Add(collision.gameObject);
		}
	}

	public override void OnCollisionStay2D(Collision2D collision) {
		// Do nothing;
	}
}
