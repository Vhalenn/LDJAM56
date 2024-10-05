using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Header("Param")]
    [SerializeField] private float maxAcce = 5;
    [SerializeField] private float maxSpeed = 5;

    [Header("Storage")]
    [SerializeField] private Camera mainCam;
    private Transform camTransform;
    [SerializeField] private Vector2 playerInput;
    [SerializeField] private Vector3 velocity;
    public Vector3 Velocity { get => velocity; set => velocity = value; }

    private void Start()
    {
        if(!mainCam) mainCam = Camera.main;
    }


    private void Update()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        MoveChara();
    }

    private void MoveChara() // On Fixed Update
    {
        velocity = rb.linearVelocity;

        Vector3 camDir = FollowingCamAngle(playerInput);
        AdjustVelocity(camDir * maxSpeed);

        rb.linearVelocity = velocity;
    }

    private void AdjustVelocity(Vector3 desiredVelocity)
    {/*
        if (!OnGround && noAirControl)
        {
            return;
            // This function was preventing doing a clean parabol in the air
        }
        */
        
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        
        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        //float acceleration = OnGround ? maxAcce;
        float maxSpeedChange = maxAcce * Time.fixedDeltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    // Utility

    private Vector3 ContactNormal => Vector3.up;

    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - ContactNormal * Vector3.Dot(vector, ContactNormal);
    }
    

    public Vector3 GetCamDir()
    {
        if (!mainCam) return Vector3.zero;
        if (!camTransform) camTransform = mainCam.transform;
        if (!camTransform) return Vector3.zero;

        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return forward; // forward
    }

    private Vector3 FollowingCamAngle(Vector2 input)
    {
        if (!mainCam) return Vector3.zero;
        if (camTransform == null) camTransform = mainCam.transform;
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 dir = forward * input.y + right * input.x;
        return dir;
    }
}
