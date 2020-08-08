using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public GameObject head;
    public float movementSpeed;
    public float mouseSens;
    public LayerMask holdingMask;
    public float holdingSpeed;
    public float holdingDistance;
    public float throwingForce;
    public float throwingCooldown;

    private Vector3 direction;
    private Rigidbody rb;
    private float horizontalRot;
    private float verticalRot;
    private GameObject holdingObject;
    private Rigidbody holdingObjectRb;
    private float throwingCooldownTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        horizontalRot += Input.GetAxis("Mouse X") * mouseSens;
        verticalRot -= Input.GetAxis("Mouse Y") * mouseSens;

        verticalRot = Mathf.Clamp(verticalRot, -90, 90);
        
        //aim
        transform.rotation = Quaternion.Euler(0, horizontalRot, 0);
        head.transform.localRotation = Quaternion.Euler(verticalRot,
            0,
            0);

        if (Input.GetAxis("Fire1") > 0)
        {
            if (holdingObject == null && throwingCooldownTimer < 0)
            {
                PickUpProp();
            }
            Screen.lockCursor = true;
        }
        else
        {
            if (holdingObject != null)
            {
                DropProp();
            }
        }


        if (Input.GetKeyDown("escape"))
        {
            Screen.lockCursor = false;
        }

        if (Input.GetAxis("Fire2") > 0)
        {
            if (holdingObject != null && throwingCooldownTimer < 0)
            {
                holdingObjectRb.AddForce(head.transform.forward * throwingForce * holdingObjectRb.mass, ForceMode.Impulse);
                DropProp();

                throwingCooldownTimer = throwingCooldown;
            }
        }

        throwingCooldownTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.TransformDirection(direction) * movementSpeed * Time.deltaTime + transform.position);

        if(holdingObject != null)
        {
            holdingObjectRb.MovePosition(holdingObject.transform.position + (Vector3.ClampMagnitude((head.transform.forward * holdingDistance + head.transform.position) - holdingObject.transform.position, 1) * holdingSpeed * Time.deltaTime));

            //holdingObjectRb.AddForce(((head.transform.forward * holdingDistance + head.transform.position) - holdingObject.transform.position) * holdingSpeed, ForceMode.Acceleration);
            holdingObjectRb.velocity = Vector3.zero;
        }
    }

    private void PickUpProp()
    {
        RaycastHit hit;

        if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, holdingDistance * 2, holdingMask))
        {
            if (hit.transform.CompareTag("Prop"))
            {
                holdingObject = hit.transform.gameObject;
                holdingObjectRb = holdingObject.GetComponent<Rigidbody>();
            }
        }
    }

    private void DropProp()
    {
        holdingObject = null;
        holdingObjectRb = null;
    }
}
