using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: This class will help the sprite renderer transition through any animations changes the 
////     player avatar will undergo. Should be pretty tightly linked to PlayerMovement.cs
public class PlayerAnimator : MonoBehaviour {
	public enum State {
		Undefined,
		Idle,
		Walk,
		Jump,
		Stun,
	}

	public SpriteRenderer idle;
	public SpriteRenderer walk;
	public SpriteRenderer jump;
	public SpriteRenderer stun;

	public bool defaultFacingLeft = false;

	public State storedState = State.Undefined;

	void Start() {
		SetState(State.Idle);
	}

	public void TurnLeft() {
		if (idle != null) idle.flipX = defaultFacingLeft;
		if (walk != null) walk.flipX = defaultFacingLeft;
		if (jump != null) jump.flipX = defaultFacingLeft;
		if (stun != null) stun.flipX = defaultFacingLeft;
	}

	public void TurnRight() {
		if (idle != null) idle.flipX = ! defaultFacingLeft;
		if (walk != null) walk.flipX = ! defaultFacingLeft;
		if (jump != null) jump.flipX = ! defaultFacingLeft;
		if (stun != null) stun.flipX = ! defaultFacingLeft;
	}

	public void SetState(State state) {
		// Only allow the state to "change" if there truly is a change.
		// Setting an animated game object disabled and then enabled will probably cause the
		// animation to restart.
		if (state != storedState) {
			// First set everything to be disabled.
			idle.gameObject.SetActive(false);
			walk.gameObject.SetActive(false);
			jump.gameObject.SetActive(false);
			stun.gameObject.SetActive(false);

			// Re-enable based on state
			switch (state) {
				case State.Idle:
					idle.gameObject.SetActive(true);
					break;

				case State.Walk:
					walk.gameObject.SetActive(true);
					break;

				case State.Jump:
					jump.gameObject.SetActive(true);
					break;

				case State.Stun:
					stun.gameObject.SetActive(true);
					break;

				default:
					// Default to idle, but keep the state undefined, so we can properly switch
					// into idle later.
					state = State.Undefined;
					idle.gameObject.SetActive(true);
					break;
			}

			storedState = state;
		}
	}

}
