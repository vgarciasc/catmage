using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpecialCamera : MonoBehaviour {

	public RoomManager roomManager;
	Vector3 originalPos;

	public static SpecialCamera getSpecialCamera() {
		return (SpecialCamera) HushPuppy.safeFindComponent("MainCamera", "SpecialCamera");
	}

	void Start() {
		originalPos = this.transform.localPosition;
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
	}

    #region Screen Shake
    public void screenShake_(float power) { StartCoroutine(screenShake(power)); }
    IEnumerator screenShake(float power) {
        // if (power < 0.05f) {
        //     power = 0.1f;
        // }

        for (int i = 0; i < 10; i++) {
            yield return new WaitForEndOfFrame();
            float x = getScreenShakeDistance(power);
            float y = getScreenShakeDistance(power);

            this.transform.localPosition = new Vector3(originalPos.x + x,
                                                       originalPos.y + y,
                                                       originalPos.z);
        }

        this.transform.localPosition = originalPos;
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
