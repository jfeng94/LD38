using UnityEngine;

// Controls how the player and camera wrap around scenes
public class WarpObject : MonoBehaviour {
	public GameObject editorDebugDisplay;

	void Start() {
		if (editorDebugDisplay != null) {
			editorDebugDisplay.SetActive(false);
		}
	}
}