using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour {
	public TextMeshProUGUI enemies_text;
	public GameObject arrow_container;
	public Image arrow_image;
	public Image foreground;

	public void updateEnemyCount(int count) {
		enemies_text.text = "enemies: <color=#EEEEEE>" + count +"</color>";
	}

	public void updateArrowCount(int count) {
		float duration = 0.2f;
		float size = 0.1f;

		if (count == 0) {
			arrow_image.DOFade(0.2f, duration);
			arrow_image.transform.DOScale(arrow_image.transform.localScale - Vector3.one * size, duration);
		}
		else {
			arrow_image.DOFade(1f, duration);
			arrow_image.transform.DOScale(arrow_image.transform.localScale + Vector3.one * size, duration);
		}
	}

	public IEnumerator reset() {
		foreground.gameObject.SetActive(true);

		yield return new WaitForSeconds(0.2f);

		foreground.gameObject.SetActive(false);
	}

	public void toggle(bool value) {
		enemies_text.gameObject.SetActive(value);
		arrow_container.gameObject.SetActive(value);
	}
}
