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

}
