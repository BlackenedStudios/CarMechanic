using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pako : MonoBehaviour
{
    public VariableJoystick joystick;

    public float forwardAccrelation;
    public float angularAccrelation;

    public float maximumSpeed;

    public Transform body;

    private List<Wheel> wheels = new List<Wheel>();
    private List<TrailRenderer> skids = new List<TrailRenderer>();

    private Rigidbody rb;

    private bool moving = false;

    private Vector3 direction;
    private Vector3 movement;

    private Vector3 lastPosition;
    private Vector3 targetDirection;
    private Vector3 temp;

    private float currentAngle = 0;
    private float currentSpeed = 0;

    public static Pako instance;

    private void Awake()
    {
        Construct();
    }
    private void Construct()
    {
        rb = GetComponent<Rigidbody>();
        
        wheels = GetComponentsInChildren<Wheel>().ToList();
        skids = GetComponentsInChildren<TrailRenderer>().ToList();

        instance = this;

    }
    private void FixedUpdate()
    {
        Movement();
        CalculateMovement();
    }
    private void Update()
    {
        GetInput();
        
        UpdateVisuals();
    }
    private void GetInput()
    {
        moving = Input.GetMouseButton(0);

        if (moving) targetDirection = transform.position + Vector3.right * joystick.Horizontal + Vector3.forward * joystick.Vertical;
    }
    private void Movement()
    {
        if (moving)
        {
            if(currentSpeed < maximumSpeed) rb.AddRelativeForce(Vector3.forward * forwardAccrelation * Time.fixedDeltaTime);
            temp = targetDirection - transform.position; temp.y = 0;

            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(temp),Time.fixedDeltaTime * angularAccrelation);
        }
    }
    private void CalculateMovement()
    {
        direction = transform.position - lastPosition;
        movement = transform.InverseTransformDirection(direction);
        lastPosition = transform.position;
        
        currentAngle = movement.x * 200;
        currentSpeed = rb.velocity.magnitude;
    }
    private void UpdateVisuals()
    {
        body.localRotation = Quaternion.Lerp(body.localRotation, Quaternion.Euler(0, 0, Mathf.Clamp(-currentAngle / 5f,-10f,10f)),Time.deltaTime * 10f);
        
        wheels.ForEach(x => x.Animate(currentAngle, currentSpeed));
        skids.ForEach(x => x.emitting = Mathf.Abs(currentAngle) > 15);
    }
}
