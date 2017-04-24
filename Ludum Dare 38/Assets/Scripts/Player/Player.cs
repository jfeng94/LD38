using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves as a public interface for all the various components that make up a player.
public class Player : Character {
	//----------------------------------------------------------------------------------------------
	// Dashing
	//----------------------------------------------------------------------------------------------
	public  float horizontalDashingVelocity = 30f;
	public  float verticalDashingVelocity   = 50f;
	public  int   numFramesDashing          = 30;
	private int   dashStartFrame            = int.MinValue;
	private int 	gAttackStartFrame 				= int.MinValue;
	private int 	aAttackStartFrame 				= int.MinValue;
	private int 	jumpCounter 							= 0;
	private bool 	justJumped 								= false;
	private bool 	stopJump 									= false;
	//----------------------------------------------------------------------------------------------
	// Jumping
	//----------------------------------------------------------------------------------------------
	public float jumpingVelocity = 5f;

	//----------------------------------------------------------------------------------------------
	// Animator and state flags state flags
	//----------------------------------------------------------------------------------------------
	public PlayerAnimator animator;
	private bool hDashing    = false;
	private bool vDashing    = false;
	private bool movingLeft  = false;
	private bool movingRight = false;
	private bool gAttacking  = false;
	private bool aAttacking  = false;
	private bool facingLeft  = false;

	//----------------------------------------------------------------------------------------------
	// Passive idle regen
	//----------------------------------------------------------------------------------------------
	public int numFramesHealthRegen = 300;
	public int numFramesManaRegen   = 600;
	public int idleStartFrame       = int.MaxValue;

	//----------------------------------------------------------------------------------------------
	// Interaction
	//----------------------------------------------------------------------------------------------
	private IInteractable currentInteractable = null;

	//----------------------------------------------------------------------------------------------
	// Aggro
	//----------------------------------------------------------------------------------------------
	// List of all enemies that currently hold aggro on the player
	private List<Enemy> enemiesWithAggro = new List<Enemy>();

	public bool isAttacking {
		get {
			return (gAttacking || aAttacking);
		}
	}

	public bool isDashing {
		get {
			return (hDashing || vDashing);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MONOBEHAVIOUR METHODS
	////////////////////////////////////////////////////////////////////////////////////////////////
	// Update is called once per frame
	protected override void Update () {
		base.Update();

		CheckInvincibility();

		CheckDashing();

		CheckRegen();

		// Go through all enemies with aggro. If we're sufficiently far enough away from it, 
		// it should drop aggro.
		CheckAggro();

		// Reset Long Jump amount.
		CheckGround();
		//------------------------------------------------------------------------------------------
		// ON KEY PRESS DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.LeftArrow))  { TurnLeft();  movingLeft  = true; }
		if (Input.GetKeyDown(KeyCode.RightArrow)) { TurnRight(); movingRight = true; }
		if (Input.GetKeyDown(KeyCode.F))          Interact();
		if (Input.GetKeyDown(KeyCode.A))          Attack();
		if (Input.GetKeyDown(KeyCode.D))          Dash();
		if (Input.GetKeyDown(KeyCode.E))          DashUp();
		if (Input.GetKeyDown(KeyCode.Space))      Jump();


		//------------------------------------------------------------------------------------------
		// ON KEY PRESS UP
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyUp(KeyCode.LeftArrow))  movingLeft  = false;
		if (Input.GetKeyUp(KeyCode.RightArrow)) movingRight = false;
		if (Input.GetKeyUp(KeyCode.Space))      stopJump = true;

		//------------------------------------------------------------------------------------------
		// WHILE HOLDING KEY DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKey(KeyCode.Space) &&
				!justJumped && !stopJump)      		LongerJump();
		if (Input.GetKey(KeyCode.LeftArrow))  MoveLeft();
		if (Input.GetKey(KeyCode.RightArrow)) MoveRight();
		if (Input.GetKey(KeyCode.A))          Attack();


		//------------------------------------------------------------------------------------------
		// QUIT APPLICATION
		//------------------------------------------------------------------------------------------
		if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) ||
		     Input.GetKey(KeyCode.Q)) {
			Application.Quit();
		}

		UpdateAnimationState();

	}

	void LateUpdate() {
		if (movingLeft && !movingRight) TurnLeft();
		else if (movingRight && !movingLeft) TurnRight();
		else if (movingRight && movingLeft) {
			if (rb.velocity.x > 0) {
				TurnRight();
			}
			else if (rb.velocity.x < 0) {
				TurnLeft();
			}
		}

		justJumped = false;
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// PASSIVE REGEN
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected void CheckRegen() {
		if ((Time.frameCount - idleStartFrame) > 0) {
			if ( (Time.frameCount - idleStartFrame) % numFramesHealthRegen == 0) {
				GainHealth(1);
			}
		}

		if ((Time.frameCount - firstFrameCanRegenMana) > 0) {
			if ( (Time.frameCount - firstFrameCanRegenMana) % numFramesManaRegen == 0) {
				GainMana(1);
			}	
		}
	}

	public float GetManaRegenProgress() {
		int frames = Time.frameCount - firstFrameCanRegenMana;
		if ((frames) > 0) {
			frames = ((frames % numFramesManaRegen) + numFramesManaRegen) % numFramesManaRegen;
			return (float) frames / (float) numFramesManaRegen;
		}
		else return 0f;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// GROUNDEDNESS
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected override int GetGroundedLayerMask() {
		LayerMask groundLayer = LayerMask.NameToLayer("Ground");
		int layerMask = (1 << groundLayer.value);
		return layerMask;
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//// ATTACKING
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void Attack() {
		if (!isAttacking) {
			if (isDashing) {
				CancelDash();
			}

			if (grounded) {	
				animator.SetState(PlayerAnimator.State.GroundAttack);
				gAttacking = true;
				gAttackStartFrame = Time.frameCount;
			}
			else {
				animator.SetState(PlayerAnimator.State.AerialAttack);
				aAttacking = true;
				aAttackStartFrame = Time.frameCount;
			}
		}
	}

	public void EndAttack() {
		gAttacking = false;
		aAttacking = false;
		UpdateAnimationState();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// DASHING
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected void CheckDashing() {
		if (isAttacking ||
			(Time.frameCount - dashStartFrame > 0 && Time.frameCount - dashStartFrame > numFramesDashing) ) {
			CancelDash();
		}

		if (hDashing) {
			Vector3 velocity = rb.velocity;

			velocity.x = horizontalDashingVelocity;
			if (facingLeft) velocity.x = -1f * velocity.x;
			// We don't want to move vertically when we're horizontally dashing.
			velocity.y = 0f;
			rb.velocity = velocity;
		}

		else if (vDashing) {
			Vector3 velocity = rb.velocity;

			velocity.y = verticalDashingVelocity;
			// We don't want to move horizontally when we're vertically dashing
			velocity.x = 0f;
			rb.velocity = velocity;
		}
		else rb.gravityScale = 0.8f;
	}

	public void Dash() {
		if (!hDashing) {
			if (isDashing) {
				CancelDash();
			}

			if (hasMana) {
				if (!isAttacking || 
						(isAttacking && (Time.frameCount - gAttackStartFrame > 10 || Time.frameCount - aAttackStartFrame > 3))) {

					// If you want to dash right after you attack
					if (facingLeft) animator.TurnLeft();
					else animator.TurnRight();

					gAttacking = false;
					aAttacking = false;
					animator.SetState(PlayerAnimator.State.Dash);
					hDashing = true;
					dashStartFrame = Time.frameCount;
					rb.gravityScale = 0;
					ExpendMana(1);
				}
			}
		}
	}

	public void DashUp() {
		if (!vDashing) {
			if (isDashing) {
				CancelDash();
			}

			if (hasMana) {
				if (!isAttacking || 
					(isAttacking && (Time.frameCount - gAttackStartFrame > 10 || Time.frameCount - aAttackStartFrame > 3))) {

					gAttacking = false;
					aAttacking = false;
					animator.SetState(PlayerAnimator.State.DashUp);
					grounded = false;
					stopJump = true;
					vDashing = true;
					dashStartFrame = Time.frameCount;
					ExpendMana(1);
				}
			}
		}
	}

	public void CancelDash() {
		hDashing = false;
		vDashing = false;
		Vector3 velocity = rb.velocity;
		velocity.x = 0f;
		velocity.y = 0f;
		//# if (facingLeft) velocity.x = -1f * velocity.x;
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
	public void CheckGround() {
		if (grounded) {
			jumpCounter = 0;
			aAttacking = false;
			stopJump = false;
		}
	}

	public void Jump() {
		// Only allow jumping if we're grounded and not gAttacking
		if (grounded && !gAttacking && !isDashing) {
			Vector2 initialVelocity = rb.velocity;
			initialVelocity.y = jumpingVelocity;
			rb.velocity = initialVelocity;	
			// TODO: Start animating jump cycle

			grounded = false;
			justJumped = true;
			AudioSource[] sounds = gameObject.GetComponents<AudioSource>();
			sounds[0].Play();
		}
	}

	public void LongerJump() {
		jumpCounter++;
		if (jumpCounter % 3 == 0 && jumpCounter < 15) {
			Vector2 continuingVelocity = rb.velocity;
			continuingVelocity.y = continuingVelocity.y + 4f;

			rb.velocity = continuingVelocity;
		}
	}

	public void MoveLeft()  { Move(true);  }
	public void MoveRight() { Move(false); }

	// The actual logic behind movement, to reduce code duplication as we iterate with movement.
	private void Move(bool left) {
		// Note that when the user dashes or attacks, they lock themselves into that animation.
		// As a result, we block any 
		if (!isDashing && !gAttacking) {
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
		if (gAttacking) {
			animator.SetState(PlayerAnimator.State.GroundAttack);
			idleStartFrame = int.MaxValue;			
		}
		else if (aAttacking) {
			animator.SetState(PlayerAnimator.State.AerialAttack);
			idleStartFrame = int.MaxValue;		
		}
		else if (hDashing) {
			animator.SetState(PlayerAnimator.State.Dash);
			idleStartFrame = int.MaxValue;
		}
		else if (vDashing) {
			animator.SetState(PlayerAnimator.State.DashUp);
			idleStartFrame = int.MaxValue;
		}
		else if (!grounded) {
			animator.SetState(PlayerAnimator.State.Jump);
			idleStartFrame = int.MaxValue;
		}
		else if (movingLeft ^ movingRight) {
			animator.SetState(PlayerAnimator.State.Walk);
			idleStartFrame = int.MaxValue;
		}
		else {
			animator.SetState(PlayerAnimator.State.Idle);
			if (idleStartFrame == int.MaxValue) {
				idleStartFrame = Time.frameCount;
			}
		}
		//# Debug.Log("UpdateAnimationState -- idleStartFrame: " + idleStartFrame);
	}

	public void TurnLeft()  {
		facingLeft = true;
		if ( ! isAttacking && !hDashing) {
			animator.TurnLeft();
		}
	}

	public void TurnRight() {
		facingLeft = false;
		if ( ! isAttacking && !hDashing) {
			animator.TurnRight();
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
