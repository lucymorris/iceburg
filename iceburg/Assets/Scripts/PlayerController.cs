using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  Animator animator;
  public float speed = 2.0f;
  public float rotationSpeed = 150.0f;
  public Camera cameraPrefab;
  public Transform cameraTarget;
  public float maxSlope = 30;
  public float staminaSeconds = 5;
  public float tumbleSeconds = 2;

  Transform cameraTransform;
  Rigidbody characterRigidbody;

  Vector3 prevMove;
  float tumble = 0;
  float stamina = 0;

  void Start()
  {
    Camera cam = Object.Instantiate(cameraPrefab);

    var camScript = cam.GetComponent<FollowCamera>();
    camScript.followedObject = cameraTarget;

    cameraTransform = cam.transform;

    animator = GetComponent<Animator>();
    characterRigidbody = GetComponent<Rigidbody>();

    prevMove = transform.forward;
  }

  void Update()
  {
    if (tumble > 0) {
      tumble -= Time.deltaTime;
    }

    if (!IsTumbling()) {
	    float forward = Input.GetAxis("Vertical");
	    float right = Input.GetAxis("Horizontal");

	    Vector3 inputVector = new Vector3(right, 0, forward);
	    // Normalize the input vector before scaling it so that pressing two directions at once doesn't make you move faster!
	    inputVector = inputVector.normalized;
	    inputVector = inputVector * speed * Time.deltaTime;

	    bool moving = inputVector.sqrMagnitude > float.Epsilon;

	    if (moving) {
	      animator.SetBool("IsWalking", true);
	    } else {
	      animator.SetBool("IsWalking", false);
	    }

	    Vector3 groundNormal = Vector3.up;

	    Vector3 actualGroundNormal = Vector3.up;

	    RaycastHit hitinfo;
	    float maxDistance = 3;
	    int layerMask = 1 << LayerMask.NameToLayer("Environment");
	    bool hit = Physics.Raycast(characterRigidbody.position, Vector3.down, out hitinfo, maxDistance, layerMask);
	    if (hit) {
	      actualGroundNormal = hitinfo.normal;
	    }

	    Vector3 cameraVector = Vector3.ProjectOnPlane(cameraTransform.forward, groundNormal);
	    Quaternion cameraRot = Quaternion.LookRotation(cameraVector, Vector3.up);
	    Vector3 moveVector = cameraRot * inputVector;

	    moveVector = Vector3.ProjectOnPlane(moveVector, groundNormal);

	    if (moving) {
	      characterRigidbody.MovePosition(transform.position + moveVector);
	      prevMove = moveVector;
	    } else {
	      moveVector = prevMove;
	    }

    	float groundAngle = Vector3.Angle(actualGroundNormal, Vector3.up);

    	bool angleOk = groundAngle < maxSlope;

    	if (angleOk) {
    		//animator.SetBool("UsingStamina", false);
    		stamina = staminaSeconds;
    	} else if (stamina > 0) {
    		//animator.SetBool("UsingStamina", true);
    		stamina -= Time.deltaTime;
    		angleOk = true;
    	} else {
    		//animator.SetBool("UsingStamina", true);
    	}
    
      if ((moving && angleOk) || angleOk) {
        tumble = 0;

        Quaternion targetRotation = Quaternion.LookRotation(moveVector, groundNormal);
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        characterRigidbody.MoveRotation(newRotation);

        characterRigidbody.angularVelocity = new Vector3();
      } else {
        tumble = tumbleSeconds;
      }
    }
  }

  bool IsTumbling()
  {
    return tumble > 0;
  }
}
