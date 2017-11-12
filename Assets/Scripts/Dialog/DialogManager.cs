using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogManager : MonoBehaviour {

	public TextAsset inkAsset;
	public TextMeshProUGUI dialogText;
	public Animator dialogAnim;
	public Image portraitLeft;
	public Image portraitRight;

	public bool dialog_active = false;

	public delegate void SetActiveDelegate(bool value);
	public event SetActiveDelegate set_active_event;

	bool skip_display = false;
	bool next_dialog = false;
	bool text_running = false;
	Story _story;
	string savedJson = "";

	public static DialogManager getDialogManager() {
		return (DialogManager) HushPuppy.safeFindComponent("GameController", "DialogManager");
	}

	void toggle(bool value) {
		dialog_active = value;
		if (set_active_event != null) {
			set_active_event(value);
		}
	}

	public void start(string dialogID, Sprite portrait) {
		handlePortraits(portrait);

		_story = new Story(inkAsset.text);
		if (savedJson != "") {
			_story.state.LoadJson(savedJson);
		}

		_story.ChoosePathString(dialogID);

		toggle(true);
		StartCoroutine(Text());
	}

	IEnumerator Text() {
		dialogAnim.gameObject.SetActive(true);
		dialogAnim.SetBool("active", true);

		while (_story.canContinue) {
			string str = _story.Continue();

			yield return Display_String(str, 2);
			dialogAnim.SetBool("idle_on", true);
			yield return new WaitUntil(() => (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Q)));
		}		
		
		savedJson = _story.state.ToJson();
		dialogAnim.SetBool("active", false);
		toggle(false);
	}
	
	IEnumerator Display_String(string text, int speed) {
		dialogAnim.SetBool("idle_on", false);
		int current_character = 0;
		text_running = true;

		for (current_character = 0; current_character < text.Length; current_character++) {
			if (current_character == text.Length ||
				skip_display) {
				break;
			}

			dialogText.text = text.Substring(0, current_character) + "<color=#0000>" + text.Substring(current_character) + "</color>";
			yield return HushPuppy.WaitForEndOfFrames(speed);
		}

		skip_display = false;
		text_running = false;
		dialogText.text = text;
	}

	void handlePortraits(Sprite portrait) {
		if (portrait == null) {
			portraitLeft.gameObject.SetActive(false);
			portraitRight.gameObject.SetActive(false);
			return;
		}

		portraitLeft.gameObject.SetActive(true);
		portraitRight.gameObject.SetActive(true);

		portraitRight.sprite = portrait;
	}
}
