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

    [System.NonSerialized]
    public int playerIndex;

    [System.NonSerialized]
    public InputConfig inputConfig;

    float currentAngleAboutY;
    float currentAngleAboutX;

    float currentYOffset = 0;

    bool justSpawned;

    Camera thisCamera;

    bool zoomMode = false;

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
        zoom = (zoomMax - zoomMin) * 0.5f;
        // var postProcessing = GetComponent<PostProcessLayer>();
        // if (postProcessing != null) {
        //     postProcessing.volumeTrigger = followedObject;
        // }
        currentAngleAboutY = transform.eulerAngles.y;
        currentAngleAboutX = transform.eulerAngles.x;

        justSpawned = true;

        thisCamera.rect = new Rect( 0.0f,0.5f * (float)playerIndex, 1.0f, 0.5f);
    }

    void Update()
    {
        if (inputConfig.useMouse)
        {
            zoomMode = true;
        }
        else if (Input.GetButtonDown(inputConfig.toggleZoom))
        {
            zoomMode = !zoomMode;
        }
        if (zoomMode)
        {
            zoom += Input.GetAxis(inputConfig.camZoom) * inputConfig.camZoomSensitivity * zoomSpeed;
        }

        if (zoom > zoomMin)
            zoom = zoomMin;

        if (zoom < zoomMax)
            zoom = zoomMax;

        if (!zoomMode || (inputConfig.useMouse && Input.GetMouseButton(1)))
        {
            lookDeltaHoriz += Input.GetAxis(inputConfig.camPan) * inputConfig.camTurnSensitivity * mouseSensitivity;
            lookDeltaVert -= Input.GetAxis(inputConfig.camPitch) * inputConfig.camTurnSensitivity * mouseSensitivity;
        }
    }

    void LateUpdate() {
        // TODO: enable autofacing intelligently?
        if (allowAutoFacing)
        {
    		float desiredAngle = followedObject.eulerAngles.y;
    		currentAngleAboutY = Mathf.MoveTowardsAngle(currentAngleAboutY, desiredAngle, moveSpeed * Time.deltaTime);
        }

        // add rotation input
        currentAngleAboutY += lookDeltaHoriz;
        currentAngleAboutX += lookDeltaVert;
        lookDeltaHoriz = 0;
        lookDeltaVert = 0;

        currentAngleAboutX = ClampAngle(currentAngleAboutX, angleMin, angleMax);

        Vector3 offset = new Vector3(0, 0, -zoom);
		Quaternion rotation = Quaternion.Euler(currentAngleAboutX, currentAngleAboutY, 0);
		Vector3 targetPosition = followedObject.position - (rotation * offset);


        //////////////////
        /// check for objects blocking the line of sight to the character
        int blockingLayerMask = 1 << LayerMask.NameToLayer("Environment");
        Vector3 directionFromCamToFollowedObj = Vector3.Normalize(targetPosition - followedObject.position);
        RaycastHit hitInfo;
        bool hit = Physics.Linecast(followedObject.position, targetPosition, out hitInfo, blockingLayerMask);
        if (hit)
        {
            Debug.DrawLine(followedObject.position, targetPosition, Color.red);

            float avoidDist = -zoomMin;
            if (hitInfo.distance > -zoomMin)
            {
                avoidDist = hitInfo.distance;
            }
            targetPosition = followedObject.position + directionFromCamToFollowedObj * avoidDist;
        }


        // apply the look rotation here, before moving the camera out of the ground, to reduce jitter whenever the height changes
        Quaternion targetRotation = Quaternion.LookRotation(followedObject.position - targetPosition, Vector3.up);

        transform.rotation = targetRotation;


        ////////////////
        /// avoid clipping into the ground

        float lerpSpeed = justSpawned ? 100000 : Time.deltaTime * zoomSpeed;

        // figure out the coordinates of the middle point of the camera view
        Vector3 viewMiddle = targetPosition + transform.forward * thisCamera.nearClipPlane;
        // get an orientation vector
        Vector3 viewPitchVector = targetRotation * Vector3.down;
        // determine the size of the near clip plane
        Vector2 cameraNearPlaneExtents = CameraPlaneExtentsInWorldSpace(thisCamera, thisCamera.nearClipPlane);

        // determine a starting point for the collision check (a bit higher than the current target mid point in case it's far below the terrain)
        float originRelativeOffset = 4;
        Vector3 castOrigin = viewMiddle - viewPitchVector * cameraNearPlaneExtents.y * originRelativeOffset;

        Vector3 boxExtents = new Vector3(cameraNearPlaneExtents.x, cameraNearPlaneExtents.y, 0.05f);

        // just used for debugging
        Vector3 viewBottom = viewMiddle + viewPitchVector * cameraNearPlaneExtents.y;

        int groundLayerMask = 1 << LayerMask.NameToLayer("Environment");
        hit = Physics.BoxCast(castOrigin, boxExtents, viewPitchVector, out hitInfo, targetRotation, groundLayerMask);
        if (hit && hitInfo.distance < cameraNearPlaneExtents.y*originRelativeOffset)
        {
            Debug.DrawLine(castOrigin, viewBottom, Color.yellow);
            float targetYOffset = hitInfo.point.y + cameraNearPlaneExtents.y;
            targetYOffset = Mathf.Max(0, targetYOffset - targetPosition.y);
            currentYOffset = Mathf.MoveTowards(currentYOffset, targetYOffset, lerpSpeed);
        }
        else
        {
            Debug.DrawLine(castOrigin, viewBottom, Color.green);
            currentYOffset = Mathf.MoveTowards(currentYOffset, 0, lerpSpeed);
        }

        viewMiddle.y = targetPosition.y + currentYOffset;
        Vector3 later = viewMiddle - this.transform.forward * thisCamera.nearClipPlane;

        Vector3 offsetTargetPosition = new Vector3(targetPosition.x, later.y, targetPosition.z);
        Debug.DrawLine(targetPosition, offsetTargetPosition, Color.cyan);
        
        targetPosition = offsetTargetPosition;


        transform.position = targetPosition;

        justSpawned = false;
	}

    public static float ClampAngle(float angle, float min, float max)
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

    struct Quad
    {
        public Vector3 tl;
        public Vector3 tr;
        public Vector3 br;
        public Vector3 bl;
    }

    public static Vector2 CameraPlaneExtentsInWorldSpace(Camera cam, float distance)
    {
        float halfFov = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float aspect = cam.aspect;
        float halfHeight = Mathf.Tan(halfFov) * distance;
        float halfWidth = halfHeight * aspect;

        Vector2 result = new Vector2(halfWidth, halfHeight);

        return result;
    }
}