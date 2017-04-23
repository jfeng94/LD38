using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {
	public int   damage = 1;
	public float knockbackStrength = 20f;

	protected virtual bool IsValidTarget(IHittable hittable) {
		return false;
	}

	public void InflictDamage(Collision2D collision) {
		IHittable hitObject = collision.gameObject.GetComponent<IHittable>();

		if (hitObject != null && IsValidTarget(hitObject)) {
			ContactPoint2D[] contacts = collision.contacts;

			// We'll just use the first contact point to determine where to send the player flying
			if (contacts.Length >= 1) {
				Vector2 direction = contacts[0].point - new Vector2(transform.position.x, transform.position.y);

				// Scalle down the amount of vertical knockback we get
				direction.y *= 0.5f;

				direction.Normalize();
				hitObject.InflictDamage(damage, direction, knockbackStrength);
			}
		}
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		InflictDamage(collision);
	}

	public void OnCollisionStay2D(Collision2D collision) {
		InflictDamage(collision);
	}

}

