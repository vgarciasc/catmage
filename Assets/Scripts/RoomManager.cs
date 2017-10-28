using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour {
	public TextMeshProUGUI enemiesText;

	List<Room> rooms = new List<Room>();

	void Start() {
		rooms.Clear();
		rooms.AddRange(GameObject.FindObjectsOfType<Room>());
		foreach (Room r in rooms) {
			r.death_event += UpdateEnemyCount;
		}

		SetCurrentRoom(rooms[0]);
	}

	void UpdateEnemyCount(int count) {
		enemiesText.text = "enemies: " + count;
	}
	
	public void SetCurrentRoom(Room room) {
		// print("current_room: " + rooms.IndexOf(room));
		room.RoomActive();
		UpdateEnemyCount(room.enemy_count);

		Camera.main.transform.localPosition = new Vector3(
			room.transform.localPosition.x,
			room.transform.localPosition.y,
			-10);
	}
}
