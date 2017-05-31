using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject target;

    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 tgtPos;
    private Vector2 camPos;
    private const float CAM_ACCELERATION = 8f;
    private const float VERT_OFFSET = 1f;

    private const float MAX_AXIS_DIFFERENCE_BETWEEN_CAM_AND_TARGET = 0.1f;

    /// <summary>
    ///     START
    /// </summary>
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        cam = GetComponent<Camera>();
    }

    /// <summary>
    ///     FIXED UPDATE
    /// </summary>
    void FixedUpdate() {
        tgtPos = target.transform.position + new Vector3(0f, VERT_OFFSET, 0f);
        camPos = cam.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if(isCameraOnTarget()) {
            rb.velocity = Vector2.zero;
        } else {
            Vector2 vectorToTarget = new Vector2(tgtPos.x - camPos.x, tgtPos.y - camPos.y);
            rb.velocity = vectorToTarget * CAM_ACCELERATION;
        }
    }

    /// <summary>
    ///     Checks if the camera is basically on top of the target.
    /// </summary>
    /// <returns>True if very close to target, false otherwise.</returns>
    private bool isCameraOnTarget() {
        bool res = false;
        if(Mathf.Abs(tgtPos.x - camPos.x) < MAX_AXIS_DIFFERENCE_BETWEEN_CAM_AND_TARGET
            && Mathf.Abs(tgtPos.y - camPos.y) < MAX_AXIS_DIFFERENCE_BETWEEN_CAM_AND_TARGET) {
            res = true;
        }

        return res;
    }
}
