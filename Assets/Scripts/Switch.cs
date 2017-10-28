using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

	public GameObject door;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void takeHit(Arrow arrow) {
		door.GetComponentInChildren<Animator>().SetTrigger("open");
	}
}
