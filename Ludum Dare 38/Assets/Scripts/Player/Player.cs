using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves as a public interface for all the various components that make up a player.
public class Player : MonoBehaviour {
	public PlayerMovement movement;
	public PlayerStatus   status;
	public PlayerAnimator animator;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InflictDamage(int damage, Vector2 knockbackVector) {}
}
