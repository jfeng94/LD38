using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bridge between animator and per-sprite-frame management. Used for things like activating hitboxes
public class SpriteAnimation : MonoBehaviour {
	public MovingObject.AnimationState state;
	public SpriteAnimator animator;

	// Called by animation
	public void EndAnimationCallback() {
		animator.EndAnimation(state);
	}
}
