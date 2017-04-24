using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basically totally copied over from player animator.
public class EnemyAnimator : MonoBehaviour {
	public Enemy enemy;

	public enum State {
		Undefined,
		Idle,
		Walk,
		Dead,
	}

	public SpriteRenderer idle;
	public SpriteRenderer walk;
	public SpriteRenderer dead;

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
		if (enemy.IsInvincible()) {
			int framesSinceInvincible = enemy.GetFramesSinceInvincible();
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
		switch (state) {
			case State.Dead:
				enemy.gameObject.SetActive(false);
				break;

			default:
				break;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COLOR MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetColor(Color color) {
		if (idle    != null) idle.color = color;
		if (walk    != null) walk.color = color;
		if (dead    != null) dead.color = color;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// FACING MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void TurnLeft() {
		Debug.Log("TurnLeft");
		if (defaultFacingLeft) {
			transform.localScale = new Vector3(-1, 1, 1);
		}
		else {
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

	public void TurnRight() {
		Debug.Log("TurnRight");
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
			if (idle != null) idle.gameObject.SetActive(false);
			if (walk != null) walk.gameObject.SetActive(false);
			if (dead != null) dead.gameObject.SetActive(false);

			// Re-enable based on state
			switch (state) {
				case State.Idle:
					if (idle != null) idle.gameObject.SetActive(true);
					break;

				case State.Walk:
					if (walk != null) walk.gameObject.SetActive(true);
					break;

				case State.Dead:
					if (dead != null) dead.gameObject.SetActive(true);
					break;

				default:
					// Default to idle, but keep the state undefined, so we can properly switch
					// into idle later.
					state = State.Undefined;
					if (idle != null)idle.gameObject.SetActive(true);
					break;
			}

			storedState = state;
		}
	}
}
