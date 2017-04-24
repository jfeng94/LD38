using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour {
	public EnemyAnimator.State state;
	public EnemyAnimator animator;

	// Called by animation
	public void EndAnimationCallback() {
		animator.EndAnimation(state);
	}
}
