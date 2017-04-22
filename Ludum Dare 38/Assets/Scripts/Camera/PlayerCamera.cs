using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script allows the camera to follow the player around.
public class PlayerCamera : MonoBehaviour {
	public Player player;

	// We'll keep the player's avatar confined to a smaller rectangle
	float  leftLeniency = -5.0f;
	float rightLeniency =  5.0f;
	float    upLeniency =  2.5f;
	float  downLeniency = -3.5f;


	// Use this for initialization
	void Start () {
		
	}
	
	// We're using LateUpdate, because we want to make sure all the potential changes in the scene
	// have been made before we modify the camera.
	void LateUpdate () {
		// TODO: Keep the camera from moving past the y bounds, however we set that up.
		// TODO: Make the camera less sticky. Should allow user to move some before moving.

		// Make sure we're in parity with the player.
		Vector3 displacement = Vector3.zero;

		displacement.x = (player.transform.position.x - transform.position.x);
		displacement.y = (player.transform.position.y - transform.position.y);

		if (displacement.x < 0 && displacement.x < leftLeniency) {
			displacement.x -= leftLeniency;
		}
		else if (displacement.x > 0 && displacement.x > rightLeniency) {
			displacement.x -= rightLeniency;
		}
		else {
			displacement.x = 0;
		}

		if (displacement.y > 0 && displacement.y > upLeniency) {
			displacement.y -= upLeniency;
		}
		else if (displacement.y < 0 && displacement.y < downLeniency) {
			displacement.y -= downLeniency;
		}
		else {
			displacement.y = 0;
		}

		transform.position += displacement;
	}
}
