using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public GameObject arrowPrefab;

	Animator animator;
	Rigidbody2D rb;
	SpriteRenderer sr;

	void Start () {
		animator = this.GetComponentInChildren<Animator>();		
		rb = this.GetComponentInChildren<Rigidbody2D>();
		sr = this.GetComponentInChildren<SpriteRenderer>();
	}
	
	void Update () {
		handleMovement();
		handleShot();

		if (Input.GetKeyDown(KeyCode.R)) {
			animator.SetTrigger("hand_movement");
		}
	}

	void handleMovement() {
		int x = 0;
		int y = 0;
		float speed = 4;
		
		if (Input.GetKey(KeyCode.LeftArrow)) {
			x = -1;
		}
		else if (Input.GetKey(KeyCode.RightArrow)) {
			x = 1;
		}
		if (Input.GetKey(KeyCode.UpArrow)) {
			y = 1;
		}
		else if (Input.GetKey(KeyCode.DownArrow)) {
			y = -1;
		}

		if (sr.flipX && x == 1) {
			sr.flipX = false;
		}
		else if (!sr.flipX && x == -1) {
			sr.flipX = true;
		}

		animator.SetBool("walking", x != 0 || y != 0);
		rb.velocity = new Vector2(x, y) * speed;
	}

	void handleShot() {
		if (Input.GetButtonUp("Fire1")) {
			Vector2 velocity = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
			GameObject arrow = Instantiate(arrowPrefab, this.transform.position, Quaternion.identity);
			arrow.GetComponentInChildren<Rigidbody2D>().velocity = velocity.normalized * 5f;
		}
	}
}
