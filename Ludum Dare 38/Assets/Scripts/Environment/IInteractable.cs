using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
	bool CanInteract();
	void StartSignaling();
	void StopSignaling();
	void Interact();
}
