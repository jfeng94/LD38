using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTrigger : MonoBehaviour {
	public GameObject target;

	public virtual void OnTriggerEnter2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			Vector3 oldPosition = player.transform.position;
			Vector3 position = oldPosition;
			position.x = target.transform.position.x;
			player.transform.position = position;

			Vector3 displacement = position - oldPosition;
			Camera.main.transform.position += displacement;
		}
	}

	//# public virtual void OnTriggerExit2D(Collider2D other) {
	//# 	Player player = other.gameObject.GetComponent<Player>();
	//# 	if (player != null) {
	//# 		OnPlayerCannotInteract(player);
	//# 	}
	//# }

}
