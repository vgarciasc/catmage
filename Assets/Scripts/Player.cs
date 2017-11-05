using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public GameObject arrow_prefab;
	public GameObject animation_obj;
	public LineOfShot line;

	int arrow_count = 1;
	bool block_walk = false;
	bool block_recall = false;
	bool block_shooting = false;
	bool just_ended_text = false;

	Animator animator;
	Rigidbody2D rb;
	SpriteRenderer sr;

	UIManager ui;
	RoomManager roomManager;
	DialogManager dialogManager;

	void Start () {
		animator = this.GetComponentInChildren<Animator>();		
		rb = this.GetComponentInChildren<Rigidbody2D>();
		sr = this.GetComponentInChildren<SpriteRenderer>();

		ui = HushPuppy.safeFindComponent("GameController", "UIManager") as UIManager;
		roomManager = HushPuppy.safeFindComponent("GameController", "RoomManager") as RoomManager;
		dialogManager = DialogManager.getDialogManager();

		dialogManager.set_active_event += reactToText;
	}

	void Update () {
		handleMovement();
		handleShot();
		handleLineOfShot();
		handleReset();
		handleArrowRecall();
		handlePause();
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
		if (arrow_count <= 0 || block_shooting) {
			return;
		}
		
		if (Input.GetButtonUp("Fire1")) {
			if (just_ended_text) {
				just_ended_text = false;
				return;
			}

			Vector2 velocity = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
			GameObject arrow = Instantiate(arrow_prefab, this.transform.position, Quaternion.identity);
			arrow.GetComponentInChildren<Rigidbody2D>().velocity = velocity.normalized * 5f;
			updateArrow(arrow_count - 1);
		}
	}

	void handleLineOfShot() {
		if (block_shooting) {
			return;
		}

		if (Input.GetMouseButton(0)) {
			if (just_ended_text) {
				return;
			}

			line.Set_Line(
				LineOfShot.Get_Trajectory(this.transform.position,
				Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position,
				30f
			));
		}
		if (Input.GetMouseButtonUp(0)) {
			line.Deactivate();
		}
	}

	void handleReset() {
		if (Input.GetKeyDown(KeyCode.R)) {
			reset_animation();
		}
	}

	void handleArrowRecall() {
		if (Input.GetKeyDown(KeyCode.E) && !block_recall) {
			var arrows = FindObjectsOfType<Arrow>();
			foreach (Arrow a in arrows) {
				a.recall();
			}
		}
	}

	bool isPausing = false;
	float pause = 1f;
	void handlePause() {
		if (Input.GetKeyDown(KeyCode.F)) {
			isPausing = true;
		}

		if (Input.GetKeyUp(KeyCode.F)) {
			isPausing = false;
		}

		if (isPausing) {
			pause -= Time.deltaTime * 0.1f;
		}

		if (pause <= 0f) {
			pause = 0f;
		}
		// ui.set_pause(pause);
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
		updateArrow(1);
	}

	public void blockBattle(bool value) {
		blockShooting(value);
	}

	void blockShooting(bool value) {
		block_shooting = value;
	}

	void blockWalk(bool value) {
		block_walk = value;
		if (block_walk) {
			rb.velocity = Vector3.zero;
			animator.SetBool("walking", false);
		}
	}

	void reactToText(bool value) {
		blockWalk(value);
		blockShooting(value);

		if (!value) {
			just_ended_text = true;
		}
	}

	void AnimBlockWalk(int value) {
		var val = value == 1;
		block_recall = val;
		blockWalk(val);
	}

	void AnimResetEnd() {
		roomManager.reset();
	}
}
