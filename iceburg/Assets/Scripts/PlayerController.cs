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
	public float reachDistance = 2;
	public LayerMask pickUpLayers;

	public InputConfig inputConfig;

	public Expandy pickUpPrompt;

	Transform cameraTransform;
	Rigidbody characterRigidbody;
	UnityEngine.AI.NavMeshAgent agent; // nav-mesh agent used for avoidance
	PickerUpper pickerUpper;

	Fungus.Writer writer;

	Vector3 prevMove;
	float tumble = 0;
	float stamina = 0;

	Vector3 inputVector;
	bool moving;
	bool interactButtonDownHandled;

	Collider[] results = new Collider[60];

	void Awake()
	{
		pickerUpper = GetComponent<PickerUpper>();
		pickerUpper.pickUpDistance = reachDistance;
	}

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
		if (tumble > 0)
		{
			tumble -= Time.deltaTime;
		}

		float forward = Input.GetAxis(inputConfig.moveForwardBack);
		float right = Input.GetAxis(inputConfig.moveRightLeft);

		forward *= inputConfig.useMouse ? 1 : -1;

		inputVector = new Vector3(right, 0, forward);
		// Normalize the input vector before scaling it so that pressing two directions at once doesn't make you move faster!
		inputVector = inputVector.normalized;

		moving = inputVector.sqrMagnitude > float.Epsilon;

		if (moving)
		{
			animator.SetBool("IsWalking", true);
		}
		else
		{
			animator.SetBool("IsWalking", false);
		}

		if (Input.GetButtonDown(inputConfig.interact))
		{
			Fungus.Writer writer = Fungus.SayDialog.ActiveSayDialog.GetComponent<Fungus.Writer>();
			if (writer.IsWriting)
			{
				var inputListeners = Fungus.SayDialog.ActiveSayDialog.GetComponentsInChildren<Fungus.IDialogInputListener>();
				for (int i = 0; i < inputListeners.Length; i++)
				{
						var inputListener = inputListeners[i];
						inputListener.OnNextLineEvent();
				}
				interactButtonDownHandled = true;
			}
			else if (pickerUpper.heldItem != null)
			{
				interactButtonDownHandled = true;
				pickerUpper.Drop();
			}
		}
		if (!interactButtonDownHandled)
		{
			pickerUpper.pickUpDistance = reachDistance;

			Holdable closest = null;
			if (pickerUpper.heldItem == null)
			{
				Vector3 origin = this.transform.position;
				float radius = pickerUpper.pickUpDistance;
				int layerMask = pickUpLayers.value;
				int hits = Physics.OverlapSphereNonAlloc(origin, radius, results, layerMask);
				float closestDist = pickerUpper.activeRadius;

				Vector3 pickUpPos = origin;

				for (int index = 0; index < hits; ++index)
				{
					Collider hit = results[index];
					float distance = Vector3.Distance(hit.transform.position, pickUpPos);
					if (distance < closestDist)
					{
						Holdable item = hit.GetComponent<Holdable>();
						if (item != null)
						{
							closestDist = distance;
							closest = item;
						}
					}
				}
			}
			if (pickUpPrompt != null)
			{
				if (closest != null)
				{
					pickUpPrompt.Show();
				}
				else
				{
					pickUpPrompt.Hide();
				}
			}
			if (Input.GetButton(inputConfig.interact))
			{
				if (pickerUpper.heldItem == null)
				{
					pickerUpper.Activate();
					if (closest != null)
					{
						pickerUpper.PickUp(closest);
					}
				}
			}
		}
		else
		{
			if (Input.GetButtonUp(inputConfig.interact))
			{
				interactButtonDownHandled = false;
			}
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

				Quaternion targetRotation;
				if (moving)
				{
					targetRotation = Quaternion.LookRotation(moveVector, groundNormal);
				}
				else
				{
					targetRotation = Quaternion.LookRotation(transform.forward, groundNormal);
				}
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
