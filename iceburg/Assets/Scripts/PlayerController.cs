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
		float forward = Input.GetAxis ("Vertical") * speed;
		//float rotation = Input.GetAxis ("Horizontal") * rotationSpeed;
		float right = Input.GetAxis ("Horizontal") * speed;
		forward *= Time.deltaTime;
		//rotation *= Time.deltaTime;
		right *= Time.deltaTime;

		Vector3 inputVector = new Vector3(right, 0, forward);

		Vector3 moveVector = cameraTransform.rotation * inputVector;

		//characterRigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0, rotation, 0));
		characterRigidbody.MovePosition(transform.position + moveVector);

		bool moving = inputVector.sqrMagnitude > float.Epsilon;

		if (moving) {
			animator.SetBool ("IsWalking", true);
		} else {
			animator.SetBool ("IsWalking", false);
		}
		
		if (moving)
		{
			Quaternion targetRotation = Quaternion.LookRotation(moveVector, Vector3.up);
			Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

			characterRigidbody.MoveRotation(newRotation);
		}
	}
}
