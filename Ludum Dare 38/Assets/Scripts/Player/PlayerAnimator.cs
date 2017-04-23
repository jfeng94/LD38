using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class helps the player avatar transition through any animations changes the it will undergo.
// Should be pretty tightly linked to PlayerMovement.cs
public class PlayerAnimator : MonoBehaviour {
	public Player player;

	public enum State {
		Undefined,
		Idle,
		Walk,
		Jump,
		Attack1,
	}

	public SpriteRenderer idle;
	public SpriteRenderer walk;
	public SpriteRenderer jump;
	public SpriteRenderer attack1;

	public bool defaultFacingLeft = false;

	public State storedState = State.Undefined;

	// Invincibility blinking
	public Color invincibleTint1; 
	public Color invincibleTint2;
	public int   blinkFrames = 30;

	void Start() {
		SetState(State.Idle);
	}

	void Update() {
		if (player.IsInvincible()) {
			int framesSinceInvincible = player.GetFramesSinceInvincible();
			int numBlinksSince = (int) framesSinceInvincible / blinkFrames;

			if ( (numBlinksSince % 2) == 0) {
				SetColor(invincibleTint1);
			}
			else {
				SetColor(invincibleTint2);
			}
		}
		else {
			SetColor(Color.white);
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// ANIMATION CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////
	// Right now only used by attack1 animation.
	// Not really sure what else this is supposed to do.
	public void EndAnimation(State state) {
		player.EndAttack();
	}	

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COLOR MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetColor(Color color) {
		if (idle    != null)    idle.color = color;
		if (walk    != null)    walk.color = color;
		if (jump    != null)    jump.color = color;
		if (attack1 != null) attack1.color = color;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// FACING MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void TurnLeft() {
		//# if (idle    != null)    idle.flipX = defaultFacingLeft;
		//# if (walk    != null)    walk.flipX = defaultFacingLeft;
		//# if (jump    != null)    jump.flipX = defaultFacingLeft;
		//# if (attack1 != null) attack1.flipX = defaultFacingLeft;
		if (defaultFacingLeft) {
			transform.localScale = new Vector3(-1, 1, 1);
		}
		else {
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

	public void TurnRight() {
		//# if (idle    != null)    idle.flipX = ! defaultFacingLeft;
		//# if (walk    != null)    walk.flipX = ! defaultFacingLeft;
		//# if (jump    != null)    jump.flipX = ! defaultFacingLeft;
		//# if (attack1 != null) attack1.flipX = ! defaultFacingLeft;
		if (defaultFacingLeft) {
			transform.localScale = new Vector3(1, 1, 1);
		}
		else {
			transform.localScale = new Vector3(-1, 1, 1);
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
			idle.gameObject.SetActive(false);
			walk.gameObject.SetActive(false);
			jump.gameObject.SetActive(false);

			attack1.gameObject.SetActive(false);

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

				case State.Attack1:
					attack1.gameObject.SetActive(true);
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
