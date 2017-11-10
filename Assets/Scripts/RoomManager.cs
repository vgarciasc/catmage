using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
	UIManager ui;
	Player player;

	List<Room> rooms = new List<Room>();
	Room currentRoom = null;

	public Room startRoom;

	public static RoomManager getRoomManager() {
		return (RoomManager) HushPuppy.safeFindComponent("GameController", "RoomManager");
	}

	void Start() {
		player = HushPuppy.safeFindComponent("Player", "Player") as Player;
		ui = HushPuppy.safeFindComponent("GameController", "UIManager") as UIManager;

		rooms.Clear();
		rooms.AddRange(GameObject.FindObjectsOfType<Room>());
		foreach (Room r in rooms) {
			r.setID(rooms.IndexOf(r));
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
			if (currentRoom == room) {
				return;
			}

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
		player.blockBattle(!room.battle_possible);
	}

	public void arrowStopped(Arrow arrow) {
		if (currentRoom.enemiesDead()) {
			currentRoom.perfectElimination();
		}
		else {
			if (currentRoom.only_perfect_elimination) {
				StartCoroutine(currentRoom.respawnEnemies());
			}
		}
	}
}
