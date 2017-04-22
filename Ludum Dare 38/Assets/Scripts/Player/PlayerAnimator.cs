using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: This class will help the sprite renderer transition through any animations changes the 
////     player avatar will undergo. Should be pretty tightly linked to PlayerMovement.cs
public class PlayerAnimator : MonoBehaviour {
	public SpriteRenderer renderer;

	public bool defaultFacingLeft = false;

	public void TurnLeft() {
		if (renderer != null) {
			if (defaultFacingLeft) {
				renderer.flipX = true;
			}
		}
	}

	public void TurnRight() {
		if (renderer != null) {
			if (defaultFacingLeft) {
				renderer.flipX = false;
			}
		}
	}

	public void Crouch() {

	}
}
