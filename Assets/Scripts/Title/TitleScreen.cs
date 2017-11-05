using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleScreen : MonoBehaviour {

	public Image whitescreen;
	public TextMeshProUGUI press_Q;

	void Start () {
		StartCoroutine(Text());		
	}
	
	bool already_pressed = false;
	void Update () {
		if (Input.GetKeyDown(KeyCode.Q) && !already_pressed) {
			already_pressed = true;
			StartCoroutine(Exit());
		}		
	}

	IEnumerator Text() {
		yield return new WaitForSeconds(2f);
		press_Q.DOColor(Color.black, 0.5f);
	}

	IEnumerator Exit() {
		whitescreen.DOColor(Color.white, 2f);
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene("Main");
	}
}
