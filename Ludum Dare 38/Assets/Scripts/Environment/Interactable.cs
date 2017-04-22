using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	public GameObject indicator;

	public virtual void Start() {
		indicator.SetActive(false);
	}

	// What happens when the user presses the interaction key
	public virtual void Interact() {}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// INTERACTION ENABLE/DISABLE
	////////////////////////////////////////////////////////////////////////////////////////////////
	// The player enters the interaction collider
	public virtual void OnPlayerCanInteract(Player player) {
		indicator.SetActive(true);
	}

	// The player exits the interaction collider
	public virtual void OnPlayerCannotInteract(Player player) {
		indicator.SetActive(false);
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// PHYSICS
	////////////////////////////////////////////////////////////////////////////////////////////////
	public virtual void OnTriggerEnter2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			OnPlayerCanInteract(player);
		}
	}

	public virtual void OnTriggerExit2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			OnPlayerCannotInteract(player);
		}
	}	
}
