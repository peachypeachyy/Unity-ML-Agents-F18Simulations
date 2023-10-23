using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    public WaveGenerator waveGenerator;

    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    // Rotation settings
    public float rotationFactor = 1f;  // Control the force of rotation
    public Vector3[] floatPoints;      // Points on the object to test buoyancy and induce rotation

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.AddForceAtPosition(Physics.gravity / floatPoints.Length, transform.position, ForceMode.Acceleration);
        foreach (var point in floatPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(point);
            float waveYPos = waveGenerator.GetWaveYPos(worldPoint);
            bool isSubmerged = worldPoint.y < waveYPos;

            if (isSubmerged)
            {
                float displacementMultiplier = Mathf.Clamp01((waveYPos - worldPoint.y) / depthBeforeSubmerged) * displacementAmount;
                rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), worldPoint, ForceMode.Acceleration);
                rb.AddForceAtPosition(-rb.velocity * waterDrag * Time.fixedDeltaTime, worldPoint, ForceMode.VelocityChange);

                // Apply the rotational force
                rb.AddTorque((worldPoint - transform.position) * rotationFactor * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }

        rb.AddTorque(-rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}
