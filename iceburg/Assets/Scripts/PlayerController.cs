using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	Animator animator;
	public float speed = 2.0f;
	public float rotationSpeed = 150.0f;

	Transform cameraTransform;
	new Rigidbody rigidbody;

	void Start () {
		animator = GetComponent<Animator> ();
		cameraTransform = Camera.main.transform;
		rigidbody = GetComponent<Rigidbody>();
	}
	
	void Update () {

		float translation = Input.GetAxis ("Vertical") * speed;
		float rotation = Input.GetAxis ("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;

		rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0, rotation, 0));
		rigidbody.MovePosition(transform.position + transform.rotation * new Vector3(0, 0, translation));

		if (translation != 0) {
			animator.SetBool ("IsWalking", true);
		} else {
			animator.SetBool ("IsWalking", false);
		}
		
		if (translation != 0)
		{
			Quaternion targetRotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
			Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

			rigidbody.MoveRotation(newRotation);
		}
	}
}
