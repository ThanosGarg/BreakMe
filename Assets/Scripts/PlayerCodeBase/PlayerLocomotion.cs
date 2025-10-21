using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerLocomotion : MonoBehaviour
{
	// New Input System references
	[SerializeField] InputActionReference moveAction;   // Vector2
	[SerializeField] InputActionReference jumpAction;   // Button
	[SerializeField] InputActionReference sprintAction; // Button or Value

	[SerializeField] Transform cameraTransform; // optional, will fallback to Camera.main
	[SerializeField] float walkSpeed = 4f;
	[SerializeField] float sprintSpeed = 7f;
	[SerializeField] float rotationSmoothTime = 0.12f;
	[SerializeField] float acceleration = 10f;
	[SerializeField] float jumpHeight = 1.5f;
	[SerializeField] float gravity = -9.81f;

	// Camera follow settings (top-down)
	[SerializeField] bool useTopDownCamera = true;
	[SerializeField] float cameraHeight = 20f;         // how high above the player the camera sits
	[SerializeField] float cameraSmoothTime = 0.12f;   // smoothing time for camera follow
	[SerializeField] float cameraPitch = 90f;          // pitch in degrees; 90 = straight down
	[SerializeField] Vector3 cameraOffset = default;   // optional lateral offset from player

	CharacterController controller;
	float currentSpeed;
	float rotationVelocity;
	float verticalVelocity;

	// velocity used by SmoothDamp for camera
	Vector3 cameraVelocity;

	// jump request flagged from input callback
	bool jumpRequested = false;

	void OnEnable()
	{
		// enable actions if assigned
		if (moveAction != null && moveAction.action != null) moveAction.action.Enable();
		if (jumpAction != null && jumpAction.action != null)
		{
			jumpAction.action.Enable();
			jumpAction.action.performed += OnJumpPerformed;
		}
		if (sprintAction != null && sprintAction.action != null) sprintAction.action.Enable();
	}

	void OnDisable()
	{
		// disable and unsubscribe
		if (moveAction != null && moveAction.action != null) moveAction.action.Disable();
		if (jumpAction != null && jumpAction.action != null)
		{
			jumpAction.action.performed -= OnJumpPerformed;
			jumpAction.action.Disable();
		}
		if (sprintAction != null && sprintAction.action != null) sprintAction.action.Disable();
	}

	void Start()
	{
		controller = GetComponent<CharacterController>();
		if (cameraTransform == null && Camera.main != null)
			cameraTransform = Camera.main.transform;
	}

	void Update()
	{
		HandleMovement();
		HandleGravityAndJump();
	}

	void OnJumpPerformed(InputAction.CallbackContext ctx)
	{
		// set the flag; actual jump handled in physics step
		jumpRequested = true;
	}

	// ensure camera follows after movement
	void LateUpdate()
	{
		if (!useTopDownCamera || cameraTransform == null) return;

		// target position is directly above the player plus optional offset
		Vector3 targetPos = transform.position + Vector3.up * cameraHeight + cameraOffset;
		// smooth position
		cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPos, ref cameraVelocity, cameraSmoothTime);
		// lock rotation to top-down pitch and align yaw to world (0). If you want the camera to rotate with player, change yaw accordingly.
		cameraTransform.rotation = Quaternion.Euler(cameraPitch, 0f, 0f);
	}

	void HandleMovement()
	{
		// read input from new Input System (fallback to zero if action not assigned)
		Vector2 moveInput = Vector2.zero;
		if (moveAction != null && moveAction.action != null)
			moveInput = moveAction.action.ReadValue<Vector2>();

		float horizontal = moveInput.x;
		float vertical = moveInput.y;
		Vector3 inputRaw = new Vector3(horizontal, 0f, vertical);
		Vector3 inputDir = inputRaw.normalized;

		// determine target speed (sprint) using new input system (if assigned)
		bool isSprinting = false;
		if (sprintAction != null && sprintAction.action != null)
		{
			// treat any non-zero value as sprint pressed
			isSprinting = sprintAction.action.ReadValue<float>() > 0.5f;
		}

		float targetSpeed = (isSprinting ? sprintSpeed : walkSpeed) * (inputRaw.magnitude > 0f ? 1f : 0f);

		// smooth speed change
		currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

		if (inputDir.magnitude >= 0.01f)
		{
			// choose movement basis: world axes for top-down (or if camera missing), otherwise camera-relative
			Vector3 moveDir;
			if (useTopDownCamera || cameraTransform == null)
			{
				// world-relative: forward = +Z, right = +X (allows up/right/down/left and diagonals)
				moveDir = Vector3.forward * inputDir.z + Vector3.right * inputDir.x;
			}
			else
			{
				Vector3 camForward = cameraTransform.forward;
				camForward.y = 0f;
				camForward.Normalize();
				Vector3 camRight = cameraTransform.right;
				camRight.y = 0f;
				camRight.Normalize();

				moveDir = camForward * inputDir.z + camRight * inputDir.x;
			}

			moveDir.Normalize();

			// smooth rotation toward move direction
			float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
			float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
			transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

			// horizontal move vector
			Vector3 horizontalMove = moveDir * currentSpeed;
			// apply vertical velocity in HandleGravityAndJump via controller.Move below
			Vector3 finalMove = horizontalMove + Vector3.up * verticalVelocity;
			controller.Move(finalMove * Time.deltaTime);
		}
		else
		{
			// no input: still apply vertical motion (gravity/jump)
			controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
			// optionally decay speed to zero when stopping
			currentSpeed = Mathf.Lerp(currentSpeed, 0f, acceleration * Time.deltaTime);
		}
	}

	void HandleGravityAndJump()
	{
		if (controller.isGrounded)
		{
			// stick to ground to avoid tiny floating
			if (verticalVelocity < 0f)
				verticalVelocity = -2f;

			// jump (use flag set by new input system callback)
			if (jumpRequested)
			{
				verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
				jumpRequested = false;
			}
		}
		// apply gravity
		verticalVelocity += gravity * Time.deltaTime;
	}
}
