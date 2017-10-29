using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour {

	Rigidbody2D rb;
	SpriteRenderer sr;
	Player player;
	Vector2 last_position;
	float distance = 5f;
	float last_change = 0f;
	bool is_stopped = false;
	bool is_recalling = false;

	public ParticleSystem recall_ps;
	public GameObject recall_circle;

	void Start() {
		rb = this.GetComponentInChildren<Rigidbody2D>();
		sr = this.GetComponentInChildren<SpriteRenderer>();
		player = HushPuppy.safeFindComponent("Player", "Player") as Player;
		last_position = transform.position;
	}

	void FixedUpdate() {
		handleMaxDistance();
	}

	void Update() {
		pointToDirection(rb.velocity);
		handleRecall();
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
	
	void OnTriggerEnter2D(Collider2D collider) {
		GameObject target = collider.gameObject;

		if (target.tag == "Wall" && Time.time - last_change > 0.1f) {
			RaycastHit2D hit = Physics2D.Raycast(
				transform.position,
				rb.velocity,
				Mathf.Infinity
			);

			float magnitude = rb.velocity.magnitude;
			rb.velocity = Vector2.Reflect(
				hit.point - (Vector2) transform.position,
				hit.normal
			).normalized * magnitude;

			last_change = Time.time;
		}

		if (target.tag == "Enemy") {
			target.GetComponentInChildren<Enemy>().takeHit(this);
		}

		if (target.tag == "Switch") {
			target.GetComponentInChildren<Switch>().takeHit(this);
		}
	}

	void handleMaxDistance() {
		if (is_recalling) {
			return;
		}

		if (last_position.x != transform.position.x ||
			last_position.y != transform.position.y) {

			distance -= Vector2.Distance(
				last_position,
				transform.position
			);

			if (distance <= 0f) {
				stop();
			}
		}

		last_position = transform.position;
	}

	void stop() {
		rb.velocity = Vector2.zero;
		is_stopped = true;
		this.gameObject.layer = LayerMask.NameToLayer("Default");
	}

	public void destroy() {
		Destroy(this.gameObject);
	}

	IEnumerator destroy_() {
		rb.velocity = Vector2.zero;
		sr.enabled = false;
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
	}

	void handleRecall() {
		if (is_recalling) {
			Vector2 distance = (player.transform.position - this.transform.position);
			rb.velocity = (distance.normalized * 5f);
			rb.angularVelocity = 2500f / Mathf.Pow(distance.magnitude, 1.2f);
		}
	}
}
