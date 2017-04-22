using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script allows the camera to follow the player around.
public class PlayerCamera : MonoBehaviour {
	public Player player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Make sure we're in parity with the player.
		Vector3 position = transform.position;
		position.x = player.transform.position.x;
		position.y = player.transform.position.y;
		transform.position = position;
	}
}
