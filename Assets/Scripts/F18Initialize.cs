using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class F18Initialize : MonoBehaviour
{
    [Header("Respawn Settings")]
    public GameObject aircraftCarrier;
    private Vector3 respawnPoint;
    private F18Agent f18Agent;
    private Rigidbody F18Rigidbody;
    public GameObject seaSurface;


    [Header("Landing Gear Touch Settings")]
    public GameObject landingGearBackLeft;
    public GameObject landingGearBackRight;
    public GameObject landingGearFront;
    private bool isBackLeftTouched, isBackRightTouched, isFrontTouched;
    private Vector3 previousVelocity = new Vector3 (0, 0, 0);

    [Header("Aircraft Landing Parameters")]
    private float landingDistance = 15f;
    public float planeSpeed = 5f;
    private float descentSpeed = 2f;

    private Vector3 bufferDistance = new Vector3(0,5f,0);

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay Triggered!");
        if (other.gameObject.CompareTag("Hull"))
        {
            Debug.Log("OnTriggerStay -> Hull Detected");
            F18Rigidbody.AddForce(Vector3.up * 12f, ForceMode.Acceleration);
            LandAircraft();
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("OnCollisionStay Triggered!");
        if (collision.gameObject.tag == "Hull")
        {
            F18Rigidbody.AddForce(Vector3.up * 9.8f, ForceMode.Acceleration);
            LandAircraft();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Inside OnCollisionEnter");
        // Check for collision with the AircraftCarrier or Sea
        if (collision.gameObject.CompareTag("AircraftCarrier") || collision.gameObject.CompareTag("Sea"))
        {
            // Check if the collision is head-on with the AircraftCarrier
            if (collision.gameObject.CompareTag("AircraftCarrier") && Vector3.Dot(transform.forward, collision.contacts[0].point - transform.position) > 0.5f)
            {
                Debug.Log("Ship Collision Head on Occured");
                Respawn();
            }
            // Check for collision with Sea
            else if (collision.gameObject.CompareTag("Sea"))
            {
                Debug.Log("Collision Occured, Plane hit the sea");
                
                // Apply an upward force to lift the aircraft
                Vector3 upwardForce = Vector3.up * F18Rigidbody.mass * 20.0f;
                F18Rigidbody.AddForce(upwardForce, ForceMode.Impulse);
                
            }
            else if (collision.gameObject.CompareTag("Hull"))
            {
                Debug.Log("Hit Hull Tag, Preparing to Land");
                LandAircraft();
            }
        }
    }


    private void Start()
    {
        f18Agent = GetComponent<F18Agent>();
        F18Rigidbody = GetComponent<Rigidbody>();
    }

    public void LandAircraft()
    {
        transform.rotation = Quaternion.Euler(0, -180, 0);

        // Start the descent.
        F18Rigidbody.velocity = new Vector3(0, -descentSpeed, 0);

        // Use raycasting to check for the landing gears' touch.
        isBackLeftTouched = Physics.Raycast(landingGearBackLeft.transform.position, Vector3.down, 0.5f);
        isBackRightTouched = Physics.Raycast(landingGearBackRight.transform.position, Vector3.down, 0.5f);
        isFrontTouched = Physics.Raycast(landingGearFront.transform.position, Vector3.down, 0.5f);

        // If all landing gears are in contact with the carrier, stop the descent.
        if (isBackLeftTouched && isBackRightTouched && isFrontTouched)
        {
            previousVelocity = F18Rigidbody.velocity;
            F18Rigidbody.velocity = Vector3.zero;
            F18Rigidbody.isKinematic = true;
            Debug.Log("Landing Successfull");
            f18Agent.AddReward(3.0f);


            // Resetting Kinematic and velocity
            Debug.Log("Resetting Velocity and Kinematic after landing is successfull.");
            F18Rigidbody.isKinematic = false;
            F18Rigidbody.velocity = previousVelocity;
        }
    }

    private void FixedUpdate()
    {

        // Get distance from the ground or aircraft carrier
        float groundShipLevel = Mathf.Max(0, aircraftCarrier.transform.position.y);
        float groundSeaLevel = Mathf.Max(0, seaSurface.transform.position.y);

        float heightAboveGround = transform.position.y - groundShipLevel;
        float heightAboveSeaLevel = transform.position.y - groundSeaLevel;
        

        // If the aircraft is very close to the ground, we need to prevent the distance from becoming too small, 
        // as this will result in a very large force. 
        // So, we clamp the minimum value of heightAboveGround to a small positive value.
        if (transform.position.y - heightAboveGround < 10f)
        {
            heightAboveGround = Mathf.Max(heightAboveGround, 1.0f);
        }

        // Calculate force using inverse relationship with distance, then applying an upward force.
        // If carrier is near sea, add additional force to push the aircraft on top
        float upwardForceMultiplier = (1.0f / heightAboveGround) + (1.0f / heightAboveSeaLevel);
        float baseUpwardForce = F18Rigidbody.mass * Physics.gravity.magnitude;

        // Apply the upward force adjusted by the multiplier. upwardForceBonus is used when the flight is close to sea level
        F18Rigidbody.AddForce(Vector3.up * baseUpwardForce * Mathf.Abs(upwardForceMultiplier) * 1.5f);
        float distanceToCarrierForLanding = Vector3.Distance(transform.position, aircraftCarrier.transform.position);

        // Reduce the speed of the aircraft as it approaches the carrier.
        float speedFactor = Mathf.Clamp(distanceToCarrierForLanding / 75f, 0.2f, 1f);
        F18Rigidbody.velocity = F18Rigidbody.velocity.normalized * planeSpeed * speedFactor;

        // Check if the aircraft is directly above the carrier and within landing distance.
        Vector3 relativePosition = aircraftCarrier.transform.position - transform.position;
        bool isDirectlyAbove = Mathf.Abs(relativePosition.x) < 0.5f && Mathf.Abs(relativePosition.z) < 0.5f;

        if (isDirectlyAbove && distanceToCarrierForLanding <= landingDistance)
        {
            // Trigger the landing procedure
            Debug.Log("Directly Above Aircraft");

            f18Agent.AddReward(0.5f);
            LandAircraft();
        }
    }


    void SetRandomRespawnPoint()
    {
        float xOffset = Random.Range(-100f, 100f);
        float zOffset = Random.Range(-100f, 100f);
        float yOffset = Random.Range(30f, 100f);

        // Set the respawn point above the aircraft carrier with the offsets
        respawnPoint = aircraftCarrier.transform.position + new Vector3(xOffset, yOffset, zOffset);
        
    }

    public void Respawn()
    {

        SetRandomRespawnPoint();
        transform.position = respawnPoint;
        transform.rotation = Quaternion.identity;

        // Reset the plane's velocity and angular velocity
        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isBackLeftTouched = false;
        isBackRightTouched = false;
        isFrontTouched = false;
    }

}

