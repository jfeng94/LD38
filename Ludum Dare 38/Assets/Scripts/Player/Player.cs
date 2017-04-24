using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves as a public interface for all the various components that make up a player.
public class Player : Character {
	public PlayerAnimator animator;

	private float jumpingVelocity = 20f;

	// Movement state flags
	private bool movingLeft  = false;
	private bool movingRight = false;
	private bool attacking   = false;

	private IInteractable currentInteractable = null;

	// List of all enemies that currently hold aggro on the player
	private List<Enemy> enemiesWithAggro = new List<Enemy>();

	// Update is called once per frame
	protected override void Update () {
		base.Update();

		CheckInvincibility();

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


		//------------------------------------------------------------------------------------------
		// ON KEY PRESS UP
		//------------------------------------------------------------sssssssssssssssssssssssssssssssssssss------------------------------
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

	public void MoveLeft() {
		if (!attacking) {
			Vector3 velocity = rb.velocity;
			if (grounded) velocity.x += -1f * movementSpeed;
			else          velocity.x += -1f * aerialDrift;
			
			if (velocity.x < -1f * maxSpeed) velocity.x = -1f * maxSpeed;

			rb.velocity = velocity;
		}
	}

	public void MoveRight() {
		if (!attacking) {
			Vector3 velocity = rb.velocity;
			if (grounded) velocity.x += movementSpeed;
			else          velocity.x += aerialDrift;
			
			if (velocity.x > maxSpeed) velocity.x = maxSpeed;

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
		if (!attacking) {
			animator.TurnLeft();
		}
	}
	public void TurnRight() {
		if (!attacking) {
			animator.TurnRight();
		}
	}
	public void Crouch() {
	}

	public void Attack() {
		animator.SetState(PlayerAnimator.State.Attack1);
		attacking = true;
	}

	public void EndAttack() {
		attacking = false;
		UpdateAnimationState();
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
