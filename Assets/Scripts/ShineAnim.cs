using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShineAnim : MonoBehaviour {

	public IEnumerator anim() {
		this.transform.localScale = Vector3.zero;
		this.transform.DOScale(Vector3.one * 2, 0.2f);
		this.transform.DORotate(new Vector3(0f, 0f, 180f), 0.5f);
		yield return new WaitForSeconds(0.3f);
		this.transform.DOScale(Vector3.zero, 0.2f);
	}
}
