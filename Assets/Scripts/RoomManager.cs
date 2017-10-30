using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
	UIManager ui;
	Player player;

	List<Room> rooms = new List<Room>();
	Room currentRoom = null;

	public Room startRoom;

	void Start() {
		player = HushPuppy.safeFindComponent("Player", "Player") as Player;
		ui = HushPuppy.safeFindComponent("GameController", "UIManager") as UIManager;

		rooms.Clear();
		rooms.AddRange(GameObject.FindObjectsOfType<Room>());
		foreach (Room r in rooms) {
			r.update_enemy_alive_event += updateEnemyCount;
		}

		setCurrentRoom(startRoom);
	}

	public void reset() {
		currentRoom.reset();
		StartCoroutine(ui.reset());
	}

	void updateEnemyCount(int count) {
		ui.updateEnemyCount(count);
	}
	
	public void setCurrentRoom(Room room) {
		if (currentRoom != null) {
			currentRoom.setActive(false);
		}

		currentRoom = room;
		room.setActive(true);
		updateEnemyCount(room.enemy_count);

		Camera.main.transform.localPosition = new Vector3(
			room.transform.localPosition.x,
			room.transform.localPosition.y,
			-10);

		ui.toggle(room.shows_UI);
		player.blockBattle(room.battle_possible);
	}
}
