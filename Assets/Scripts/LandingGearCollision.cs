using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingGearCollision : MonoBehaviour
{
    public delegate void LandingGearTouchdown();
    public static event LandingGearTouchdown OnLandingGearTouchdown;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hull"))
        {
            OnLandingGearTouchdown?.Invoke();
        }
    }
}


