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

		Debug.LogFormat ("Rotation is {0} is  speed {1} times {2}", rotation, rotationSpeed, Input.GetAxis ("Horizontal"));

		transform.Translate (0, 0, translation);
		transform.Rotate (0, rotation, 0);

		if (translation != 0) {
			animator.SetBool ("IsWalking", true);
		} else {
			animator.SetBool ("IsWalking", false);
		}
			

		// player facing camera direction

	
}
}
