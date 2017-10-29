using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorActivationTrigger {
	SWITCH,
	NO_ENEMIES
}

public class Door : MonoBehaviour {

	Animator anim;
	bool door_open = false;

	[Header("Activation Trigger")]
	public DoorActivationTrigger trigger;

	public Switch switch_obj;

	void Start () {
		anim = this.GetComponentInChildren<Animator>();

		if (trigger == DoorActivationTrigger.SWITCH) {
			switch_obj.hit_event += toggleDoor;
		}
		else if (trigger == DoorActivationTrigger.NO_ENEMIES) {
			this.GetComponentInParent<Room>().update_enemy_alive_event += updateEnemyCount;
		}
	}

	void updateEnemyCount(int count) {
		if (trigger == DoorActivationTrigger.NO_ENEMIES) {
			if (count == 0) {
				openDoor();
			}
			else {
				closeDoor();
			}
		}
	}

	void toggleDoor() {
		if (door_open) {
			closeDoor();
		}
		else {
			openDoor();
		}

		door_open = !door_open;
	}

	void openDoor() {
		anim.SetBool("open", true);
	}

	void closeDoor() {
		anim.SetBool("open", false);
	}
}
