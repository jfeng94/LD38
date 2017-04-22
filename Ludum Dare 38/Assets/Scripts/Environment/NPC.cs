using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable {
	public GameObject textBubble;

	public override void Start() {
		base.Start();

		if (textBubble != null) {
			textBubble.SetActive(false);
		}
	}

	public override void Interact() {
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
