using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour, Pausable {

	public Transform moved;
	public List<Transform> path = new List<Transform>();
	List<Vector3> path_vec = new List<Vector3>();
	float speed = 3f;
	Tweener currentTween;
	int currentPoint = 0;
	int nextPoint = 1;
	float duration_of_step;
	public Ease ease = Ease.Linear;
	public bool is_paused = false;

	void Start() {
		foreach (Transform t in path) {
			path_vec.Add(t.position);
		}

		duration_of_step = 3f / speed;

		moved.position = path_vec[0];
		moveTo(path_vec[1], duration_of_step);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha8)) {
			pause();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9)) {
			unpause();
		}
	}

	void onStepComplete() {
		currentPoint += 1;
		nextPoint = (currentPoint + 1) % path.Count;

		moveTo(path_vec[nextPoint], duration_of_step);
	}

	void moveTo(Vector3 position, float duration) {
		currentTween = moved.DOMove(position, duration);
		currentTween.SetEase(ease);
		currentTween.OnComplete(onStepComplete);
	}

	public void pause() {
		currentTween.Pause();
		is_paused = true;
	}

	public void unpause() {
		currentTween.Play();
		is_paused = false;
	}
}
