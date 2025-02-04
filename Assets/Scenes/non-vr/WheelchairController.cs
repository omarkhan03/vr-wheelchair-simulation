using UnityEngine;
using UnityEngine.InputSystem;

public class WheelchairController : MonoBehaviour
{
    public float speed = 5f;        // Speed for forward movement
    public float turnSpeed = 50f;   // Speed for turning

    private WheelchairInputActions controls;
    
    private bool forwardLeft;
    private bool forwardRight;
    
    private bool backLeft;
    private bool backRight;

    void Awake()
    {
        controls = new WheelchairInputActions();

        controls.WheelchairControls.LeftWheelForward.performed += ctx => forwardLeft = true;
        controls.WheelchairControls.LeftWheelForward.canceled += ctx => forwardLeft = false;

        controls.WheelchairControls.RightWheelForward.performed += ctx => forwardRight = true;
        controls.WheelchairControls.RightWheelForward.canceled += ctx => forwardRight = false;
        
        controls.WheelchairControls.LeftWheelBackward.performed += ctx => backLeft = true;
        controls.WheelchairControls.LeftWheelBackward.canceled += ctx => backLeft = false;

        controls.WheelchairControls.RightWheelBackward.performed += ctx => backRight = true;
        controls.WheelchairControls.RightWheelBackward.canceled += ctx => backRight = false;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {

        // Turning
        if (forwardLeft)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * speed * Time.deltaTime / 2);
        }
        if (forwardRight)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * speed * Time.deltaTime / 2);
        }
        if (backLeft)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
            transform.Translate(Vector3.back * speed * Time.deltaTime / 2);
        }
        if (backRight)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            transform.Translate(Vector3.back * speed * Time.deltaTime / 2);
        }
    }
}