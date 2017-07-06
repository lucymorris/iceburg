using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float lookSmooth = 0.09f;
    public Vector3 offsetFromTarget = new Vector3(0, 6, -8);
    public float xTilt = 10;

    private Vector3 destination = Vector3.zero;
    private CharacterController charController;
    private float rotateVel = 0;

	void Start () {
        SetCameraTarget(target);
	}
	
    void SetCameraTarget(Transform t)
    {
        target = t;

        if (target != null)
        {
            if (target.GetComponent<CharacterController>())
            {
                charController = target.GetComponent<CharacterController>();
            }
            else
                Debug.LogError("Y'ALL'S CAMERA'S TARGET NEED A CHAR CONTROLLER YO");
        }
        else
            Debug.LogError("Y'ALL NEED A TARGET");
    }

	void LateUpdate () {
        MovetoTarget();
        LookatTarget();
	}

    void MovetoTarget()
    {
        destination = charController.targetRotation * offsetFromTarget;
        destination += target.position;
        transform.position = destination;
    }

    void LookatTarget()
    {
        float eulerYAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target.eulerAngles.y, ref rotateVel, lookSmooth);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, eulerYAngle, 0);
    }
}
