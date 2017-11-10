using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	class EnemyStartPos {
		public Enemy enemy;
		public Vector3 position;

		public EnemyStartPos(Enemy enemy, Vector3 pos) {
			this.position = pos;
			this.enemy = enemy;
		}
	}

	RoomManager roomManager;
	Player player;
	List<Enemy> enemies = new List<Enemy>();
	List<Door> doors = new List<Door>();

	bool room_active = false;
	int id = -1;

	public delegate void EnemyCountDelegate(int count);
	public event EnemyCountDelegate update_enemy_alive_event;

	public int enemy_count = 0;
	public bool shows_UI = true;
	public bool battle_possible = true;
	public bool only_perfect_elimination = true;

	//reset variables
	Vector3 player_start_position;
	List<EnemyStartPos> enemies_start = new List<EnemyStartPos>();

	void Start() {
		player = HushPuppy.safeFindComponent("Player", "Player") as Player;
		roomManager = HushPuppy.safeFindComponent("GameController", "RoomManager") as RoomManager;

		enemies = new List<Enemy>();
		enemies.AddRange(this.GetComponentsInChildren<Enemy>());
		foreach (Enemy e in enemies) {
			e.death_event += enemyDeath;
			enemies_start.Add(new EnemyStartPos(e, e.gameObject.transform.position));
		}
		enemy_count = enemies.Count;

		doors = new List<Door>();
		doors.AddRange(this.GetComponentsInChildren<Door>());

		if (room_active) {
			updateEnemyAliveCount(enemies.Count);
		}
	}

	public void setID(int id) {
		this.id = id;
	}

	public void setActive(bool value) {
		room_active = value;

		if (player == null) {
			player = HushPuppy.safeFindComponent("Player", "Player") as Player;
		}
		
		if (value) {
			updateEnemyAliveCount(enemy_count);
			player_start_position = player.transform.position;
		}
	}

	void updateEnemyAliveCount(int amount) {
		enemy_count = amount;

		if (update_enemy_alive_event != null) {
			update_enemy_alive_event(amount);
		}
	}

	void enemyDeath() {
		updateEnemyAliveCount(enemy_count - 1);
	}

	public void sensorEntered() {
		roomManager.setCurrentRoom(this);
	}

	public void reset() {
		player.reset(player_start_position);

		var arrows = GameObject.FindObjectsOfType<Arrow>();
		foreach (Arrow a in arrows) {
			a.destroy();
		}

		var switches = GameObject.FindObjectsOfType<Switch>();
		foreach (Switch s in switches) {
			s.reset();
		}

		var doors = GameObject.FindObjectsOfType<Door>();
		foreach (Door d in doors) {
			d.reset();
		}

		foreach (EnemyStartPos e in enemies_start) {
			e.enemy.gameObject.transform.position = e.position;
			e.enemy.gameObject.SetActive(true);
			e.enemy.reset();
		}

		updateEnemyAliveCount(enemies_start.Count);
	}

	public IEnumerator respawnEnemies() {
		updateEnemyAliveCount(enemies_start.Count);
		foreach (EnemyStartPos e in enemies_start) {
			if (!e.enemy.is_dead) {
				continue;
			}

			e.enemy.gameObject.transform.position = e.position;
			e.enemy.gameObject.SetActive(true);
			e.enemy.reset();
			StartCoroutine(e.enemy.spawnAnimation());
			yield return new WaitForSeconds(0.4f);
		}
	}

	public bool enemiesDead() {
		foreach (EnemyStartPos e in enemies_start) {
			if (e.enemy.gameObject.activeSelf) {
				return false;
			}
		}

		return true;
	}

	public void perfectElimination() {
		foreach (Door d in doors) {
			if (d.door_open) {
				continue;
			}

			switch (d.trigger) {
				case DoorActivationTrigger.ALL_ENEMY_PERFECT_ELIMINATION:
					d.openDoor();
					break;
			}
		}
	}

	public void openDoors() {
		foreach (Door d in doors) {
			if (d.door_open) {
				continue;
			}

			switch (d.trigger) {
				case DoorActivationTrigger.SWITCH:
					if (d.switch_obj.switch_on) {
						d.openDoor();
					}
					break;
				case DoorActivationTrigger.NO_ENEMIES:
					if (enemiesDead()) {
						d.openDoor();
					}
					break;
			}
		}
	}
}
