﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Hitbox {
	protected override bool IsValidTarget(IHittable hitObject) {
		// Note the difference between the explicit cast operator and the "as" keyword:
		// http://stackoverflow.com/questions/132445/direct-casting-vs-as-operator
		if ( (hitObject as Player) != null) {
			return true;
		}

		return false;
	}

}
