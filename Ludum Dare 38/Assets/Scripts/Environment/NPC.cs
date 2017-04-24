using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character, IInteractable {
	public GameObject textBubble;
	public GameObject indicator;
	public bool canInteract = true;

	public NPCAnimator animator;

	// If followPath is true, this NPC will walk towards the nodes specified in pathNodes. As it 
	// arrives at each node, it will wait for a random number of frames (from minWaitFrames to 
	// maxWaitFrames)
	// When it reaches the end of the nodes list, it will wrap around to the first node.
	public bool followPath = false;
	public List<GameObject> pathNodes = new List<GameObject>();
	public int minWaitFrames = 10;
	public int maxWaitFrames = 60;
	private int pathIndex = 0;
	private int waitStartFrame = int.MinValue;
	private int waitFrameDuration = 0;
	private System.Random RNG = new System.Random();

	protected override void Start() {
		base.Start();

		if (textBubble != null) {
			textBubble.SetActive(false);
		}
	}

	protected override void Update() {
		if (followPath) {
			// Note we don't call base!
			if (Time.frameCount - waitStartFrame > 0 &&
			    Time.frameCount - waitStartFrame < waitFrameDuration) {
				// Do nothing. We wait.
				Debug.Log("Do nothing?");
			}
			else {
				waitFrameDuration = 0;
				waitStartFrame = int.MinValue;

				MoveTowardsPosition(pathNodes[pathIndex].transform.position);
			}
		}
	}

	public bool CanInteract() {
		return canInteract;
	}

	public void StartSignaling() {
		if (indicator != null) {
			indicator.SetActive(true);
		}
	}

	public void StopSignaling() {
		if (indicator != null) {
			indicator.SetActive(false);
		}
	}

	public void Interact() {
		if (indicator != null) {
			indicator.SetActive(false);
		}

		if (indicator != null) {
			textBubble.SetActive(true);
		}

		Invoke("HideTextBubble", 1f);
	}	

	private void HideTextBubble() {
		if (textBubble != null) {
			textBubble.SetActive(false);
		}
	}

	protected virtual void MoveTowardsPosition(Vector3 position) {
		if (rb != null) {
			float displacement = position.x - transform.position.x;

			Vector3 velocity = rb.velocity;

			if (Mathf.Abs(displacement) < 0.2f) {
				velocity.x = 0;
				rb.velocity = velocity;

				int numNodes = pathNodes.Count;
				pathIndex = (((pathIndex + 1) % numNodes) + numNodes) % numNodes;

				waitFrameDuration = RNG.Next(minWaitFrames, maxWaitFrames);
				waitStartFrame = Time.frameCount;

				if (animator != null) {
					animator.SetState(NPCAnimator.State.Idle);
				}
				return;
			}

			if (grounded) velocity.x += Mathf.Sign(displacement) * movementSpeed;
			else          velocity.x += Mathf.Sign(displacement) * aerialDrift;
			
			if (Mathf.Abs(velocity.x) > maxSpeed && Mathf.Sign(velocity.x) == Mathf.Sign(displacement)) {
				velocity.x = Mathf.Sign(displacement) * maxSpeed;
			}

			if (Mathf.Sign(displacement) > 0) {
				if (animator != null) animator.TurnRight();
			}
			else                              {
				if (animator != null) animator.TurnLeft();  
			}

			if (animator != null) {
				animator.SetState(NPCAnimator.State.Walk);
			}

			rb.velocity = velocity;
		}
	}	
}
