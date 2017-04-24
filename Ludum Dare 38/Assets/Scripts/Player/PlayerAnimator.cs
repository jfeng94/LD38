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
		Dash,
		DashUp,
		GroundAttack,
		AerialAttack,
	}

	public SpriteRenderer idle;
	public SpriteRenderer walk;
	public SpriteRenderer jump;
	public SpriteRenderer dash;
	public SpriteRenderer dashUp;
	public SpriteRenderer groundAttack;
	public SpriteRenderer aerialAttack;

	public bool defaultFacingLeft = false;

	public State storedState = State.Undefined;

	// Invincibility 1blinking
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
	// Right now only used by groundAttack animation.
	// Not really sure what else this is supposed to do.
	public void EndAnimation(State state) {
		player.EndAttack();
	}	

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COLOR MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetColor(Color color) {
		if (idle         != null)         idle.color = color;
		if (walk         != null)         walk.color = color;
		if (jump         != null)         jump.color = color;
		if (dash         != null)         dash.color = color;
		if (dashUp       != null)       dashUp.color = color;
		if (groundAttack != null) groundAttack.color = color;
		if (aerialAttack != null) aerialAttack.color = color;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// FACING MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void TurnLeft() {
		if (defaultFacingLeft) {
			transform.localScale = new Vector3(-1, 1, 1);
		}
		else {
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

	public void TurnRight() {
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
			if (idle         != null)         idle.gameObject.SetActive(false);
			if (walk         != null)         walk.gameObject.SetActive(false);
			if (jump         != null)         jump.gameObject.SetActive(false);
			if (dash         != null)         dash.gameObject.SetActive(false);
			if (dashUp       != null)       dashUp.gameObject.SetActive(false);
			if (groundAttack != null) groundAttack.gameObject.SetActive(false);
			if (aerialAttack != null) aerialAttack.gameObject.SetActive(false);

			// Re-enable based on state
			switch (state) {
				case State.Idle:
					if (idle != null) idle.gameObject.SetActive(true);
					break;

				case State.Walk:
					if (walk != null) walk.gameObject.SetActive(true);
					break;

				case State.Jump:
					if (jump != null) jump.gameObject.SetActive(true);
					break;

				case State.Dash:
					if (dash != null) dash.gameObject.SetActive(true);
					break;

				case State.DashUp:
					if (dashUp != null) dashUp.gameObject.SetActive(true);
					break;

				case State.GroundAttack:
					if (groundAttack != null) groundAttack.gameObject.SetActive(true);
					break;

				case State.AerialAttack:
					if (aerialAttack != null) aerialAttack.gameObject.SetActive(true);
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
