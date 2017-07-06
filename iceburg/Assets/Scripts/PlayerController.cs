using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	static Animator animator;
	public float speed = 2.0f;
	public float rotationSpeed = 150.0f;

	void Start () {

		animator = GetComponent<Animator> ();

	}
	
	void Update () {

		float translation = Input.GetAxis ("Vertical") * speed;
		float rotation = Input.GetAxis ("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;

		transform.Translate (0, 0, translation);
		transform.Rotate (0, rotation, 0);

		if (translation != 0) {
			animator.SetBool ("IsWalking", true);
		} else {
			animator.SetBool ("IsWalking", false);
		}
			
		//character.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

		// player facing camera direction

	
}
}
