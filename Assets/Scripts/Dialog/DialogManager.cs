using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour {

	public TextMeshProUGUI dialogText;
	public Animator dialogAnim;

	public delegate void SetActiveDelegate(bool value);
	public event SetActiveDelegate set_active_event;

	bool skip_display = false;
	bool next_dialog = false;
	bool text_running = false;

	public bool dialog_active = false;

	public static DialogManager getDialogManager() {
		return (DialogManager) HushPuppy.safeFindComponent("GameController", "DialogManager");
	}

	void toggle(bool value) {
		dialog_active = value;
		if (set_active_event != null) {
			set_active_event(value);
		}
	}

	public void start() {
		toggle(true);
		StartCoroutine(Text());
	}

	IEnumerator Text() {
		dialogAnim.gameObject.SetActive(true);
		dialogAnim.SetBool("active", true);
		List<string> aux = new List<string> {
			"The boss will see you...",
			"...eventually!"
		};

		for (int i = 0; i < aux.Count; i++) {
			yield return Display_String(aux[i], 3);
			dialogAnim.SetBool("idle_on", true);
			yield return new WaitUntil(() => Input.GetButtonDown("Fire1"));
		}
		
		dialogAnim.SetBool("active", false);
		toggle(false);
	}
	
	IEnumerator Display_String(string text, int speed) {
		dialogAnim.SetBool("idle_on", false);
		int current_character = 0;
		text_running = true;

		while (true) {
			if (current_character == text.Length ||
				skip_display) {
				break;
			}

			dialogText.text = text.Substring(0, current_character++);
			// audioManager.Play();
			yield return HushPuppy.WaitForEndOfFrames(speed);
		}

		skip_display = false;
		text_running = false;
		dialogText.text = text;
	}
}
