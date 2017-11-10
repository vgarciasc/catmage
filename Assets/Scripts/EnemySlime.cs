using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : MonoBehaviour {

	Animator animator;

	void Start () {
		animator = this.GetComponentInChildren<Animator>();
		StartCoroutine(Behaviour());
	}

	IEnumerator Behaviour() {
		while (true) {
			animator.SetTrigger("jump");
			this.GetComponentInChildren<Rigidbody2D>().velocity = Vector3.left * 5;
			yield return new WaitForSeconds(2f);
			this.GetComponentInChildren<Rigidbody2D>().velocity = Vector3.zero;
		}
	}
}
