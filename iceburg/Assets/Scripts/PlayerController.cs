using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	Animator animator;
	public float speed = 2.0f;
	public float rotationSpeed = 150.0f;
	public Camera cameraPrefab;
	public Transform cameraTarget;

	Transform cameraTransform;
	Rigidbody characterRigidbody;

	void Start () {
		Camera cam = Object.Instantiate(cameraPrefab);

		var camScript = cam.GetComponent<FollowCamera>();
		camScript.target = cameraTarget;

		cameraTransform = cam.transform;
		
		animator = GetComponent<Animator>();
		characterRigidbody = GetComponent<Rigidbody>();
	}
	
	void Update () {
		float forward = Input.GetAxis ("Vertical");
		float right = Input.GetAxis ("Horizontal");

		Vector3 inputVector = new Vector3(right, 0, forward);
		// Normalize the input vector before scaling it so that pressing two directions at once doesn't make you move faster!
		inputVector = inputVector.normalized;
		inputVector = inputVector * speed * Time.deltaTime;

		Vector3 groundNormal = Vector3.up;
		RaycastHit hitinfo;
		float maxDistance = 3;
		int layerMask = Physics.DefaultRaycastLayers;
		bool hit = Physics.Raycast(characterRigidbody.position, Vector3.down, out hitinfo, maxDistance, layerMask);
		if (hit)
		{
			groundNormal = hitinfo.normal;
		}

		Vector3 cameraVector = Vector3.ProjectOnPlane(cameraTransform.forward, groundNormal);
		Quaternion cameraRot = Quaternion.LookRotation(cameraVector, Vector3.up);
		Vector3 moveVector = cameraRot * inputVector;

		moveVector = Vector3.ProjectOnPlane(moveVector, groundNormal);

		characterRigidbody.MovePosition(transform.position + moveVector);

		bool moving = inputVector.sqrMagnitude > float.Epsilon;

		if (moving) {
			animator.SetBool ("IsWalking", true);
		} else {
			animator.SetBool ("IsWalking", false);
		}

		characterRigidbody.angularVelocity = new Vector3();
		
		if (moving)
		{
			Quaternion targetRotation = Quaternion.LookRotation(moveVector, Vector3.up);
			Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

			characterRigidbody.MoveRotation(newRotation);
		}
	}
}
