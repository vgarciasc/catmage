using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour {
	public delegate void VoidDelegate();
	public event VoidDelegate death_event;

	public GameObject spriteSurrogate;
	public GameObject bloodSplatterPrefab;
	public GameObject shinePrefab;
	bool is_dead = false;

	public void takeHit(Arrow arrow) {
		arrow.pause();
		death(arrow);
	}

	IEnumerator deathAnimation(Arrow arrow) {
		if (spriteSurrogate == null) {
			yield break;
		}

		this.GetComponentInChildren<PolygonCollider2D>().enabled = false;

		SpecialCamera.getSpecialCamera().screenShake_(0.02f);
		var shine = Instantiate(shinePrefab, arrow.getTip(), Quaternion.identity);
		var scale_y = arrow.transform.localScale.y;
		yield return shine.GetComponentInChildren<ShineAnim>().anim();
		arrow.unpause();

		Color death_color = new Color(0.12f, 0.9f, 0.35f, 1f);
		SpriteRenderer sr_aux = spriteSurrogate.GetComponentInChildren<SpriteRenderer>();
		var original_scale = this.transform.localScale;
		spriteSurrogate.SetActive(true);

		sr_aux.color = new Color(1f, 1f, 1f, 0f);
		sr_aux.DOColor(death_color, 0.3f);
		this.transform.DOScaleY(original_scale.y * 1.3f, 0.2f);
		this.transform.DOScaleX(original_scale.x * 0.5f, 0.2f);

		yield return new WaitForSeconds(0.3f);

		this.GetComponentInChildren<SpriteRenderer>().enabled = false;
		spriteSurrogate.transform.DOScale(Vector3.zero, 0.2f);

		int dice = Random.Range(1, 3);
		for (int i = 0; i < dice; i++) {
			var blood = Instantiate(bloodSplatterPrefab,
				this.transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0f),
				Quaternion.identity
			);

			blood.transform.localScale = Vector2.zero;
			blood.transform.DOScale(original_scale * 2f * Random.Range(0.7f, 1.3f), 0.1f);
			blood.GetComponentInChildren<SpriteRenderer>().color = death_color;
			blood.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		}

		yield return new WaitForSeconds(0.2f);
		this.gameObject.SetActive(false);
	}

	void death(Arrow arrow) {
		if (is_dead) {
			return;
		}

		is_dead = true;

		if (death_event != null) {
			death_event();
		}

		StartCoroutine(deathAnimation(arrow));
	}

	public void reset() {
		is_dead = false;
	}
}
