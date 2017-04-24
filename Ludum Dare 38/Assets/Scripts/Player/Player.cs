using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves as a public interface for all the various components that make up a player.
public class Player : Character {
	//----------------------------------------------------------------------------------------------
	// Dashing
	//----------------------------------------------------------------------------------------------
	public  float horizontalDashingVelocity = 20f;
	public  float verticalDashingVelocity   = 40f;
	public  int   numFramesDashing          = 30;
	private int   dashStartFrame            = int.MinValue;

	//----------------------------------------------------------------------------------------------
	// Jumping
	//----------------------------------------------------------------------------------------------
	public float jumpingVelocity = 20f;

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

		//------------------------------------------------------------------------------------------
		// ON KEY PRESS DOWN
		//------------------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.LeftArrow))  { TurnLeft();  movingLeft  = true; }
		if (Input.GetKeyDown(KeyCode.RightArrow)) { TurnRight(); movingRight = true; }
		if (Input.GetKeyDown(KeyCode.F))          Interact();
		if (Input.GetKeyDown(KeyCode.A))          Attack();
		if (Input.GetKeyDown(KeyCode.D))          Dash();
		if (Input.GetKeyDown(KeyCode.E))          DashUp();


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
		if (rb.velocity.x > 0) {
			TurnRight();
		}
		else if (rb.velocity.x < 0) {
			TurnLeft();
		}
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
		LayerMask enemyLayer  = LayerMask.NameToLayer("Enemy");
		int layerMask = (1 << groundLayer.value) | (1 << enemyLayer.value);
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
			}
			else {
				animator.SetState(PlayerAnimator.State.AerialAttack);
				aAttacking = true;
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

			rb.velocity = velocity;
		}

		if (vDashing) {
			Vector3 velocity = rb.velocity;

			velocity.y = verticalDashingVelocity;

			rb.velocity = velocity;
		}
	}

	public void Dash() {
		if ( ! isAttacking) {
			if (isDashing) {
				CancelDash();
			}

			if (hasMana) {
				animator.SetState(PlayerAnimator.State.Dash);
				hDashing = true;
				dashStartFrame = Time.frameCount;
				ExpendMana(1);
			}
		}
	}

	public void DashUp() {
		if ( ! isAttacking) {
			if (isDashing) {
				CancelDash();
			}

			if (hasMana) {
				animator.SetState(PlayerAnimator.State.DashUp);
				vDashing = true;
				dashStartFrame = Time.frameCount;
				ExpendMana(1);
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
	public void Jump() {
		// Only allow jumping if we're grounded and not gAttacking
		if (grounded && !gAttacking) {
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
		if ( ! isAttacking && !hDashing) {
			animator.TurnLeft();
			facingLeft = true;
		}
	}

	public void TurnRight() {
		if ( ! isAttacking && !hDashing) {
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
