using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {
	//----------------------------------------------------------------------------------------------
	// Groundedness
	//----------------------------------------------------------------------------------------------
	// Whether the character is touching the ground
	protected bool grounded = true;

	//----------------------------------------------------------------------------------------------
	// Movement
	//----------------------------------------------------------------------------------------------
	// How fast the player can possibly move
	public float maxSpeed = 5f;

	// How much the user accelerates per frame while holding a direction
	public float movementSpeed = 1f;

	// How much the user can affect their aerial trajectory.
	public float aerialDrift = 0.5f;

	//----------------------------------------------------------------------------------------------
	// Path following
	//----------------------------------------------------------------------------------------------
	// If walkAround is true, this MovingObject will walk towards the nodes specified in path. As it 
	// arrives at each node, it will wait for a random number of frames (from minWaitFrames to 
	// maxWaitFrames).
	//
	// To specify a path, pass the NPC an object whose children are the nodes of the path.
	//
	// If randomWalk is true, the NPC will walk to a random node each time.
	// When it reaches the end of the nodes list, it will wrap around to the first node.
	public  MovementPath path;
	public  bool walkAround        = true;
	public  bool pathIgnoresX      = false;
	public  bool pathIgnoresY      = false;
	public  bool pathIgnoresZ      = false;
	public  bool randomWalk        = true;
	public  int  minWaitFrames     = 10;
	public  int  maxWaitFrames     = 60;
	private int  pathIndex         = 0;
	private int  waitStartFrame    = int.MinValue;
	private int  waitFrameDuration = 0;

	private System.Random RNG = new System.Random();

	//----------------------------------------------------------------------------------------------
	// Jumping
	//----------------------------------------------------------------------------------------------
	public float jumpingVelocity = 5f;

	//----------------------------------------------------------------------------------------------
	// Physics
	//----------------------------------------------------------------------------------------------
	protected Rigidbody2D rb;

	//----------------------------------------------------------------------------------------------
	// Animation states
	//----------------------------------------------------------------------------------------------
	protected SpriteAnimator animator; 
	public enum AnimationState {
		Undefined,
		Idle,
		Walk,
		Jump,
		Dash,
		DashUp,
		GroundAttack,
		AerialAttack,
		Dead,
	}
	protected AnimationState state = AnimationState.Undefined;

	protected bool facingLeft  = false;

	//----------------------------------------------------------------------------------------------
	// Blinking state management
	//----------------------------------------------------------------------------------------------
	// Ostensibly, we will need to make objects blink. Here are some examples when:
	// 1. Draw the user's attention
	// 2. Designate invincibility for the player
	// 3. Give visual feedback that an enemy was hit
	protected bool blinking;
	protected int  blinkingStartFrame;
	public    int  blinkCycleLength = 30;
	public    int  blinkingDuration = 180;


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MONOBEHAVIOUR METHODS
	////////////////////////////////////////////////////////////////////////////////////////////////
	// Use this for initialization
	protected virtual void Start () {
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0.8f;

		animator = GetComponentInChildren<SpriteAnimator>();
	}
	
	protected virtual void Update() {
		CheckGrounded();
		CheckBlinking();

		if (walkAround) {
			FollowPath();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// UPDATE METHODS
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected virtual void CheckGrounded() {
		// We can only be grounded if the y velocity isn't positive (going up);
		if (rb.velocity.y <= 0) {
			// Get all the box-colliders on this object
			BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
			for (int i = 0; i < colliders.Length; i++) {

				// Don't use trigger colliders to determine groundedness	
				if ( ! colliders[i].isTrigger) {

					// If that collider touches a layer we consider to be ground
					if (colliders[i].IsTouchingLayers(GetGroundedLayerMask())) {
						grounded = true;
						return;
					}
				}
			}

			// Get all the box-colliders on childed to this object
			// Note this does not retrieve any colliders that are on inactive objects.
			colliders = GetComponentsInChildren<BoxCollider2D>();
			for (int i = 0; i < colliders.Length; i++) {

				// Don't use trigger colliders to determine groundedness	
				if ( ! colliders[i].isTrigger) {

					// If that collider touches a layer we consider to be ground
					if (colliders[i].IsTouchingLayers(GetGroundedLayerMask())) {
						grounded = true;
						return;
					}
				}
			}
		}
		grounded = false;
	}

	public void CheckBlinking() {
		if (blinking) {
			if ( (Time.frameCount - blinkingStartFrame) > blinkingDuration) {
				blinking = false;
			}
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// GROUNDEDNESS
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected virtual int GetGroundedLayerMask() {
		LayerMask groundLayer = LayerMask.NameToLayer("Ground");
		int layerMask = (1 << groundLayer.value);
		return layerMask;
	} 


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// PATHING
	////////////////////////////////////////////////////////////////////////////////////////////////
	public virtual void FollowPath() {
		if (walkAround && path != null) {
			int numNodes = path.GetNumNodes();
			if (numNodes != 0) {

				if (Time.frameCount - waitStartFrame > 0 &&
				    Time.frameCount - waitStartFrame < waitFrameDuration) {
					// Do nothing. We wait.
				}
				else {
					waitFrameDuration = 0;
					waitStartFrame = int.MinValue;

					// Get current node
					PathNode node = path.GetNodeAtIndex(pathIndex);

					if (node != null) {
						// Get displacement between this MovingObject and its target position
						Vector3 displacement = node.transform.position - transform.position;

						// Get rid of undesired dimensional contributions to displacement
						if (pathIgnoresX) {
							displacement.x = 0;
						}
						if (pathIgnoresY) {
							displacement.y = 0;
						}
						if (pathIgnoresZ) {
							displacement.z = 0;
						}

						// If we are within a reasonable distance from the target node, switch targets.
						if (displacement.magnitude < 0.2f) {
							Vector3 velocity = rb.velocity;
							velocity.x = 0;
							rb.velocity = velocity;

							if (randomWalk) {
								int newIndex = RNG.Next(0, numNodes - 1);

								if (newIndex == pathIndex) {
									newIndex = newIndex - 1;
								}

								pathIndex = newIndex;
							}
							else {
								pathIndex = pathIndex + 1;
							}

							pathIndex = (((pathIndex) % numNodes) + numNodes) % numNodes;

							waitFrameDuration = RNG.Next(minWaitFrames, maxWaitFrames);
							waitStartFrame = Time.frameCount;

							if (animator != null) {
								animator.SetState(AnimationState.Idle);
							}
							return;
						}
						MoveTowardsPosition(transform.position + displacement);
					}
				}
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MOVEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void MoveLeft()  { Move(true);  }
	public void MoveRight() { Move(false); }

	// The actual logic behind movement, to reduce code duplication as we iterate with movement.
	protected virtual void Move(bool left) {
		if (CanMove()) {
			float sign = 1f;
			if (left) sign = -1f;

			Vector3 velocity = rb.velocity;

			if (grounded) velocity.x += sign * movementSpeed;
			else          velocity.x += sign * aerialDrift;				

			if (Mathf.Abs(velocity.x) > maxSpeed) velocity.x = sign * maxSpeed;

			rb.velocity = velocity;
		}
	}

	protected virtual void MoveTowardsPosition(Vector3 position) {
		if (CanMove()) {
			Vector3 displacement = position - transform.position;

			// Get rid of undesired dimensional contributions to displacement
			if (pathIgnoresX) {
				displacement.x = 0;
			}
			if (pathIgnoresY) {
				displacement.y = 0;
			}
			if (pathIgnoresZ) {
				displacement.z = 0;
			}

			Vector3 velocity = rb.velocity;

			if (displacement.magnitude < 0.2f) {
				rb.velocity = Vector3.zero;

				if (animator != null) {
					animator.SetState(AnimationState.Idle);
				}
				return;
			}

			if (grounded) {
				velocity += displacement.normalized * movementSpeed;
			}
			else {
				velocity += displacement.normalized * aerialDrift;
			}
			
			if (velocity.magnitude > maxSpeed) {
				velocity = velocity.normalized * maxSpeed;
			}

			if (velocity.x > 0) {
				if (animator != null) animator.TurnRight();
			}
			else {
				if (animator != null) animator.TurnLeft();  
			}

			if (animator != null) {
				animator.SetState(AnimationState.Walk);
				if (gameObject.name == "Blob enemy") {
					Debug.Log("???? displacement is " + displacement);
				}
			}

			rb.velocity = velocity;
		}
	}

	// Implement this in child classes to dictate whether an object can move.
	// For instance, in Player, we may return false if it's currently attacking.
	public virtual bool CanMove() { return true; }


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// JUMPING
	////////////////////////////////////////////////////////////////////////////////////////////////
	public virtual bool CanJump() { return true; }

	public virtual void Jump() {
		// Only allow jumping if we're grounded and not gAttacking
		if (CanJump()) {
			Vector2 initialVelocity = rb.velocity;
			initialVelocity.y = jumpingVelocity;
			rb.velocity = initialVelocity;	
			// TODO: Start animating jump cycle

			grounded = false;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// BLINKING
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void StartBlinking() {
		blinking = true;
		blinkingStartFrame = Time.frameCount;
	}

	public void StopBlinking() {
		blinking = false;
		blinkingStartFrame = -1;
	}

	public int GetBlinkCycleLength() {
		return blinkCycleLength;
	}

	public bool IsBlinking() {
		return blinking;
	}

	public int GetFramesSinceBlinkingStart() {
		return Time.frameCount - blinkingStartFrame;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// ANIMATION
	////////////////////////////////////////////////////////////////////////////////////////////////
	public virtual bool CanTurn()  { return true; }

	public void TurnLeft()  {
		facingLeft = true;
		if (CanTurn()) {
			animator.TurnLeft();
		}
	}

	public void TurnRight() {
		facingLeft = false;
		if (CanTurn()) {
			animator.TurnRight();
		}
	}
}
