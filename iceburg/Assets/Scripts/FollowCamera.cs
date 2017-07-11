using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

public class FollowCamera : MonoBehaviour {
    [UnityEngine.Serialization.FormerlySerializedAs("target")]
	public Transform followedObject;
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

    public float angleMin = 20;
    public float angleMax = 80;

    float currentAngleAboutY;
    float currentAngleAboutX;

    float currentYOffset = 0;

    Camera thisCamera;

    void Awake()
    {
        thisCamera = GetComponent<Camera>();
        if (thisCamera == null)
        {
            Debug.LogError("No camera attacked to FollowCamera object");
        }
    }

    void Start()
    {
        #if UNITY_EDITOR
        if (followedObject == null)
        {
            Object.Destroy(this.gameObject);
            return;
        }
        #endif
        zoom = 0;
        // var postProcessing = GetComponent<PostProcessLayer>();
        // if (postProcessing != null) {
        //     postProcessing.volumeTrigger = followedObject;
        // }
        currentAngleAboutY = transform.eulerAngles.y;
        currentAngleAboutX = transform.eulerAngles.x;
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
        // TODO: enable autofacing intelligently?
        if (allowAutoFacing)
        {
    		float desiredAngle = followedObject.eulerAngles.y;
    		currentAngleAboutY = Mathf.MoveTowardsAngle(currentAngleAboutY, desiredAngle, moveSpeed * Time.deltaTime);
        }

        currentAngleAboutY += lookDeltaHoriz;
        currentAngleAboutX += lookDeltaVert;
        lookDeltaHoriz = 0;
        lookDeltaVert = 0;

        currentAngleAboutX = ClampAngle(currentAngleAboutX, angleMin, angleMax);

        Vector3 offset = new Vector3(0, 0, -zoom);

		Quaternion rotation = Quaternion.Euler(currentAngleAboutX, currentAngleAboutY, 0);
		Vector3 targetPosition = followedObject.position - (rotation * offset);

        Vector3 direction = Vector3.Normalize(targetPosition - followedObject.position);
        RaycastHit hitInfo;
        bool hit = Physics.Linecast(followedObject.position, targetPosition, out hitInfo);
        if (hit)
        {
            Debug.DrawLine(followedObject.position, targetPosition, Color.red);

            float avoidDist = -zoomMin;
            if (hitInfo.distance > -zoomMin)
            {
                avoidDist = hitInfo.distance;
            }
            targetPosition = followedObject.position + direction * avoidDist;
        }

        Quaternion targetRotation = Quaternion.LookRotation(followedObject.position - targetPosition, Vector3.up);

        Vector3 viewBottom = thisCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, thisCamera.nearClipPlane)) - thisCamera.transform.position;
        viewBottom = viewBottom + targetPosition;

        Vector3 viewMiddle = targetPosition + new Vector3(0, 0, thisCamera.nearClipPlane);

        float maxDistance = (viewMiddle-viewBottom).magnitude;
        int layerMask = 1 << LayerMask.NameToLayer("Environment");
        hit = Physics.Linecast(viewMiddle, viewBottom, out hitInfo, layerMask);
        if (hit)
        {
            Debug.DrawLine(viewMiddle, viewBottom, Color.yellow);
            currentYOffset = Mathf.MoveTowards(currentYOffset, maxDistance - hitInfo.point.y, Time.deltaTime * zoomSpeed);
        }
        else
        {
            Debug.DrawLine(viewMiddle, viewBottom, Color.green);
            currentYOffset = Mathf.MoveTowards(currentYOffset, targetPosition.y, Time.deltaTime * zoomSpeed);
        }

        targetPosition.y = currentYOffset;

        targetRotation = Quaternion.LookRotation(followedObject.position - targetPosition, Vector3.up);

        transform.position = targetPosition;

		transform.rotation = targetRotation;
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