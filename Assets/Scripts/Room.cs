using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	public delegate void EnemyCountDelegate(int count);
	public event EnemyCountDelegate death_event;

	public int enemy_count = -1;

	public void RoomActive() {
		var enemies = this.GetComponentsInChildren<Enemy>();
		enemy_count = enemies.Length;
		foreach (Enemy e in enemies) {
			e.death_event += EnemyDeath;
		}
	}

	void EnemyDeath() {
		enemy_count -= 1;

		if (death_event != null) {
			death_event(enemy_count);
		}
	}

	public void SensorEntered() {
		RoomActive();
		GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<RoomManager>().SetCurrentRoom(this);
	}
}
