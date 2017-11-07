using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour {

	Rigidbody2D rb;
	SpriteRenderer sr;
	Player player;
	Vector2 last_position;
	float distance = 25f;
	float last_change = 0f;
	bool is_stopped = false;
	bool is_recalling = false;

	public ParticleSystem recall_ps;
	public GameObject recall_circle;

	bool collided_this_frame = false;

	void Start() {
		rb = this.GetComponentInChildren<Rigidbody2D>();
		sr = this.GetComponentInChildren<SpriteRenderer>();
		player = HushPuppy.safeFindComponent("Player", "Player") as Player;
		last_position = transform.position;

		StartCoroutine(foo());
	}

	IEnumerator foo() {
		while (true) {
			var aux = LineOfShot.Get_Trajectory(this.transform.position, rb.velocity, 40);
			rb.velocity = (this.transform.position - (Vector3) aux[1]).normalized;

			yield return new WaitForSeconds(1f);
		}
	}

	void FixedUpdate() {
		collided_this_frame = false;

		var aux = LineOfShot.Get_Trajectory(this.transform.position, rb.velocity, 40);
		for (int i = 0; i < aux.Count - 1; i++) {
			Debug.DrawRay(aux[i], aux[i+1] - aux[i], Color.blue, Time.deltaTime);
		}

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

	Hashtable walltime_hash = new Hashtable();	
	void OnTriggerEnter2D(Collider2D collider) {
		return;
		GameObject target = collider.gameObject;

		if (target.tag == "Wall") {
			// if (walltime_hash.ContainsKey(target.name) && Time.time - (float) walltime_hash[target.name] < 0.05f) {
			// 	return;
			// }
			if (collided_this_frame) {
				return;
			}

			collided_this_frame = true;

			RaycastHit2D hit = Physics2D.Raycast(
				transform.position,
				rb.velocity,
				Mathf.Infinity
			);

			var aux = rb.velocity;

			float magnitude = rb.velocity.magnitude;
			rb.velocity = Vector2.Reflect(
				hit.point - (Vector2) transform.position,
				hit.normal
			).normalized * magnitude;

			// this.transform.position += (Vector3) rb.velocity * Time.deltaTime;

			// Debug.Break();
			print("collided with: " + target + "\n[(" + aux.x + ", " + aux.y + ") => (" + rb.velocity.x + ", " + rb.velocity.y + ")]");
			walltime_hash[target.name] = Time.time;

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

			if (distance <= 6f) {
				float magnitude = rb.velocity.magnitude;
				rb.velocity = rb.velocity.normalized * magnitude * distance / 6f;
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
}
