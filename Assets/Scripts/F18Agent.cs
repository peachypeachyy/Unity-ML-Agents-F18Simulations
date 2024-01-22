using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static Data.Util.KeywordDependentCollection;
using System;

public class F18Agent : Agent
{
    public GameObject aircraftCarrier;
    private F18Initialize f18Aircraft;
    private Rigidbody rb;

    [Header("Agent Settings")]
    public float speed = 10f;
    public float turnSpeed = 10f;
    public float pitchSpeed = 5f;
    private float smoothYaw = 0f;
    private float smoothPitch = 0f;

    private const float MaxPitchAngle = 80f;

    public Vector3 initialTargetDistance = Vector3.zero;
    public Vector3 updatedTargetDistance = Vector3.zero;
    public Vector3 maxAircraftDistance = new Vector3(250f, 200f, 250f);
    private Vector3 randomDirection;
    private Vector3 previousPosition;

    public Transform aircraftCarrierTransform;


    private void Start()
    {
        Debug.Log("Inside the Start in F18Agent");
        f18Aircraft = GetComponent<F18Initialize>();
        rb = GetComponent<Rigidbody>();
        previousPosition = transform.position;

        Debug.Log("Value of F18's position inside Start in F18Agent" + transform.position);
    }

    public override void OnEpisodeBegin()
    {
        f18Aircraft.Respawn();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position += randomDirection * speed * Time.deltaTime;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Direction to the Aircraft carrier
        Vector3 initialTargetDistance = (transform.position - aircraftCarrier.transform.position).normalized;
        sensor.AddObservation(initialTargetDistance);

        // Plane's current forward direction
        sensor.AddObservation(transform.forward);

        // Plane's velocity
        sensor.AddObservation(rb.velocity);
    }

    private void SetRandomDirection()
    {
        // Generate a random direction
        float randomX = UnityEngine.Random.Range(-1f, 1f);
        float randomY = UnityEngine.Random.Range(-0.5f, 0.5f);
        float randomZ = UnityEngine.Random.Range(-1f, 1f);

        randomDirection = new Vector3(randomX, randomY, randomZ).normalized;
    }

    private void OnTiggerEnter(Collider other)
    {
        int i = 0;
        if (other.gameObject.tag == "Sea")
        {
            Int32 xRotation = UnityEngine.Random.Range(0, -45);
            transform.rotation = Quaternion.Euler(xRotation, -180, 0);
            for (i=0; i<=5; i++)
            {
                rb.AddForce(transform.forward * speed * 3, ForceMode.Impulse);
            }

            // Apply an upward force to lift the aircraft
            Vector3 upwardForce = Vector3.up * rb.mass * 50.0f;
            rb.AddForce(upwardForce, ForceMode.Impulse);
        }

        if (other.gameObject.tag == "AircraftCarrier")
        {
            f18Aircraft.Respawn();
        }
    }

    private void Update()
    {
        float distanceToCarrier = Vector3.Distance(transform.position, aircraftCarrierTransform.position);
        Debug.Log("Distance to carrier: " + distanceToCarrier);
    }

    private void LateUpdate()
    {
        previousPosition = transform.position;
    }

    private void GiveRewardBasedOnDistance()
    {
        float previousDistance = Vector3.Distance(previousPosition, aircraftCarrierTransform.position);
        float currentDistance = Vector3.Distance(transform.position, aircraftCarrierTransform.position);

        Vector3 distanceVector = transform.position - aircraftCarrierTransform.position;        

        if (currentDistance < previousDistance)
        {
            AddReward(.001f * 0.1f);
            Debug.Log("Gave positive reward");
        }

        if (currentDistance < 40f) AddReward(.01f * 0.25f);
        if (currentDistance < 30f) AddReward(.01f * 0.5f);
        if (currentDistance < 20f) AddReward(.1f * 0.5f);
        if (currentDistance < 10f) AddReward(.1f * 0.6f);

        if (currentDistance > 170f) AddReward(-(.01f * 0.2f));

        if (Mathf.Abs(distanceVector.x) >= 0.85f * maxAircraftDistance.x ||
            Mathf.Abs(distanceVector.y) >= 0.85f * maxAircraftDistance.y ||
            Mathf.Abs(distanceVector.z) >= 0.85f * maxAircraftDistance.z)
        {
            transform.rotation = Quaternion.LookRotation(distanceVector);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            rb.AddForce(transform.forward * speed);
            Debug.Log("Gave negative reward for traversing beyond 85%");
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Move the agent in the random direction

        // Debug.Log("Inside the OnActionReceived");
        // Actions to control the plane
        float moveForward = actions.ContinuousActions[0];
        float turn = actions.ContinuousActions[1];

        // transform.position += randomDirection * speed * Time.deltaTime;

        float[] vectorAction = actions.ContinuousActions.Array;
        Vector3 rotationVector = transform.rotation.eulerAngles;

        // Movement in forward direction
        float moveZ = vectorAction[0];
        rb.AddForce(transform.forward * speed * moveZ);

        // Yaw (turning) left or right
        float yaw = vectorAction[1];
        smoothYaw = Mathf.MoveTowards(smoothYaw, yaw, 2f * Time.fixedDeltaTime);
        transform.Rotate(Vector3.up * yaw * turnSpeed);

        // Pitch (looking up or down)
        float pitch = vectorAction[2];
        smoothPitch = Mathf.MoveTowards(smoothPitch, pitch, 2f * Time.fixedDeltaTime);
        if (smoothPitch > 180f) smoothPitch -= 360f;
        smoothPitch = Mathf.Clamp(smoothPitch, -MaxPitchAngle, MaxPitchAngle);
        transform.Rotate(Vector3.right * smoothPitch * pitchSpeed);

        rb.AddForce(transform.forward * moveForward * speed);

        GiveRewardBasedOnDistance();

        float previousDist = Vector3.Distance(previousPosition, aircraftCarrierTransform.position);
        float currentDist = Vector3.Distance(transform.position, aircraftCarrierTransform.position);
        Vector3 distanceToShip = transform.position - aircraftCarrierTransform.position;

        if (currentDist > previousDist)
        {
            transform.rotation = Quaternion.LookRotation(distanceToShip);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            rb.AddForce(transform.forward * speed * moveZ);
        }

        // Debug.Log("Value of F18's position inside OnActionReceived after reward is" + transform.position);

        if (transform.position.magnitude > maxAircraftDistance.magnitude)
        {
            // EndEpisode();
            Debug.Log("Gave negative reward for going beyond boundaries");
            AddReward(-0.7f);
            f18Aircraft.Respawn();
            // OnEpisodeBegin();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical"); // Assuming standard Unity input setup
        continuousActions[1] = Input.GetAxis("Horizontal");

    }
}

