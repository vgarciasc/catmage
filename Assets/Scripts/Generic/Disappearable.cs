using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Disappearable : MonoBehaviour {

	public bool is_off = false;
	public bool once_off_off_forever = true;
	public List<Switch> switch_obj = new List<Switch>();

	public void updateSwitch() {
		foreach (Switch s in switch_obj) {
			if (!s.switch_on) {
				reappear();
				return;
			}
		}

		disappear();
	}

	public void disappear() {
		is_off = true;
		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
			sr.DOFade(0f, 0.5f);
		}
		foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) {
			c.enabled = false;
		}
	}

	public void reappear() {
		if (is_off && once_off_off_forever) {
			return;
		}

		is_off = false;
		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
			sr.DOFade(1f, 0.5f);
		}
		foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) {
			c.enabled = true;
		}
	}
}
