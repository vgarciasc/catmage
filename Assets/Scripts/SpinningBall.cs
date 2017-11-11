using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBall : MonoBehaviour {

	public float currentAngle = 0f;
	public float spinningSpeed = 1f;
	public GameObject direction;
	public Arrow holdingArrow;

	void Update () {
		currentAngle = (this.currentAngle + Time.deltaTime * spinningSpeed * 100f) % 360f;
		direction.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

		if (holdingArrow != null) {
			holdingArrow.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
			if (Input.GetButtonDown("Fire1")) {
				releaseArrow();
			}
		}
	}

	public void storeArrow(Arrow arrow) {
		holdingArrow = arrow;
		holdingArrow.pause();
		arrow.transform.position = this.transform.position;
	}

	public void releaseArrow() {
		var rb = holdingArrow.GetComponentInChildren<Rigidbody2D>();
		rb.velocity = new Vector3(
			Mathf.Cos(Mathf.Deg2Rad * (currentAngle + 90)),
			Mathf.Sin(Mathf.Deg2Rad * (currentAngle + 90)) 
		) * 5f;

		holdingArrow.unpause(false, true);
		holdingArrow = null;
	}
}
