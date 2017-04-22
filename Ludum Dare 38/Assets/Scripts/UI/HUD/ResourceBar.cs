using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HUD element that shows the users' current health or mana levels.
public class ResourceBar : MonoBehaviour {
	private List<ResourceTick> ticks = new List<ResourceTick>();

	void Start() {
		ResourceTick[] foundTicks = GetComponentsInChildren<ResourceTick>();

		for (int i = 0; i < foundTicks.Length; i++) {
			ticks.Add(foundTicks[i]);
		}


		// Sort ticks by x position;
		ticks.Sort(delegate(ResourceTick a, ResourceTick b) {
			return a.transform.position.x.CompareTo(b.transform.position.x);
		});
	}

	public void SetValue(int value) {
		
		for (int i = 0; i < value; i++) {
			if (i < ticks.Count) {
				ticks[i].gameObject.SetActive(true);
			}
		} 

		for (int i = value; i < ticks.Count; i++) {
			ticks[i].gameObject.SetActive(false);
		} 
	} 

}
