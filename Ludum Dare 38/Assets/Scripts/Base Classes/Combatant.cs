using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MovingObject, IHittable {
	//----------------------------------------------------------------------------------------------
	// Resources
	//----------------------------------------------------------------------------------------------
	public int maxHealth = 5;
	public int maxMana   = 4;

	public int health = 5;
	public int mana   = 3;
	protected int firstFrameCanRegenMana = int.MaxValue;

	protected bool isDead {
		get {
			return health <= 0;
		}
	}

	protected bool hasMana {
		get {
			return mana > 0;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// DEATH
	////////////////////////////////////////////////////////////////////////////////////////////////
	protected virtual void DieAHorribleDeath() {
		gameObject.SetActive(false);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// HEALTH
	////////////////////////////////////////////////////////////////////////////////////////////////
	public void GainHealth(int amount) {
		health += amount;
		EnsureHealthIsWithinBounds();
	}

	public void DealDamage(int amount) {
		health -= amount;
		EnsureHealthIsWithinBounds();
	}

	private void EnsureHealthIsWithinBounds() {
		if (health <= 0) {
			DieAHorribleDeath();
		}
		if (health > maxHealth) {
			health = maxHealth;
		}
	}

	public int GetHealth() {
		return health;
	}

	public int GetMaxHealth() {
		return maxHealth;
	}

	public void Heal(int heal) {
		GainHealth(heal);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// COMBAT AND DAMAGE
	////////////////////////////////////////////////////////////////////////////////////////////////
	public virtual void InflictDamage(int damage, Vector2 direction, float strength) {
		if (! blinking) {
			DealDamage(damage);

			if (!isDead) {
				if (rb != null) {
					Debug.Log("InflictDamage imparted force in " + direction + " direction with " + strength + " strength");
					rb.AddForce(direction * strength, ForceMode2D.Impulse);
				}
				else {
					Debug.Log("InflictDamage Rigidbody2D is null?");
				}

				StartBlinking();

				// TODO -- Check for game overs?
				if (health <= 0) {
					Debug.Log("Game Over!!!");
				}
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//// MANA
	////////////////////////////////////////////////////////////////////////////////////////////////
	public int GetMana() {
		return mana;
	}
	public int GetMaxMana() {
		return maxMana;
	}

	public void ExpendMana(int amount) {
		mana = mana - amount;
		EnsureManaIsWithinBounds();

		if (mana < maxMana) {
			if (firstFrameCanRegenMana == int.MaxValue) {
				firstFrameCanRegenMana = Time.frameCount;
			}
		}
	}

	public void GainMana(int amount) {
		mana = mana + amount;
		EnsureManaIsWithinBounds();

		if (mana >= maxMana) {
			firstFrameCanRegenMana = int.MaxValue;
		}
	}

	private void EnsureManaIsWithinBounds() {
		if (mana <= 0) {
			mana = 0;
		}

		if (mana > maxMana) {
			mana = maxMana;
		}
	}

}
