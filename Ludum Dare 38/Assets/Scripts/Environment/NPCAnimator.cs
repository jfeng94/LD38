using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class helps the player avatar transition through any animations changes the it will undergo.
// Should be pretty tightly linked to PlayerMovement.cs
public class NPCAnimator : MonoBehaviour {
	public NPC npc;

	public enum State {
		Undefined,
		Idle,
		Walk,
	}

	public SpriteRenderer idle;
	public SpriteRenderer walk;

	public bool defaultFacingLeft = false;

	public State storedState = State.Undefined;


	void Start() {
		SetState(State.Idle);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// ANIMATION CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////
	// Not really sure what this is supposed to do.
	public void EndAnimation(State state) {
	}	

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COLOR MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetColor(Color color) {
		if (idle    != null)    idle.color = color;
		if (walk    != null)    walk.color = color;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// FACING MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void TurnLeft() {
		if (defaultFacingLeft) {
			transform.localScale = new Vector3(1, 1, 1);
		}
		else {
			transform.localScale = new Vector3(-1, 1, 1);
		}
	}

	public void TurnRight() {
		if (defaultFacingLeft) {
			transform.localScale = new Vector3(-1, 1, 1);
		}
		else {
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// STATE MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetState(State state) {
		// Only allow the state to "change" if there truly is a change.
		// Setting an animated game object disabled and then enabled will probably cause the
		// animation to restart.
		if (state != storedState) {
			// First set everything to be disabled.
			if (idle != null) idle.gameObject.SetActive(false);
			if (walk != null) walk.gameObject.SetActive(false);

			// Re-enable based on state
			switch (state) {
				case State.Idle:
					if (idle != null) idle.gameObject.SetActive(true);
					break;

				case State.Walk:
					if (walk != null) walk.gameObject.SetActive(true);
					break;

				default:
					// Default to idle, but keep the state undefined, so we can properly switch
					// into idle later.
					state = State.Undefined;
					if (idle != null) idle.gameObject.SetActive(true);
					break;
			}

			storedState = state;
		}
	}

}
