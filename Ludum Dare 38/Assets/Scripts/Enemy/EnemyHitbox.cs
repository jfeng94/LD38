using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour {
	public int   damage = 1;
	public float knockbackStrength = 20f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();

		if (player != null) {
			ContactPoint2D[] contacts = collision.contacts;

			// We'll just use the first contact point to determine where to send the player flying
			if (contacts.Length >= 1) {
				Vector2 direction = contacts[0].point - new Vector2(transform.position.x, transform.position.y);

				// Scalle down the amount of vertical knockback we get
				direction.y *= 0.5f;

				direction.Normalize();
				player.InflictDamage(damage, direction, knockbackStrength);
			}
		}
	}
}
