using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public Animator animator;
  public float speed = 1.0f;
  public float rotationSpeed = 150.0f;
  public Camera cameraPrefab;
  public Transform cameraTarget;
  public float maxSlope = 30;
  public float staminaSeconds = 5;
  public float tumbleSeconds = 2;
  public bool enableTumbling = false;
  public int playerIndex = 0;

  public InputConfig inputConfig;

  Transform cameraTransform;
  Rigidbody characterRigidbody;
  private UnityEngine.AI.NavMeshAgent agent; // nav-mesh agent used for avoidance

  Vector3 prevMove;
  float tumble = 0;
  float stamina = 0;

  Vector3 inputVector;
  bool moving;

  void Start()
  {
    Camera cam = Object.Instantiate(cameraPrefab);

    var camScript = cam.GetComponent<FollowCamera>();
    camScript.followedObject = cameraTarget;
    camScript.playerIndex = playerIndex;
    camScript.inputConfig = inputConfig;

    AudioListener al = cam.GetComponent<AudioListener>();
    al.enabled = playerIndex == 0;

    cameraTransform = cam.transform;

    //animator = GetComponent<Animator>();
    characterRigidbody = GetComponent<Rigidbody>();
    agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    agent.updatePosition = false;
    agent.updateRotation = false;

    prevMove = transform.forward;
  }

  void Update()
  {
    if (tumble > 0) {
      tumble -= Time.deltaTime;
    }

    float forward = Input.GetAxis(inputConfig.moveForwardBack);
    float right = Input.GetAxis(inputConfig.moveRightLeft);

    inputVector = new Vector3(right, 0, forward);
    // Normalize the input vector before scaling it so that pressing two directions at once doesn't make you move faster!
    inputVector = inputVector.normalized;

    moving = inputVector.sqrMagnitude > float.Epsilon;

    if (moving) {
      animator.SetBool("IsWalking", true);
    } else {
      animator.SetBool("IsWalking", false);
    }
  }

  void FixedUpdate()
  {
    if (!IsTumbling())
    {
      characterRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

      Vector3 groundNormal = Vector3.up;

      Vector3 actualGroundNormal = Vector3.up;

      RaycastHit hitinfo;
      float maxDistance = 3;
      int layerMask = 1 << LayerMask.NameToLayer("Environment");
      bool hit = Physics.Raycast(characterRigidbody.position, Vector3.down, out hitinfo, maxDistance, layerMask);
      if (hit) {
        actualGroundNormal = hitinfo.normal;
      }

      float desiredSpeed = 0;
      if (moving)
      {
        desiredSpeed = speed;
      }

      Vector3 scaledInput = inputVector * desiredSpeed;

      Vector3 cameraVector = Vector3.ProjectOnPlane(cameraTransform.forward, groundNormal);
      Quaternion cameraRot = Quaternion.LookRotation(cameraVector.normalized, Vector3.up);
      Vector3 moveVector = cameraRot * scaledInput;

      moveVector = Vector3.ProjectOnPlane(moveVector, groundNormal);

    #if PHYSICS_MOVEMENT
      if (moving)
      {
        Vector3 velocityChange = (moveVector - characterRigidbody.velocity);
        characterRigidbody.AddForce(moveVector * 100);
        prevMove = moveVector;
      }
      else
      {
        // slow down
        Vector3 velocityChange = (moveVector - characterRigidbody.velocity) * speed;
        characterRigidbody.AddForce(velocityChange, ForceMode.Acceleration);

        moveVector = prevMove;
      }

      // limit speed
      float maxSpeed = desiredSpeed * 10;
      if (characterRigidbody.velocity.magnitude > maxSpeed)
      {
        Vector3 maxVelocity = characterRigidbody.velocity.normalized * maxSpeed;
        Vector3 velocityChange = (maxVelocity - characterRigidbody.velocity);
        characterRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
      }
    #else
      if (moving) {
        characterRigidbody.MovePosition(transform.position + moveVector * Time.deltaTime);
        prevMove = moveVector;
      } else {
        moveVector = prevMove;
      }
    #endif


      float groundAngle = Vector3.Angle(actualGroundNormal, Vector3.up);

      bool angleOk = groundAngle < maxSlope;

      if (angleOk)
      {
        //animator.SetBool("UsingStamina", false);
        stamina = staminaSeconds;
      }
      else if (stamina > 0)
      {
        //animator.SetBool("UsingStamina", true);
        stamina -= Time.deltaTime;
        angleOk = true;
      }
      else
      {
        //animator.SetBool("UsingStamina", true);
      }
    
      if (angleOk)
      {
        tumble = 0;

        Quaternion targetRotation = Quaternion.LookRotation(moveVector, groundNormal);
        Quaternion newRotation = Quaternion.RotateTowards(characterRigidbody.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        characterRigidbody.MoveRotation(newRotation);
      }
      else
      {
        tumble = tumbleSeconds;
        characterRigidbody.constraints = RigidbodyConstraints.None;
      }
    }
    else
    {
      // Tumbling
    }

    agent.nextPosition = characterRigidbody.position;
    agent.velocity = characterRigidbody.velocity;
  }

  bool IsTumbling()
  {
    return enableTumbling && tumble > 0;
  }
}
