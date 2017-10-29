using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public GameObject arrow_prefab;
	public GameObject animation_obj;

	int arrow_count = 1;
	bool block_walk = false;
	UIManager ui;

	Animator animator;
	Rigidbody2D rb;
	SpriteRenderer sr;
	RoomManager roomManager;

	void Start () {
		animator = this.GetComponentInChildren<Animator>();		
		rb = this.GetComponentInChildren<Rigidbody2D>();
		sr = this.GetComponentInChildren<SpriteRenderer>();

		ui = HushPuppy.safeFindComponent("GameController", "UIManager") as UIManager;
		roomManager = HushPuppy.safeFindComponent("GameController", "RoomManager") as RoomManager;
	}
	
	void Update () {
		handleMovement();
		handleShot();
		handleReset();
		handleArrowRecall();
	}

	void handleMovement() {
		if (block_walk) {
			return;
		}

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
			animation_obj.transform.localScale *= -1;
		}
		else if (!sr.flipX && x == -1) {
			sr.flipX = true;
			animation_obj.transform.localScale *= -1;
		}

		animator.SetBool("walking", x != 0 || y != 0);
		rb.velocity = new Vector2(x, y) * speed;
	}

	void handleShot() {
		if (Input.GetButtonUp("Fire1") && arrow_count > 0) {
			Vector2 velocity = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
			GameObject arrow = Instantiate(arrow_prefab, this.transform.position, Quaternion.identity);
			arrow.GetComponentInChildren<Rigidbody2D>().velocity = velocity.normalized * 5f;
			updateArrow(arrow_count - 1);
		}
	}

	void handleReset() {
		if (Input.GetKeyDown(KeyCode.R)) {
			reset_animation();
		}
	}

	void handleArrowRecall() {
		if (Input.GetKeyDown(KeyCode.E)) {
			var arrows = FindObjectsOfType<Arrow>();
			foreach (Arrow a in arrows) {
				a.recall();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		GameObject target = collider.gameObject;
		if (target.tag == "Arrow") {
			target.GetComponentInChildren<Arrow>().destroy();
			updateArrow(arrow_count + 1);
		}
	}

	void updateArrow(int amount) {
		arrow_count = amount;
		ui.updateArrowCount(arrow_count);
	}

	void reset_animation() {
		animator.SetTrigger("hand_movement");
	}

	public void reset(Vector3 position) {
		// this.transform.position = position;
		updateArrow(1);
	}

	void AnimBlockWalk(int value) {
		block_walk = value == 1;

		if (block_walk) {
			rb.velocity = Vector3.zero;
		}
	}

	void AnimResetEnd() {
		roomManager.reset();
	}
}
