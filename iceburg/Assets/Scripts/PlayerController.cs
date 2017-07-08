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
		float translation = Input.GetAxis ("Vertical") * speed;
		float rotation = Input.GetAxis ("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;

		characterRigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0, rotation, 0));
		characterRigidbody.MovePosition(transform.position + transform.rotation * new Vector3(0, 0, translation));

		if (translation != 0) {
			animator.SetBool ("IsWalking", true);
		} else {
			animator.SetBool ("IsWalking", false);
		}
		
		if (Mathf.Abs(translation) > 0.01f && Mathf.Abs(rotation) < 0.01f)
		{
			Quaternion targetRotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
			Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

			characterRigidbody.MoveRotation(newRotation);
		}
	}
}
