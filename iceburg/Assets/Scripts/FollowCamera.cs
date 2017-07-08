using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
	public GameObject target;
	public float damping = 1;
    public bool allowAutoFacing;

    private float lookDeltaHoriz, lookDeltaVert;
    public float mouseSensitivity = 10f;

    private float moveFB, moveLR;
    public float moveSpeed = 2f;

    private float zoom;
    public float zoomSpeed = 2;

    public float zoomMin = -1;
    public float zoomMax = -10;

    void Start()
    {
        zoom = 0;
    }

    void Update()
    {
        zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        if (zoom > zoomMin)
            zoom = zoomMin;

        if (zoom < zoomMax)
            zoom = zoomMax;

        if (Input.GetMouseButton(1))
        {
            lookDeltaHoriz += Input.GetAxis("Mouse X") * mouseSensitivity;
            lookDeltaVert -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }
    }

    void LateUpdate() {
        float currentAngleAboutY = transform.eulerAngles.y;
        float currentAngleAboutX = transform.eulerAngles.x;
        float angleAboutY = currentAngleAboutY;
        float angleAboutX = currentAngleAboutX;
        // TODO: enable autofacing intelligently?
        if (allowAutoFacing)
        {
    		float desiredAngle = target.transform.eulerAngles.y;
    		angleAboutY = Mathf.MoveTowardsAngle(currentAngleAboutY, desiredAngle, moveSpeed);
        }

        angleAboutY += lookDeltaHoriz;
        angleAboutX += lookDeltaVert;

        lookDeltaHoriz = 0;
        lookDeltaVert = 0;

        Vector3 offset = new Vector3(0, 0, -zoom);

		Quaternion rotation = Quaternion.Euler(angleAboutX, angleAboutY, 0);
		transform.position = target.transform.position - (rotation * offset);

		transform.LookAt(target.transform);
	}

    public static float ClampAngle (float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F)) {
            if (angle < -360F) {
                angle += 360F;
            }
            if (angle > 360F) {
                angle -= 360F;
            }           
        }
        return Mathf.Clamp (angle, min, max);
    }
}