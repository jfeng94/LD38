using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable {
	public GameObject textBubble;

	public override void Start() {
		base.Start();
		textBubble.SetActive(false);
	}

	public override void Interact() {
		indicator.SetActive(false);

		textBubble.SetActive(true);

		Invoke("HideTextBubble", 1f);
	}	

	private void HideTextBubble() {
		textBubble.SetActive(false);
	}
}
