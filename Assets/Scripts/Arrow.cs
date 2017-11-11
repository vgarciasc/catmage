using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour {

	UIManager ui;
	Rigidbody2D rb;
	Player player;
	Vector2 last_position;
	float max_distance = 20f;
	float distance;
	float last_change = 0f;
	float speed = 5f;
	bool is_stopped = false;
	bool is_paused = false;
	bool is_recalling = false;
	Color outline_color;

	Vector2 last_velocity;
	LayerMask layer;

	public ParticleSystem recall_ps;
	public SpriteRenderer sr_outline;
	public SpriteRenderer sr_fill;
	public GameObject recall_circle;
	public GameObject tip;
	public GameObject arrow_capture;

	void Start() {
		rb = this.GetComponentInChildren<Rigidbody2D>();
		ui = UIManager.getUIManager();
		outline_color = sr_outline.color;

		player = HushPuppy.safeFindComponent("Player", "Player") as Player;
		last_position = transform.position;
		distance = max_distance;
	}

	void FixedUpdate() {
		handleMovement();
		handleMaxDistance();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.P)) {
			pause();
		}
		else if (Input.GetKeyDown(KeyCode.U)) {
			unpause(true, false);
		}
		else if (Input.GetKeyDown(KeyCode.S)) {
			stop();
		}
		else if (Input.GetKeyDown(KeyCode.B)) {
			startBoost(10f, 1f);
		}

		pointToDirection(rb.velocity);
		handleRecall();
	}

	void OnTriggerEnter2D(Collider2D collider) {
		GameObject target = collider.gameObject;

		if (target.tag == "Enemy") {
			target.GetComponentInChildren<Enemy>().takeHit(this);
		}

		SpinningBall spin = target.GetComponentInChildren<SpinningBall>();
		if (spin != null && !is_recalling) {
			spin.storeArrow(this);
			stop();
		}

		if (target.tag == "Wall_Leaves" && !is_recalling) {
			distance = 0f;
		}
	}

	void pointToDirection(Vector2 velocity) {
		if (is_recalling) {
			return;
		}

		if (velocity.magnitude != 0) {
			float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
		}
	}

	void handleMovement() {
		RaycastHit2D hit = Physics2D.Raycast(
			transform.position,
			rb.velocity,
			0.3f,
			(1 << LayerMask.NameToLayer("Ricochettable"))
		);

		Vector2 reflection;
		if (hit.collider != null) {
			reflection = Vector2.Reflect(
				(Vector3) hit.point - transform.position,
				hit.normal
			);
			rb.velocity = reflection.normalized * rb.velocity.magnitude;
		}

		var aux = LineOfShot.Get_Trajectory(this.transform.position, rb.velocity, 40);
		rb.velocity = ((Vector3) aux[1] - this.transform.position).normalized * rb.velocity.magnitude;
	}

	void handleMaxDistance() {
		ui.updateDistanceTraveled(distance / max_distance);

		if (is_recalling) {
			return;
		}

		if (last_position.x != transform.position.x ||
			last_position.y != transform.position.y) {

			distance -= Vector2.Distance(
				last_position,
				transform.position
			);

			if (distance <= 1f) {
				float magnitude = rb.velocity.magnitude;
				rb.velocity = rb.velocity.normalized * magnitude * distance / 1f;
			}

			if (distance <= 0.1f) {
				stop();
			}
		}

		last_position = transform.position;
	}

	Vector2 orig_boost_velocity;
	float current_boost;
	Coroutine boosting;
	Tweener boost_color_tween;
	Tweener boost_scale_tween;
	public void startBoost(float value, float duration) {
		boosting = StartCoroutine(boostArrow(value, duration));
	}

	IEnumerator boostArrow(float value, float duration) {
		current_boost = value / 3f;
		boost_color_tween = sr_outline.DOColor(new Color(0.12f, 0.9f, 0.35f, 1f), 0.4f);
		boost_scale_tween = this.transform.DOScaleY(this.transform.localScale.y * 0.75f, 0.3f);

		// print ("rb_velocity A1: " + rb.velocity);
		orig_boost_velocity = rb.velocity;
		rb.velocity = rb.velocity + rb.velocity.normalized * current_boost;
		// print ("rb_velocity A2: " + rb.velocity);

		yield return new WaitForSeconds(duration);
		endBoost();
	}

	public void endBoost() {
		if (boosting != null) {
			if (boost_color_tween != null) {
				boost_color_tween.Kill();
			}
			if (boost_scale_tween != null) {
				boost_scale_tween.Kill();
			}

			this.transform.DOScaleY(this.transform.localScale.y / 0.75f, 0.1f);
			sr_outline.DOColor(outline_color, 0.1f);

			StopCoroutine(boosting);
			boosting = null;

			// print ("rb.velocity B1: " + rb.velocity);
			// print ("rb.velocity.magnitude: " + rb.velocity.magnitude);
			// print ("(orig_boost_velocity + orig_boost_velocity.normalized * current_boost).magnitude: " + (orig_boost_velocity + orig_boost_velocity.normalized * current_boost).magnitude);
			if (Mathf.Abs(rb.velocity.magnitude - (orig_boost_velocity + orig_boost_velocity.normalized * current_boost).magnitude) < 0.1f) {
				rb.velocity = rb.velocity.normalized * speed;
				// print ("rb.velocity B2: " + rb.velocity);
			}
		}
	}

	void stop() {
		rb.velocity = Vector2.zero;
		is_stopped = true;
		endBoost();
		this.gameObject.layer = LayerMask.NameToLayer("Default");
		RoomManager.getRoomManager().arrowStopped(this);
		arrow_capture.SetActive(true);
	}

	public void pause() {
		if (is_paused) {
			return;
		}
		
		last_velocity = rb.velocity;
		rb.velocity = Vector2.zero;
		rb.angularVelocity = 0f;
		is_paused = true;
		endBoost();
		layer = this.gameObject.layer;
		this.gameObject.layer = LayerMask.NameToLayer("Default");
	}

	public void unpause(bool restore_velocity, bool reset_distance) {
		if (restore_velocity) {
			rb.velocity = last_velocity;
		}
		if (reset_distance) {
			distance = max_distance;
		}

		is_paused = false;
		arrow_capture.SetActive(false);
		this.gameObject.layer = layer;
	}

	public void destroy() {
		stop();
		Destroy(this.gameObject);
	}

	IEnumerator destroy_() {
		rb.velocity = Vector2.zero;
		sr_outline.enabled = false;
		sr_fill.enabled = false;
		recall_ps.Stop();
		yield return new WaitForSeconds(recall_ps.main.duration);
		Destroy(this.gameObject);
	}

	public void recall() {
		is_recalling = true;

		recall_circle.SetActive(true);
		var original_scale = recall_circle.transform.localScale;
		recall_circle.transform.localScale = Vector3.zero;
		var tween = recall_circle.transform.DOScale(original_scale, 0.1f);
		tween.SetEase(Ease.OutCubic);

		// this.gameObject.layer = LayerMask.NameToLayer("Default");
		arrow_capture.SetActive(true);
	}

	void handleRecall() {
		if (is_recalling && !is_paused) {
			Vector2 distance = (player.transform.position - this.transform.position);
			rb.velocity = (distance.normalized * 5f);
			rb.angularVelocity = 2500f / Mathf.Pow(distance.magnitude < 0.1f ? 0.1f : distance.magnitude, 1.2f);
		}
	}

	public Vector3 getTip() {
		return tip.transform.position;
	}
}
