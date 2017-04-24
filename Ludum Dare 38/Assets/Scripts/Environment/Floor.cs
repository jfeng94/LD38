using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {
	//----------------------------------------------------------------------------------------------
	// DISAPPEARING PLATFORM VARIABLES
	// If disappearing is true, when the player lands on the platform
	// - The floor will begin to fade after numFramesStable frames.
	// - Then, after numFramesFadeOut frames, the platform disappears completely.
	// - Next, after numFramesGone frames, the platform begins to fade back in.
	// - Finally, after numFramesFadeIn frames, the platform becomes solid again.
	//
	//// TODO:
	//// If stopWhileGone is true and this platform is a moving platform, it
	//// won't move while the platform is gone. If resetAfterReappearing is true, when the platform
	//// comes back, it will be configured exactly how it was when it first started.
	//----------------------------------------------------------------------------------------------
	public bool disappearing = false;
	public int  numFramesStable  = 30;
	public int  numFramesFadeOut = 120;
	public int  numFramesGone    = 180;
	public int  numFramesFadeIn  = 120;

	public int steppedOnFrame  = int.MaxValue;

	//----------------------------------------------------------------------------------------------
	// MOVING PLATFORM VARIABLES
	//----------------------------------------------------------------------------------------------
	public bool moving = false;

	void Update() {
		UpdateDisappearing();
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// DISAPPEARING PLATFORMS
	////////////////////////////////////////////////////////////////////////////////////////////////
	// This is some real spaghetti code.
	private void UpdateDisappearing() {
		if (disappearing) {
			int stableRemaining  = numFramesStable;
			int fadeOutRemaining = numFramesFadeOut;
			int goneRemaining    = numFramesGone;
			int fadeInRemaining  = numFramesFadeIn;

			int framesSinceSteppedOn = Time.frameCount - steppedOnFrame;
			if (framesSinceSteppedOn > 0) {

				stableRemaining = numFramesStable - framesSinceSteppedOn;
				stableRemaining = (stableRemaining < 0) ? 0 : stableRemaining;

				fadeOutRemaining = (numFramesStable + numFramesFadeOut) - framesSinceSteppedOn;
				fadeOutRemaining = (fadeOutRemaining < 0) ? 0 : fadeOutRemaining;

				goneRemaining = (numFramesStable + numFramesFadeOut + numFramesGone) - framesSinceSteppedOn;
				goneRemaining = (goneRemaining < 0) ? 0 : goneRemaining;

				fadeInRemaining = (numFramesStable + numFramesFadeOut + numFramesGone + numFramesFadeIn) - framesSinceSteppedOn;
				fadeInRemaining = (fadeInRemaining < 0) ? 0 : fadeInRemaining;
			}

			if (fadeInRemaining <= 0) {
				stableRemaining  = numFramesStable;
				fadeOutRemaining = numFramesFadeOut;
				goneRemaining    = numFramesGone;	
			}

			BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
			if (boxCollider != null) {
				boxCollider.enabled = (fadeOutRemaining > 0);
			}

			float transparency = 1f;

			if (fadeOutRemaining == 0) {
				transparency = (float) (numFramesFadeIn - fadeInRemaining) / (float) numFramesFadeIn;
			}
			else {
				transparency = (float) fadeOutRemaining / (float) numFramesFadeOut;
			}
			Color color = new Color(1f, 1f, 1f, transparency);

			SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer != null) {
				spriteRenderer.color = color;
			}

			SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
			for (int i = 0; i < spriteRenderers.Length; i++) {
				if (spriteRenderers[i] != null) {
					spriteRenderers[i].color = color;
				}			
			}
		}
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();

		if (player != null) {
			steppedOnFrame = Time.frameCount;
		}
	}
}
