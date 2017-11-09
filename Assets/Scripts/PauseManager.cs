using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {

	public static PauseManager getPauseManager() {
		return (PauseManager) HushPuppy.safeFindComponent("GameController", "PauseManager");
	}
}
