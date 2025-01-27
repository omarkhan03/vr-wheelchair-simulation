using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRWheelchair : MonoBehaviour
{
    public Collider leftHand;
    public Collider rightHand;
    public Collider leftWheel;
    public Collider rightWheel;

    public Transform leftWheelTransform;  // Attach the left wheel in the Inspector
    public Transform rightWheelTransform; // Attach the right wheel in the Inspector

    public float speedMultiplier = 0.1f;  // Controls how fast the wheelchair moves based on hand momentum
    public float turnMultiplier = 2.5f;   // Controls how fast the wheelchair turns
    public float momentumDecayRate = 0.1f; // How quickly the momentum decays
    public float wheelRadius = 0.3f;      // Approximate radius of the wheels

    private WheelchairInputActions controls;

    private bool rightGrip;
    private bool leftGrip;
    private bool leftWheelGripped;
    private bool rightWheelGripped;

    private Vector3 previousLeftHandPosition;
    private Vector3 previousRightHandPosition;

    private float leftWheelMomentum = 0f;
    private float rightWheelMomentum = 0f;

    void Awake()
    {
        controls = new WheelchairInputActions();

        controls.VR.RightGrip.performed += ctx => rightGrip = true;
        controls.VR.RightGrip.canceled += ctx => rightGrip = false;

        controls.VR.LeftGrip.performed += ctx => leftGrip = true;
        controls.VR.LeftGrip.canceled += ctx => leftGrip = false;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        // Initialize previous positions
        previousLeftHandPosition = leftHand.transform.position;
        previousRightHandPosition = rightHand.transform.position;
    }

    void Update()
    {
        // Gripping Logic
        leftWheelGripped = leftGrip && leftHand.bounds.Intersects(leftWheel.bounds);
        rightWheelGripped = rightGrip && rightHand.bounds.Intersects(rightWheel.bounds);

        // Calculate momentum for each wheel
        if (leftWheelGripped)
        {
            leftWheelMomentum += CalculateMomentum(leftHand, ref previousLeftHandPosition);
        }
        if (rightWheelGripped)
        {
            rightWheelMomentum += CalculateMomentum(rightHand, ref previousRightHandPosition);
        }

        // Apply friction-like decay to momentum
        leftWheelMomentum *= Mathf.Pow(momentumDecayRate, Time.deltaTime);
        rightWheelMomentum *= Mathf.Pow(momentumDecayRate, Time.deltaTime);

        // Apply movement and turning based on momentum
        ApplyWheelMovement(leftWheelMomentum, rightWheelMomentum);

        // Rotate wheels visually
        RotateWheels(leftWheelMomentum, rightWheelMomentum);

        // Update previous positions
        previousLeftHandPosition = leftHand.transform.position;
        previousRightHandPosition = rightHand.transform.position;
    }

    /// <summary>
    /// Calculate momentum based on hand movement.
    /// </summary>
    float CalculateMomentum(Collider hand, ref Vector3 previousPosition)
    {
        Vector3 velocity = (hand.transform.position - previousPosition) / Time.deltaTime;
        float forwardMomentum = Vector3.Dot(velocity, transform.forward);

        // Threshold to filter small noise
        if (Mathf.Abs(forwardMomentum) < 0.05f)
        {
            forwardMomentum = 0f;
        }

        return forwardMomentum;
    }

    /// <summary>
    /// Apply combined wheel momentum for movement and turning.
    /// </summary>
    void ApplyWheelMovement(float leftMomentum, float rightMomentum)
    {
        // Calculate Forward Movement (average of both wheels)
        float forwardMovement = (leftMomentum + rightMomentum) / 2.0f;
        transform.Translate(Vector3.forward * forwardMovement * speedMultiplier * Time.deltaTime);

        // Calculate Turning Movement (difference between wheels)
        float turnAmount = (leftMomentum - rightMomentum) * turnMultiplier * Time.deltaTime;
        transform.Rotate(Vector3.up, turnAmount);
    }

    /// <summary>
    /// Rotate the wheels visually based on momentum.
    /// </summary>
    void RotateWheels(float leftMomentum, float rightMomentum)
    {
        // Calculate the wheel's rotational speed based on linear momentum
        float leftWheelRotation = (leftMomentum * speedMultiplier * Time.deltaTime) / (2 * Mathf.PI * wheelRadius) * 360f; // Degrees
        float rightWheelRotation = (rightMomentum * speedMultiplier * Time.deltaTime) / (2 * Mathf.PI * wheelRadius) * 360f; // Degrees

        // Apply rotation to the wheels
        leftWheelTransform.Rotate(Vector3.right, leftWheelRotation, Space.Self);
        rightWheelTransform.Rotate(Vector3.right, rightWheelRotation, Space.Self);
    }
}
