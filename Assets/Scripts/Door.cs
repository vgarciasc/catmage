using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorActivationTrigger {
	SWITCH,
	ALL_ENEMY_PERFECT_ELIMINATION,
	NO_ENEMIES
}

public class Door : MonoBehaviour {
	Animator anim;
	public bool door_open = false;

	[Header("Activation Trigger")]
	public DoorActivationTrigger trigger;
	public List<Switch> switch_obj = new List<Switch>();

	void Start() {
		anim = this.GetComponentInChildren<Animator>();
	}

	public void openDoor() {
		if (door_open) {
			return;
		}

		anim.SetBool("open", true);
		door_open = true;
	}

	public void closeDoor() {
		if (!door_open) {
			return;
		}

		anim.SetBool("open", false);
		door_open = false;
	}

	public void reset() {
		updateSwitch();
	}

	public void updateSwitch() {
		foreach (Switch s in switch_obj) {
			if (!s.switch_on) {
				closeDoor();
				return;
			}
		}

		openDoor();
	}
}
