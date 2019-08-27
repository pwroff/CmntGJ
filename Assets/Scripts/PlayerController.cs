using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public Transform head;

    public float accelerationSpeed = 5f;
    public float maxMovementSpeed = 5f;
    public float sprintModifier = 2f;
    public float maxVerticalRotation = 85f;
    public float mouseSensitivity = 1f;
    public float rotationSpeed = 180f;
    public float walkCycleSpeed = 0.2f;
    public float walkCycleMaxOffset = 0.05f;

    public AnimationCurve walkingCurve;

    private float initialHeadYOffset;

    Footsteps footstepController;
    public float footstepCooldown = 0.6f;
    public float minSpeedForFootsteps = 0.1f;
    float lastFootstep;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        initialHeadYOffset = head.localPosition.y;
        footstepController = GetComponent<Footsteps>();
    }

    void Move(Vector3 movement, float maxMovementSpeed)
    {
        movement += rb.velocity;
        movement.x = Mathf.Clamp(movement.x, -maxMovementSpeed, maxMovementSpeed);
        movement.z = Mathf.Clamp(movement.z, -maxMovementSpeed, maxMovementSpeed);
        rb.velocity = movement;
    }

    private bool wasSprint = false;
    void ProcessMovementInput()
    {
        wasSprint = false;
        rb.velocity = rb.velocity * .25f;
        Vector3 movementForFrame = Vector3.zero;
        movementForFrame += transform.forward * Input.GetAxis("Vertical") * accelerationSpeed;
        movementForFrame += transform.right * Input.GetAxis("Horizontal") * accelerationSpeed;
        float maxmvSpeedForframe = maxMovementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementForFrame *= sprintModifier;
            maxmvSpeedForframe *= sprintModifier;
            wasSprint = true;
        }

        //not sure how you want to do footsteps so I'll just put somethign simple here with a timeout if
        //the player is walking faster than a certain speed, feel free to replace with something better.
        if (movementForFrame.sqrMagnitude > minSpeedForFootsteps &&
            (Time.time - lastFootstep > footstepCooldown  || wasSprint && Time.time - lastFootstep > footstepCooldown * 0.5f)) {
            lastFootstep = Time.time;
            footstepController.PlayFootstepSound();
        }

        Move(movementForFrame, maxmvSpeedForframe);
    }

    void Turn(float deg)
    {
        transform.Rotate(new Vector3(0, deg, 0));
    }

    void Look(float deg)
    {
        Vector3 localRot = head.localRotation.eulerAngles;
        localRot.x += deg;
        if (localRot.x >= 345 - maxVerticalRotation)
        {
            localRot.x -= 360;
        }
        localRot.x = Mathf.Clamp(localRot.x, -1 * maxVerticalRotation, maxVerticalRotation);
        head.localRotation = Quaternion.Euler(localRot);
    }

    void ProcessMouseInput()
    {
        Turn(Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * rotationSpeed);
        Look(-Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * rotationSpeed);
    }

    Vector3 beforMovePos = Vector3.zero;
    
    void Update()
    {
        ProcessMovementInput();
        ProcessMouseInput();
    }

    float totalTraveled = 0;

    private void LateUpdate()
    {
        totalTraveled += ((transform.position - beforMovePos).magnitude)* (wasSprint ? walkCycleSpeed / (sprintModifier *.5f) : walkCycleSpeed) ;
        beforMovePos = transform.position;
        totalTraveled = totalTraveled % 1.0f;
        var lp = head.localPosition;
        lp.y = initialHeadYOffset + walkingCurve.Evaluate(totalTraveled) * walkCycleMaxOffset;
        head.localPosition = lp;
    }
}
