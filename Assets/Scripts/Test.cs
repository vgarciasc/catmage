using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		this.transform.position = new Vector3(
			this.transform.position.x - 2.75f,
			this.transform.position.y,
			0
		);
	}
}
