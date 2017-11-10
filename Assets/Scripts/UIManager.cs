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
	public Image distanceTraveled;

	public static UIManager getUIManager() {
		return (UIManager) HushPuppy.safeFindComponent("GameController", "UIManager");
	}

	void Start() {
		distanceTraveled.DOFade(0f, 0f);
	}

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
			distanceTraveled.DOFade(0f, 0.5f);
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

	public void updateDistanceTraveled(float value) {
		if (distanceTraveled.fillAmount == 1f && value != 1f) {
			distanceTraveled.DOFade(0.3f, 1f);
		}

		if (value <= 0.01f) {
			value = 0f;

			if (distanceTraveled.fillAmount >= 0f) {
				distanceTraveled.DOFade(0f, 1f);
			}
		}

		distanceTraveled.fillAmount = value;
	}
}
