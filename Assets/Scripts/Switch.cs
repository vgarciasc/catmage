using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

	public delegate void VoidDelegate();
	public event VoidDelegate hit_event;

	Animator anim;
	public bool switch_on = false;

	void Start() {
		anim = this.GetComponentInChildren<Animator>();
	}

	void toggleSwitch() {
		switch_on = !switch_on;
		anim.SetBool("switch", switch_on);
	}

	public void takeHit(Arrow arrow) {
		toggleSwitch();

		if (hit_event != null) {
			hit_event();
		}
	}
}
