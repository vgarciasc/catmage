using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Switch {

	public Arrow arrow;
	public SpriteRenderer sprite;
	public Sprite sprite_off;
	public Sprite sprite_on;
	float timeout = 1f;
	float last_press = 0f;
	public bool forever_on = true;

	void OnTriggerStay2D(Collider2D collider) {
		GameObject target = collider.gameObject;
		Arrow arrow = target.GetComponentInChildren<Arrow>();

		if (arrow != null && arrow.is_stopped) {
			this.arrow = arrow;
			toggle(true);
			last_press = Time.time;
		}
	}

	void Update() {
		if (Time.time - last_press > timeout && arrow == null) {
			toggle(false);
		}
	}

	public new void takeHit(Arrow arrow) {
		toggle(true);
	}

	void toggle(bool value) {
		if (switch_on && !value && forever_on) {
			return;
		}

		switch_on = value;
		updateEvents();

		if (switch_on) {
			sprite.sprite = sprite_on;
		}
		else {
			sprite.sprite = sprite_off;
		}
	}
}
