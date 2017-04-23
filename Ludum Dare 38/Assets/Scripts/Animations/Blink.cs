using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour {
	public SpriteRenderer spriteRenderer;
	public Color tint1;
	public Color tint2;
	public float blinkSpeed = 0.2f;

	// Use this for initialization
	void OnEnable () {
		// Cancel any pending invoked functions
		CancelInvoke();

		// Tint sprite to the first specified color.
		TintSprite1();
	}
		
	public void TintSprite1() {
		spriteRenderer.color = tint1;

		Invoke("TintSprite2", blinkSpeed);
	}

	public void TintSprite2() {
		spriteRenderer.color = tint2;

		Invoke("TintSprite1", blinkSpeed);
	}	
}
