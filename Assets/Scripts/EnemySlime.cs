using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : MonoBehaviour {

	Animator animator;
	Rigidbody2D rb;
	SpriteRenderer sr;

	void OnEnable() {
		animator = this.GetComponentInChildren<Animator>();
		rb = this.GetComponentInChildren<Rigidbody2D>();
		sr = this.GetComponentInChildren<SpriteRenderer>();

		StartCoroutine(Behaviour());
	}

	IEnumerator Behaviour() {
		while (true) {
			animator.SetTrigger("jump");
			yield return new WaitForSeconds(0.15f);
			
			Vector3 vec = Vector3.left;
			for (int k = 0; k < 50; k++) {
				vec = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0).normalized;
				RaycastHit2D hit = Physics2D.Raycast(
					this.transform.position,
					vec,
					3f
				);

				if (hit.collider == null) {
					break;
				}
			}
			
			rb.velocity = vec * 5;
			adjustFlip();
			yield return new WaitForSeconds(0.15f);
			rb.velocity = Vector3.zero;
			yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
		}
	}

	void adjustFlip() {
		if (rb.velocity.x > 0) {
			sr.flipX = true;
		}
		else {
			sr.flipX = false;
		}
	}
}
