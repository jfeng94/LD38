using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves as a public interface for all the various components that make up a player.
public class Player : Character {
	//----------------------------------------------------------------------------------------------
	// Dashing
	//----------------------------------------------------------------------------------------------
	public  float dashingVelocity  = 20f;
	public  int   numFramesDashing = 30;
	private int   dashStartFrame   = int.MinValue;

	//----------------------------------------------------------------------------------------------
	// Jumping
	//----------------------------------------------------------------------------------------------
	private float jumpingVelocity = 20f;

	//----------------------------------------------------------------------------------------------
	// Animator and state flags state flags
	//----------------------------------------------------------------------------------------------
	public PlayerAnimator animator;
	private bool dashing     = false;
	private bool movingLeft  = false;
	private bool movingRight = false;
	private bool attacking   = false;
	private bool facingLeft  = false;

	//----------------------------------------------------------------------------------------------
	// Interaction
	//----------------------------------------------------------------------------------------------
	private IInteractable currentInteractable = null;

	//----------------------------------------------------------------------------------------------
	// Aggro
	//----------------------------------------------------------------------------------------------
	// List of all enemies that currently hold aggro on the player
	private List<Enemy> enemiesWithAggro = new List<Enemy>();


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MONOBEHAVIOUR METHODS
	////////////////////////////////////////////////////////////////////////////////////////////////
	// Update is called once per frame
	protected override void Update () {
		base.Update();

		CheckInvincibility();

		CheckDashing();

		// Go through all enemies with aggro. If we're sufficiently far enough away from it, 
		// it should drop aggro.
		CheckAggro();

		//------------------------------------------------------------------------------------------
		// ON KEY PRESS DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.LeftArrow))  { TurnLeft();  movingLeft  = true; }
		if (Input.GetKeyDown(KeyCode.RightArrow)) { TurnRight(); movingRight = true; }
		if (Input.GetKeyDown(KeyCode.F))          Interact();
		if (Input.GetKeyDown(KeyCode.A))          Attack();
		if (Input.GetKeyDown(KeyCode.D))          Dash();


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
	protected override int GetGroundedLayerMask() {
		LayerMask groundLayer = LayerMask.NameToLayer("Ground");
		LayerMask enemyLayer  = LayerMask.NameToLayer("Enemy");
		int layerMask = (1 << groundLayer.value) | (1 << enemyLayer.value);
		return layerMask;
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// ATTACKING
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Attack() {
		animator.SetState(PlayerAnimator.State.Attack1);
		attacking = true;
	}

	public void EndAttack() {
		attacking = false;
		UpdateAnimationState();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// DASHING
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected void CheckDashing() {
		dashing = (Time.frameCount - dashStartFrame < numFramesDashing);

		if (dashing) {
			Vector3 velocity = rb.velocity;

			velocity.x = dashingVelocity;
			if (facingLeft) velocity.x = -1f * velocity.x;

			rb.velocity = velocity;
		}
	}

	public void Dash() {
		if (!attacking) {
			animator.SetState(PlayerAnimator.State.Dash);
			dashing = true;
			dashStartFrame = Time.frameCount;
		}
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
	public override void InflictDamage(int damage, Vector2 direction, float strength) {
		if (! invincible) {
			base.InflictDamage(damage, direction, strength);

			StartInvincibility();

			// TODO -- Check for game overs?
			if (health <= 0) {
				Debug.Log("Game Over!!!");
			}
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
		// Only allow jumping if we're grounded and not attacking
		if (grounded && !attacking) {
			Vector2 initialVelocity = rb.velocity;
			initialVelocity.y = jumpingVelocity;
			rb.velocity = initialVelocity;	
			// TODO: Start animating jump cycle

			grounded = false;
		}
	}

	public void MoveLeft()  { Move(true);  }
	public void MoveRight() { Move(false); }

	// The actual logic behind movement, to reduce code duplication as we iterate with movement.
	private void Move(bool left) {
		// Note that when the user dashes or attacks, they lock themselves into that animation.
		// As a result, we block any 
		if (!dashing && !attacking) {
			float sign = 1f;
			if (left) sign = -1f;

			Vector3 velocity = rb.velocity;

			if (grounded) velocity.x += sign * movementSpeed;
			else          velocity.x += sign * aerialDrift;				

			if (Mathf.Abs(velocity.x) > maxSpeed) velocity.x = sign * maxSpeed;

			rb.velocity = velocity;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// SPRITE ANIMATION
	////////////////////////////////////////////////////////////////////////////////////////////////
	private void UpdateAnimationState() {
		if (attacking) {
			animator.SetState(PlayerAnimator.State.Attack1);			
		}
		else if (dashing) {
			animator.SetState(PlayerAnimator.State.Dash);
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
		if (!attacking && !dashing) {
			animator.TurnLeft();
			facingLeft = true;
		}
	}

	public void TurnRight() {
		if (!attacking && !dashing) {
			animator.TurnRight();
			facingLeft = false;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
	 	IInteractable interactable = collider.gameObject.GetComponent<IInteractable>();
	 	if (interactable != null) {
	 		if (interactable.CanInteract()) {
	 			if (currentInteractable != null) {
	 				currentInteractable.StopSignaling();
	 			}

		 		currentInteractable = interactable;
		 		currentInteractable.StartSignaling();
	 		}
	 	}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
	 	IInteractable interactable = collider.gameObject.GetComponent<IInteractable>();
	 	if (interactable != null && interactable == currentInteractable) {
	 		currentInteractable.StopSignaling();
	 		currentInteractable = null;
	 	}
	}

}
