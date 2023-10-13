using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;


public class HotSlide : MonoBehaviour
{
    public VariableJoystick joystick;

    public float forwardAccrelation;
    public float angularAccrelation;
    public float speedDuplicator;

    public float maximumSpeed;
    public float minimumSpeed;

    public Transform body;
    public Transform cameraTarget;

    public SplineFollower follower;

    public float followerMinSpeed;
    public float followerMaxSpeed;
    public float followerAcc;
    public float followerDuplicator;

    private List<Wheel> wheels = new List<Wheel>();
    private List<TrailRenderer> skids = new List<TrailRenderer>();

    private Rigidbody rb;

    private bool moving = true;

    private Vector3 direction;
    private Vector3 movement;

    private Vector3 lastPosition;
    private Vector3 targetDirection;
    private Vector3 temp;


    private float currentAngle = 0;
    private float currentSpeed = 0;
    private float currentDistance = 0;

    private float _maxCar = 0;
    private float _maxFollower = 0;
    private void Awake()
    {
        Construct();
    }
    private void Construct()
    {
        rb = GetComponent<Rigidbody>();

        wheels = GetComponentsInChildren<Wheel>().ToList();
        skids = GetComponentsInChildren<TrailRenderer>().ToList();

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
        targetDirection = follower.transform.position + (follower.transform.right * joystick.Horizontal * 5);
        cameraTarget.position = targetDirection;
    }
    private void Movement()
    {
        if (moving)
        {
            _maxCar = maximumSpeed;
            _maxFollower = followerMaxSpeed;
        }
        else
        {
            _maxCar = _maxFollower = 5;
        }

        if (currentSpeed < _maxCar) rb.AddRelativeForce(Vector3.forward * forwardAccrelation * currentDistance * speedDuplicator);
        temp = targetDirection - transform.position; temp.y = 0;

        follower.followSpeed = Mathf.Clamp(followerAcc / (currentDistance * followerDuplicator), followerMinSpeed, _maxFollower);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(temp), Time.fixedDeltaTime * angularAccrelation);
    }
    private void CalculateMovement()
    {
        direction = transform.position - lastPosition;
        movement = transform.InverseTransformDirection(direction);
        lastPosition = transform.position;

        currentAngle = movement.x * 200;
        currentSpeed = rb.velocity.magnitude;
        currentDistance = Mathf.Abs(transform.position.x - follower.transform.position.x) + Mathf.Abs(transform.position.z - follower.transform.position.z);
    }
    private void UpdateVisuals()
    {
        body.localRotation = Quaternion.Lerp(body.localRotation, Quaternion.Euler(0, 0, Mathf.Clamp(-currentAngle / 5f, -10f, 10f)), Time.deltaTime * 10f);

        wheels.ForEach(x => x.Animate(currentAngle, currentSpeed));
        skids.ForEach(x => x.emitting = Mathf.Abs(currentAngle) > 15);
    }
}
