using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    public float waveFrequency = 1.0f;
    public float waveHeight = 0.5f;
    public Vector2 waveDirection = new Vector2(1.0f, 0.0f);

    private float timeSinceLastWaveCycle = 0.0f;
    private const float SINE_PERIOD = 2 * Mathf.PI;  // A full cycle in radians for the sine function

    private void Update()
    {
        // Calculate how much time has passed since the last wave cycle
        timeSinceLastWaveCycle += Time.deltaTime * waveFrequency;

        if (timeSinceLastWaveCycle > SINE_PERIOD)
        {
            // Reset timer
            timeSinceLastWaveCycle -= SINE_PERIOD;

            // Randomize the wave direction's X component
            waveDirection.x = Random.Range(0.0f, 1.0f);
        }
    }

    public float GetWaveYPos(Vector3 position)
    {
        return waveHeight * Mathf.Sin(position.x * waveFrequency + Time.time);
    }
}
