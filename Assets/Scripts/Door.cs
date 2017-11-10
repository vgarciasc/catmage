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

	public Switch switch_obj;

	void Start() {
		anim = this.GetComponentInChildren<Animator>();
	}

	public void openDoor() {
		anim.SetBool("open", true);
		door_open = true;
	}

	public void closeDoor() {
		anim.SetBool("open", false);
		door_open = false;
	}

	public void reset() {
		closeDoor();
	}
}
