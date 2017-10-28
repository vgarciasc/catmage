using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public delegate void VoidDelegate();
	public event VoidDelegate death_event;

	bool isDead = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void takeHit(Arrow arrow) {
		Death();
	}

	void Death() {
		if (isDead) {
			return;
		}

		isDead = true;

		if (death_event != null) {
			death_event();
		}

		Destroy(this.gameObject);
	}
}
