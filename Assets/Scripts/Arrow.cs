using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour {

	Rigidbody2D rb;
	SpriteRenderer sr;
	Player player;
	Vector2 last_position;
	float distance = 20f;
	float last_change = 0f;
	float speed = 5f;
	bool is_stopped = false;
	bool is_recalling = false;


	Vector2 last_velocity;
	LayerMask layer;

	public ParticleSystem recall_ps;
	public GameObject recall_circle;
	public GameObject tip;

	void Start() {
		rb = this.GetComponentInChildren<Rigidbody2D>();
		sr = this.GetComponentInChildren<SpriteRenderer>();
		player = HushPuppy.safeFindComponent("Player", "Player") as Player;
		last_position = transform.position;
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
			unpause();
		}

		pointToDirection(rb.velocity);
		handleRecall();
	}

	void OnTriggerEnter2D(Collider2D collider) {
		GameObject target = collider.gameObject;

		if (target.tag == "Enemy") {
			target.GetComponentInChildren<Enemy>().takeHit(this);
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
			rb.velocity = reflection.normalized;
		}

		var aux = LineOfShot.Get_Trajectory(this.transform.position, rb.velocity, 40);
		rb.velocity = ((Vector3) aux[1] - this.transform.position).normalized * speed;
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

			if (distance <= 1f) {
				float magnitude = rb.velocity.magnitude;
				rb.velocity = rb.velocity.normalized * magnitude * distance / 1f;
			}

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

	public void pause() {
		if (is_stopped) {
			return;
		}
		
		last_velocity = rb.velocity;
		rb.velocity = Vector2.zero;
		is_stopped = true;
		layer = this.gameObject.layer;
		this.gameObject.layer = LayerMask.NameToLayer("Default");
	}

	public void unpause() {
		rb.velocity = last_velocity;
		is_stopped = false;
		this.gameObject.layer = layer;
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

		this.gameObject.layer = LayerMask.NameToLayer("Default");
	}

	void handleRecall() {
		if (is_recalling) {
			Vector2 distance = (player.transform.position - this.transform.position);
			rb.velocity = (distance.normalized * 5f);
			rb.angularVelocity = 2500f / Mathf.Pow(distance.magnitude < 0.1f ? 0.1f : distance.magnitude, 1.2f);
		}
	}

	public Vector3 getTip() {
		return tip.transform.position;
	}
}
