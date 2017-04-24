using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTick : MonoBehaviour {
	private GameObject spriteContainer;

	void Start() {
		Transform t = transform.Find("Sprite container");
		if (t != null) {
			spriteContainer = t.gameObject;
		}
	}

	public void SetSpriteContainerScale(float f) {
		spriteContainer.transform.localScale = new Vector3(f, 1f, 1f);
	}
}
