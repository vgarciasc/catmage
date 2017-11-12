using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

	public GameObject shinePrefab;
	public Door door;
	public MovingPlatform platform;

	Animator anim;
	public bool switch_on = false;
	public bool perfect_strike = false;

	void Start() {
		anim = this.GetComponentInChildren<Animator>();
	}

	void toggleSwitch() {
		toggleSwitch(!switch_on);
	}

	void toggleSwitch(bool value) {
		switch_on = value;
		anim.SetBool("switch", switch_on);

		if (door != null) {
			door.updateSwitch();
		}
		if (platform != null) {
			if (value) {
				platform.pause();
			} else {
				platform.unpause();
			}
		}
	}

	IEnumerator hitAnimation(Arrow arrow) {
		this.GetComponent<BoxCollider2D>().enabled = false;
		
		arrow.pause();

		var shine = Instantiate(shinePrefab, arrow.getTip(), Quaternion.identity);
		var scale_y = arrow.transform.localScale.y;
		
		yield return shine.GetComponentInChildren<ShineAnim>().anim();
		
		SpecialCamera.getSpecialCamera().screenShake_(0.02f);
		arrow.unpause(true, true);
		arrow.startBoost(10f, 0.3f);

		toggleSwitch();

		yield return new WaitForSeconds(1f);
		
		this.GetComponent<BoxCollider2D>().enabled = true;
	}

	public void takeHit(Arrow arrow) {
		StartCoroutine(hitAnimation(arrow));
	}

	public void reset() {
		if (switch_on) {
			toggleSwitch(false);
		}
	}

	public void arrowStopped() {
		if (perfect_strike) {
			toggleSwitch(false);
		}
	}
}
