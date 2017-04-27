using UnityEngine;
using System.Collections.Generic;

// Specifies a path in the scene.
// Construct one by placing one in scene and giving it PathNodes as children.
// Note that ordering in the nodes list is determined by the scene hierarchy!
public class MovementPath : MonoBehaviour {
	private List<PathNode> nodes = new List<PathNode>();

	void Start() {
		PathNode[] pathNodes = GetComponentsInChildren<PathNode>();
		for (int i = 0; i < pathNodes.Length; i++) {
			if ( ! nodes.Contains(pathNodes[i]) ) {
				nodes.Add(pathNodes[i]);
			}
		}
	}

	public int GetNumNodes() { return nodes.Count; }

	public PathNode GetNodeAtIndex(int i) {
		// If there are no nodes, we can't do anything other than return null...
		if (nodes.Count == 0) {
			return null;
		}

		// Let's make sure this function is safe for any integer
		int fixedIndex = ((i % nodes.Count) + nodes.Count) % nodes.Count;

		return nodes[fixedIndex];
	} 
}