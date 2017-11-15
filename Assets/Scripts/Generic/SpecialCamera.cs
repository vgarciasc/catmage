using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpecialCamera : MonoBehaviour {

	public RoomManager roomManager;
	Vector3 originalPos;
    public bool following_player;
    public bool is_shaking;
    Player player;
    public Vector2 offset;
    Camera camera;
    BoxCollider2D currentBounds;

	public static SpecialCamera getSpecialCamera() {
		return (SpecialCamera) HushPuppy.safeFindComponent("MainCamera", "SpecialCamera");
	}

	void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        camera = this.GetComponentInChildren<Camera>();
		updateOriginalPos();
	}

	void Update () {
		// #if (UNITY_EDITOR)
		// if (roomManager.startRoom != null) {
		// 	this.transform.localPosition = new Vector3(
		// 		roomManager.startRoom.transform.localPosition.x,
		// 		roomManager.startRoom.transform.localPosition.y,
		// 		-10);
		// }
		// #endif
        if (following_player) {
            updateOriginalPos();
        }

        if (currentBounds != null) {
            updateRoomBounds();
        }
	}

    public void updateOriginalPos() {
        if (following_player && !is_shaking) {
            originalPos = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                -10);
            this.transform.position = originalPos;
        }

        originalPos = this.transform.localPosition;
    }

    public void setCurrentRoomBounds(Room room) {
        var aux = room.GetComponentInChildren<CameraArea>();
        if (aux == null) {
            currentBounds = null;
        }
        else {
            currentBounds = aux.GetComponentInChildren<BoxCollider2D>();
        }
    }

    void updateRoomBounds() {
        var cameraHalfWidth = camera.orthographicSize * ((float) Screen.width / Screen.height);
        var _min = currentBounds.bounds.min;
        var _max = currentBounds.bounds.max;
        float currentAspect = ((float)Screen.width) / ((float)Screen.height);
        offset = new Vector2(
            currentAspect * -3.78f + 5.04f,
            0f
        );

        var x = transform.position.x;
        var y = transform.position.y;
        var min_x = Mathf.Min((_min.x + offset.x + cameraHalfWidth), (_max.x - cameraHalfWidth - offset.x));
        var max_x = Mathf.Max((_min.x + offset.x + cameraHalfWidth), (_max.x - cameraHalfWidth - offset.x));
        x = Mathf.Clamp(x, min_x, max_x);
        y = Mathf.Clamp(y, _min.y + offset.y + camera.orthographicSize, _max.y - camera.orthographicSize + offset.y);

        transform.position = new Vector3(x, y, transform.position.z);
    }

    #region Screen Shake
    public void screenShake_(float power) { StartCoroutine(screenShake(power)); }
    IEnumerator screenShake(float power) {
        // if (power < 0.05f) {
        //     power = 0.1f;
        // }
        is_shaking = true;

        for (int i = 0; i < 10; i++) {
            yield return new WaitForEndOfFrame();
            float x = getScreenShakeDistance(power);
            float y = getScreenShakeDistance(power);

            this.transform.localPosition = new Vector3(originalPos.x + x,
                                                       originalPos.y + y,
                                                       originalPos.z);
        }

        this.transform.localPosition = originalPos;
        is_shaking = false;
    }

    float getScreenShakeDistance(float power) {
        float power_aux = power;
        int count = 0;
        while (true) {
            count++;
            float aux = Mathf.Pow(-1, Random.Range(0, 2)) * Random.Range(power_aux / 4, power_aux / 2);
            if (Mathf.Abs(aux) > 0.1f) {
                return aux;
            }
            if (count > 5) {
                count = 0;
                power_aux += 0.25f;
            }
        }
    }
    #endregion
}
