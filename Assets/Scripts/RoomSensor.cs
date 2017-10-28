using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSensor : MonoBehaviour {
	public Room room;

	void OnTriggerEnter2D(Collider2D collision) {
		GameObject target = collision.gameObject;
		if (target.tag == "Player") {
			room.SensorEntered();
		}
	}
}
