using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MovingObject, IInteractable {
	public GameObject textBubble;
	public GameObject indicator;
	public bool canInteract = true;

	protected override void Start() {
		base.Start();

		if (indicator != null) {
			indicator.SetActive(false);
		}

		if (textBubble != null) {
			textBubble.SetActive(false);
		}


	}

	public bool CanInteract() {
		return canInteract;
	}

	public void StartSignaling() {
		if (indicator != null) {
			indicator.SetActive(true);
		}
	}

	public void StopSignaling() {
		if (indicator != null) {
			indicator.SetActive(false);
		}
	}

	public void Interact() {
		if (indicator != null) {
			indicator.SetActive(false);
		}

		if (indicator != null) {
			textBubble.SetActive(true);
		}

		Invoke("HideTextBubble", 1f);
	}	

	private void HideTextBubble() {
		if (textBubble != null) {
			textBubble.SetActive(false);
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// GROUNDEDNESS OVERRIDE
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected override void CheckGrounded() {
		// We can only be grounded if the y velocity isn't positive (going up);
		if (rb.velocity.y <= 0) {
			// Get all the box-colliders on this object
			BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
			for (int i = 0; i < colliders.Length; i++) {

				// *DO* use trigger colliders to determine groundedness	
				// If that collider touches a layer we consider to be ground
				if (colliders[i].IsTouchingLayers(GetGroundedLayerMask())) {
					grounded = true;
					return;
				}
			}
			// Get all the box-colliders on childed to this object
			// Note this does not retrieve any colliders that are on inactive objects.
			colliders = GetComponentsInChildren<BoxCollider2D>();
			for (int i = 0; i < colliders.Length; i++) {

				// *DO* use trigger colliders to determine groundedness	
				// If that collider touches a layer we consider to be ground
				if (colliders[i].IsTouchingLayers(GetGroundedLayerMask())) {
					grounded = true;
					return;
				}
			}
		}
		grounded = false;
	}

}
