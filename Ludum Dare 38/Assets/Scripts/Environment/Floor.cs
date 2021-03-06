﻿using System.Collections;
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

	public int steppedOnFrame = int.MaxValue;

	//----------------------------------------------------------------------------------------------
	// MOVING PLATFORM VARIABLES
	// If moving is true, the platform will move along the GameObject nodes that we specify in the
	// editor. Note that we should only be using empty game objects as nodes, as the floor will 
	// disable any path nodes it holds onto.  
	//----------------------------------------------------------------------------------------------
	public bool moving = false;
	public List<GameObject> pathNodes = new List<GameObject>();
	public int currentNode = -1;
	public float speed = 3f;

	void Update() {
		UpdateDisappearing();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// DISAPPEARING PLATFORMS
	////////////////////////////////////////////////////////////////////////////////////////////////
	// This is some real spaghetti code.
	private void UpdateDisappearing() {
		if (disappearing) {
			int framesSinceSteppedOn = Time.frameCount - steppedOnFrame;

			if (framesSinceSteppedOn > 0) {
				// Get the sprite renderer to begin with.
				SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
				Color color = new Color(1f, 1f, 1f, 1f);

				//----------------------------------------------------------------------------------
				// Get the number of frames left we have for each stage of the platform's life
				//----------------------------------------------------------------------------------
				int phaseContributions = numFramesStable;
				int stableRemaining = PosOrZero(phaseContributions - framesSinceSteppedOn);

				phaseContributions += numFramesFadeOut;
				int fadeOutRemaining = PosOrZero(phaseContributions - framesSinceSteppedOn);

				phaseContributions += numFramesGone;
				int goneRemaining = PosOrZero(phaseContributions - framesSinceSteppedOn);

				phaseContributions += numFramesFadeIn;
				int fadeInRemaining = PosOrZero(phaseContributions - framesSinceSteppedOn);

				//----------------------------------------------------------------------------------
				// If we've finished all parts of the animation, reset everything.
				//----------------------------------------------------------------------------------
				if (fadeInRemaining <= 0) {
					stableRemaining  = numFramesStable;
					fadeOutRemaining = numFramesFadeOut;
					goneRemaining    = numFramesGone;

					steppedOnFrame = int.MaxValue;

					if (spriteRenderer != null) {
						spriteRenderer.color = Color.white;
					}
				}
				
				//----------------------------------------------------------------------------------
				// Turn collider on or off based on whether fade out has finished.
				//----------------------------------------------------------------------------------
				BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
				if (boxCollider != null) boxCollider.enabled = (fadeOutRemaining > 0);

				//----------------------------------------------------------------------------------
				// Recolor the platform based on how much fade we have.
				//----------------------------------------------------------------------------------
				// If the user has even touched it, turn it blue.
				if (stableRemaining != numFramesStable) color.r = 0.8f;  

				// Depending on the state of the fade values, fade the platform.
				if (fadeOutRemaining == 0)  color.a = (float) (numFramesFadeIn - fadeInRemaining) / (float) numFramesFadeIn;
				else                        color.a = (float) fadeOutRemaining / (float) numFramesFadeOut;

				if (spriteRenderer != null) spriteRenderer.color = color;

				SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
				for (int i = 0; i < spriteRenderers.Length; i++) {
					if (spriteRenderers[i] != null) {
						spriteRenderers[i].color = color;
					}			
				}
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MOVING PLATFORM
	////////////////////////////////////////////////////////////////////////////////////////////////
	private int PosOrZero(int value) {
		value = (value < 0) ? 0 : value; 
		return value;
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COLLIDERS
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void OnCollisionEnter2D(Collision2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();

		if (player != null) {
			if (steppedOnFrame == int.MaxValue) {
				steppedOnFrame = Time.frameCount;
			}
		}
	}
}
