using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public GameObject bubbleBurstPrefab;
    float motionAngle = 0.0f;
    private Vector3 wobbleVelocity;
    private float motionSpeed;
    private float motionWidth;
    public Vector3 Origin { get; set; }
    public float MaxDistance { get; set; }
    private Vector3 GetBubbleMotion(float motionSpeed, float motionWidth, float motionGravity)
    {
        motionAngle += Mathf.PI / 180.0f * motionSpeed;
        var distanceFromOrigin = (transform.position - Origin).magnitude;
        var scaler = Mathf.Clamp(MaxDistance - distanceFromOrigin, -0.5f, 1f);
        return new Vector3(Mathf.Sin(motionAngle) * motionWidth, motionGravity, 0.0f) * scaler;
    }
    private void Start()
    {
        // Randomize the motion of bubbles
        motionSpeed = Random.Range(0.1f, 0.5f);
        motionWidth = Random.Range(-0.5f, 0.5f);
    }

    private void Update()
    {
        wobbleVelocity = GetBubbleMotion(motionSpeed, motionWidth, 0.05f);
        Vector3 bubblePos = new Vector3(transform.position.x, transform.position.y, Origin.z);
        var newPosition = bubblePos + wobbleVelocity * Time.deltaTime;
        transform.position = newPosition;
        Debug.Log($"Bubble {name} new position: {transform.position}");
    }
}
