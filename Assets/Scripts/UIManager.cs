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
	
	[Header("Combo UI")]
	public int currentCombo = 0;
	public List<Sprite> numbers = new List<Sprite>();
	public Image combo_text;
	public Image combo_number1;
	public Image combo_number2;

	public static UIManager getUIManager() {
		return (UIManager) HushPuppy.safeFindComponent("GameController", "UIManager");
	}

	void Start() {
		distanceTraveled.DOFade(0f, 0f);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.J)) {
			// StartCoroutine(updateCombo(currentCombo + 1));
		}

		handleCombo();
	}

	void handleCombo() {
		if (currentCombo > 0 && Time.time - last_combo_update > 1f && ending_combo == null) {
			ending_combo = StartCoroutine(endCombo());
		}
	}

	float last_combo_update = 0f;
	public IEnumerator updateCombo(int count) {
		var ended = ending_combo != null;
		if (ending_combo != null) {
			StopCoroutine(ending_combo);
			ending_combo = null;
		}

		last_combo_update = Time.time;
		currentCombo = count;

		foreach (Tweener t in combo_tweens) {
			if (t != null) {
				t.Kill();
			}
		}
		combo_tweens.Clear();

		int number_1 = count / 10;
		int number_2 = count % 10;

		combo_number1.sprite = numbers[number_1];
		combo_number2.sprite = numbers[number_2];

		var time = 0.3f;
		if (!ended) time = 0f;

		combo_tweens.Add(combo_number1.DOFade(1f, time));
		combo_tweens.Add(combo_number2.DOFade(1f, time));
		combo_tweens.Add(combo_text.DOFade(1f, time));

		var tween_1 = combo_number1.transform.DOScale(combo_number1.transform.localScale * 1.4f, time);
		tween_1.SetEase(Ease.InBack);
		var tween_2 = combo_number2.transform.DOScale(combo_number2.transform.localScale * 1.4f, time);
		tween_2.SetEase(Ease.InBack);
		var tween_3 = combo_text.transform.DOScale(combo_text.transform.localScale * 1.4f, time);
		tween_3.SetEase(Ease.InBack);

		combo_tweens.Add(tween_1);
		combo_tweens.Add(tween_2);
		combo_tweens.Add(tween_3);

		yield return new WaitForSeconds(time);

		var tween_1_2 = combo_number1.transform.DOScale(combo_number1.transform.localScale / 1.4f, 0.3f);
		tween_1_2.SetEase(Ease.InBack);
		var tween_2_2 = combo_number2.transform.DOScale(combo_number2.transform.localScale / 1.4f, 0.3f);
		tween_2_2.SetEase(Ease.InBack);
		var tween_3_2 = combo_text.transform.DOScale(combo_text.transform.localScale / 1.4f, 0.3f);
		tween_3_2.SetEase(Ease.InBack);

		combo_tweens.Add(tween_1_2);
		combo_tweens.Add(tween_2_2);
		combo_tweens.Add(tween_3_2);
	}

	List<Tweener> combo_tweens = new List<Tweener>();
	Coroutine ending_combo = null;
	IEnumerator endCombo() {
		combo_tweens.Add(combo_number1.DOFade(0f, 3f));
		combo_tweens.Add(combo_number2.DOFade(0f, 3f));
		combo_tweens.Add(combo_text.DOFade(0f, 3f));

		yield return new WaitForSeconds(4f);

		currentCombo = 0;
		ending_combo = null;
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
