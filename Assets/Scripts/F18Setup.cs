using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class F18Setup : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
}

