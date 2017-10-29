using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public delegate void VoidDelegate();
	public event VoidDelegate death_event;

	bool is_dead = false;

	public void takeHit(Arrow arrow) {
		death();
	}

	void death() {
		if (is_dead) {
			return;
		}

		is_dead = true;

		if (death_event != null) {
			death_event();
		}

		this.gameObject.SetActive(false);
	}

	public void reset() {
		is_dead = false;
	}
}
