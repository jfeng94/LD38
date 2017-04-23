using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable {
	void InflictDamage(int damage, Vector2 direction, float strength);
}
