using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpecialCamera : MonoBehaviour {

	public RoomManager roomManager;
	
	void Update () {
		#if (UNITY_EDITOR)
		if (roomManager.startRoom != null) {
			this.transform.localPosition = new Vector3(
				roomManager.startRoom.transform.localPosition.x,
				roomManager.startRoom.transform.localPosition.y,
				-10);
		}
		#endif
	}
}
