using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class helps the movingObject avatar transition through any animations changes the it will undergo.
// Should be pretty tightly linked to movingObjectMovement.cs
public class SpriteAnimator : MonoBehaviour {
	public MovingObject movingObject;

	public SpriteRenderer idle;
	public SpriteRenderer walk;
	public SpriteRenderer jump;
	public SpriteRenderer dash;
	public SpriteRenderer dashUp;
	public SpriteRenderer groundAttack;
	public SpriteRenderer aerialAttack;

	public bool defaultFacingLeft = false;

	public MovingObject.AnimationState storedState = MovingObject.AnimationState.Undefined;

	public Color blinkTint1; 
	public Color blinkTint2;
	public int   blinkFrames = 30;

	void Start() {
		SetState(MovingObject.AnimationState.Idle);
	}

	// Do any blinking calculation after all behavioral state changes is done
	void LateUpdate() {
		CheckBlinking();
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// ANIMATION CALLBACKS
	////////////////////////////////////////////////////////////////////////////////////////////////
	// Right now only used by groundAttack animation.
	// Not really sure what else this is supposed to do.
	public virtual void EndAnimation(MovingObject.AnimationState state) {
		//# movingObject.EndAttack();
	}	

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COLOR MANAGEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void CheckBlinking() {
		if (movingObject.IsBlinking()) {
			int framesSinceBlinking = movingObject.GetFramesSinceBlinkingStart();
			int blinkCycleLength    = movingObject.GetBlinkCycleLength();
			int numBlinksSince      = framesSinceBlinking / blinkCycleLength;

			if ( (numBlinksSince % 2) == 0) {
				SetColor(blinkTint1);
			}
			else {
				SetColor(blinkTint2);
			}
		}
		else {
			SetColor(Color.white);
		}	
	}

	public void SetColor(Color color) {
		SetSpriteColor(        idle, color);
		SetSpriteColor(        walk, color);
		SetSpriteColor(        jump, color);
		SetSpriteColor(        dash, color);
		SetSpriteColor(      dashUp, color);
		SetSpriteColor(groundAttack, color);
		SetSpriteColor(aerialAttack, color);
	}

	private void SetSpriteColor(SpriteRenderer spriteRenderer, Color color) {
		if (spriteRenderer != null) spriteRenderer.color = color;
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
	public void SetState(MovingObject.AnimationState state) {
		// Only allow the state to "change" if there truly is a change.
		// Setting an animated game object disabled and then enabled will probably cause the
		// animation to restart.
		if (state != storedState) {
			// First set everything to be disabled.
			SetSpriteActive(        idle, false);
			SetSpriteActive(        walk, false);
			SetSpriteActive(        jump, false);
			SetSpriteActive(        dash, false);
			SetSpriteActive(      dashUp, false);
			SetSpriteActive(groundAttack, false);
			SetSpriteActive(aerialAttack, false);

			// Re-enable based on state
			switch (state) {
				case MovingObject.AnimationState.Idle:
					SetSpriteActive(idle);
					break;

				case MovingObject.AnimationState.Walk:
					SetSpriteActive(walk);
					break;

				case MovingObject.AnimationState.Jump:
					SetSpriteActive(jump);
					break;

				case MovingObject.AnimationState.Dash:
					SetSpriteActive(dash);
					break;

				case MovingObject.AnimationState.DashUp:
					SetSpriteActive(dashUp);
					break;

				case MovingObject.AnimationState.GroundAttack:
					SetSpriteActive(groundAttack);
					break;

				case MovingObject.AnimationState.AerialAttack:
					SetSpriteActive(aerialAttack);
					break;

				default:
					// Default to idle, but keep the state undefined, so we can properly switch
					// into idle later.
					state = MovingObject.AnimationState.Undefined;
					SetSpriteActive(idle);
					break;
			}

			storedState = state;
		}
	}

	private void SetSpriteActive(SpriteRenderer spriteRenderer, bool active = true) {
		if (spriteRenderer != null) spriteRenderer.gameObject.SetActive(true);
	}

}
