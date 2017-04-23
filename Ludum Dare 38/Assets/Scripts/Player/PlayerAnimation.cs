using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bridge between animator and per-sprite-frame management. Used for things like activating hitboxes
public class PlayerAnimation : MonoBehaviour {
	public PlayerAnimator.State state;
	public PlayerAnimator animator;

	// Called by animation
	public void EndAnimationCallback() {
		animator.EndAnimation(state);
	}
}
