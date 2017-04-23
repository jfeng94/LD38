using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves as a public interface for all the various components that make up a player.
public class Player : MonoBehaviour {
	public PlayerStatus   status;
	public PlayerAnimator animator;

	private Rigidbody2D rb;
	private float jumpingVelocity = 20f;

	// Movement state flags
	private bool grounded    = true;
	private bool movingLeft  = false;
	private bool movingRight = false;

	// Hitstun state variables
	private const int hitstunFrameCount = 60; 
	private int lastHitFrame = -1 * hitstunFrameCount;

	private Interactable currentInteractable = null;

	// Direction from center of player game object to feet.
	private Vector3 directionTowardsFeet = Vector3.down;
	// Distance from center of player game object to feet.
	private float distanceToFeet = 0.8f;

	public GameObject colliderVisualizer;

	// How fast the player can possibly move
	private float maxSpeed = 5f;

	// How much the user accelerates per frame while holding a direction
	private float movementSpeed = 1f;

	// How much the user can affect their aerial trajectory.
	private float aerialDrift = 0.5f;

	// List of all enemies that currently hold aggro on the player
	private List<Enemy> enemiesWithAggro = new List<Enemy>();

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		CheckGrounded();

		// Go through all enemies with aggro. If we're sufficiently far enough away from it, 
		// it should drop aggro.
		CheckAggro();

		//------------------------------------------------------------------------------------------
		// ON KEY PRESS DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.LeftArrow))  { TurnLeft();  movingLeft  = true; }
		if (Input.GetKeyDown(KeyCode.RightArrow)) { TurnRight(); movingRight = true; }
		if (Input.GetKeyDown(KeyCode.F))          Interact();


		//------------------------------------------------------------------------------------------
		// ON KEY PRESS UP
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyUp(KeyCode.LeftArrow))  movingLeft  = false;
		if (Input.GetKeyUp(KeyCode.RightArrow)) movingRight = false;

		//------------------------------------------------------------------------------------------
		// WHILE HOLDING KEY DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKey(KeyCode.Space))      Jump();
		if (Input.GetKey(KeyCode.LeftArrow))  MoveLeft();
		if (Input.GetKey(KeyCode.RightArrow)) MoveRight();

		UpdateAnimationState();

	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// GROUNDEDNESS
	////////////////////////////////////////////////////////////////////////////////////////////////
	private void CheckGrounded() {
		LayerMask groundLayer = LayerMask.NameToLayer("Ground");
		int layerMask = 1 << groundLayer.value;

		// Check for groundedness
		Vector3 footPosition = transform.position + (directionTowardsFeet * distanceToFeet);
		Collider2D collider = Physics2D.OverlapBox(new Vector2(footPosition.x, footPosition.y),
		                                           new Vector2(0.6f, 0.2f), 0f, layerMask);

		colliderVisualizer.transform.position = footPosition;
		colliderVisualizer.transform.localScale = new Vector3(0.6f, 0.2f, 1f);


		if (collider != null && rb.velocity.y <= 0) {
			grounded = true;
		}
		else grounded = false;
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// ENEMY AGGRO
	////////////////////////////////////////////////////////////////////////////////////////////////
	private void CheckAggro() {
		List<Enemy> deAggroList = new List<Enemy>();
		for (int i = 0; i < enemiesWithAggro.Count; i++) {
			Vector3 displacement = enemiesWithAggro[i].transform.position - transform.position;
			if (displacement.magnitude > enemiesWithAggro[i].aggroRange) {
				deAggroList.Add(enemiesWithAggro[i]);
			}
		}

		for (int i = 0; i < deAggroList.Count; i++) {
			deAggroList[i].RemoveAggro(this);
			enemiesWithAggro.Remove(deAggroList[i]);
		}		
	}
	
	public List<Enemy> GetAggroEnemies() {
		return enemiesWithAggro;
	}

	public void AddEnemyAggro(Enemy enemy) {
		if (!enemiesWithAggro.Contains(enemy)) {
			enemiesWithAggro.Add(enemy);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COMBAT AND DAMAGE
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Heal(int heal) {
		status.GainHealth(heal);
	}

	public void InflictDamage(int damage, Vector2 direction, float strength) {
		if (Time.frameCount - lastHitFrame > hitstunFrameCount) {
			status.SpendHealth(damage);

			rb.AddForce(direction * strength, ForceMode2D.Impulse);

			lastHitFrame = Time.frameCount;

			// TODO -- Check for game overs?
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// INTERACTION
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Interact() {
		if (currentInteractable != null) {
			currentInteractable.Interact();
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MOVEMENT
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Jump() {
		// Only allow jumping if we're grounded
		if (grounded) {
			Vector2 initialVelocity = rb.velocity;
			initialVelocity.y = jumpingVelocity;
			rb.velocity = initialVelocity;	
			// TODO: Start animating jump cycle

			grounded = false;
		}
	}

	public void MoveLeft() {
		Vector3 velocity = rb.velocity;
		if (grounded) velocity.x += -1f * movementSpeed;
		else          velocity.x += -1f * aerialDrift;
		
		if (velocity.x < -1f * maxSpeed) velocity.x = -1f * maxSpeed;

		rb.velocity = velocity;
	}

	public void MoveRight() {
		Vector3 velocity = rb.velocity;
		if (grounded) velocity.x += movementSpeed;
		else          velocity.x += aerialDrift;
		
		if (velocity.x > maxSpeed) velocity.x = maxSpeed;

		rb.velocity = velocity;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// SPRITE ANIMATION
	////////////////////////////////////////////////////////////////////////////////////////////////
	private void UpdateAnimationState() {
		if (Time.frameCount - lastHitFrame < hitstunFrameCount) {
			animator.SetState(PlayerAnimator.State.Stun);
		}
		else if (!grounded) {
			animator.SetState(PlayerAnimator.State.Jump);
		}
		else if (movingLeft ^ movingRight) {
			animator.SetState(PlayerAnimator.State.Walk);
		}
		else {
			animator.SetState(PlayerAnimator.State.Idle);
		}
	}

	public void TurnLeft()  {
		animator.TurnLeft();
	}
	public void TurnRight() {
		animator.TurnRight();
	}
	public void Crouch()    {
	}

	void OnTriggerEnter2D(Collider2D collider) {
	 	Interactable interactable = collider.gameObject.GetComponent<Interactable>();
	 	if (interactable != null) {
	 		currentInteractable = interactable;
	 	}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
	 	Interactable interactable = collider.gameObject.GetComponent<Interactable>();
	 	if (interactable != null && interactable == currentInteractable) {
	 		currentInteractable = null;
	 	}
	}

}
