using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

	public GameObject indicator;

	void OnTriggerEnter2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			indicator.SetActive(true);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			indicator.SetActive(false);
		}
	}	
}
