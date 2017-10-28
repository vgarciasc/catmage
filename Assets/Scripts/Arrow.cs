using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

	float lastChange = 0f;
	Rigidbody2D rb;
	Vector2 last_position;
	float distance = 20f;

	void Start() {
		rb = this.GetComponentInChildren<Rigidbody2D>();
	}

	void FixedUpdate() {
		handleMaxDistance();
	}

	void Update() {
		pointToDirection(rb.velocity);
	}

	void pointToDirection(Vector2 velocity) {
		if (velocity.magnitude != 0) {
			float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
		}
	}
	
	void OnTriggerEnter2D(Collider2D collider) {
		GameObject target = collider.gameObject;

		if (target.tag == "Wall" && Time.time - lastChange > 0.5f) {
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

			lastChange = Time.time;
		}

		if (target.tag == "Enemy") {
			target.GetComponentInChildren<Enemy>().takeHit(this);
		}

		print(target.tag);
		if (target.tag == "Switch") {
			target.GetComponentInChildren<Switch>().takeHit(this);
		}
	}

	void handleMaxDistance() {
		if (last_position.x != transform.position.x ||
			last_position.y != transform.position.y) {

			distance -= Vector2.Distance(
				last_position,
				transform.position
			);

			if (distance <= 0f) {
				Destroy(this.gameObject);
			}
		}

		last_position = transform.position;
	}
}
