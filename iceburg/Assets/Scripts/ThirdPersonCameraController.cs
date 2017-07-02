using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour {

	public Transform playerCam, character, centrePoint;

	private float mouseX, mouseY;
	public float mouseSensitivity = 10f;

	private float moveFB, moveLR;
	public float moveSpeed = 2f;

	private float zoom;
	public float zoomSpeed = 2;

	public float zoomMin = -2f;
	public float zoomMax = -10f;

	// Use this for initialization
	void Start () {

		zoom = -1;
		
	}
	
	// Update is called once per frame
	void Update () {

		zoom += Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed;

		if (zoom > zoomMin)
			zoom = zoomMin;

		if (zoom < zoomMax)
			zoom = zoomMax;

		playerCam.transform.position = new Vector3 (playerCam.transform.position.x, playerCam.transform.position.y, zoom);

		if (Input.GetMouseButton (1)) {
			mouseX += Input.GetAxis ("Mouse X");
			mouseY -= Input.GetAxis ("Mouse Y");
		}

		mouseY = Mathf.Clamp (mouseY, -60f, 60f);
		playerCam.LookAt (centrePoint); 
		centrePoint.localRotation = Quaternion.Euler (mouseY, mouseX, 0);


	}
}
