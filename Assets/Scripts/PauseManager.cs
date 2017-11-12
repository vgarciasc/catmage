using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {

	public static PauseManager getPauseManager() {
		return (PauseManager) HushPuppy.safeFindComponent("GameController", "PauseManager");
	}

	List<Pausable> were_paused = new List<Pausable>();

	public void pause() {
		var rm = RoomManager.getRoomManager();

		foreach (MovingPlatform mp in rm.getCurrentRoom().GetComponentsInChildren<MovingPlatform>()) {
			if (!mp.is_paused) {
				were_paused.Add(mp);
				mp.pause();
			}
		}
	}

	public void unpause() {
		foreach (Pausable p in were_paused) {
			p.unpause();
		}

		were_paused.Clear();
	}
}
