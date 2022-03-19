using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(Rigidbody))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    private Camera cam;
    private Rigidbody rBody;
    private Vector3 moveDir;
    private Vector3 prevMousePos;

    private void Start()
    {
        cam = GetComponent<Camera>();
        rBody = GetComponent<Rigidbody>();
        prevMousePos = Input.mousePosition;
    }

    private void Update()
    {
        moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            moveDir += transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDir += -transform.right;
        if (Input.GetKey(KeyCode.S))
            moveDir += -transform.forward;
        if (Input.GetKey(KeyCode.D))
            moveDir += transform.right;
        if (Input.GetKey(KeyCode.Q))
            moveDir += -transform.up;
        if (Input.GetKey(KeyCode.E))
            moveDir += transform.up;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector3 mouseDelta = Input.mousePosition - prevMousePos;
            transform.localRotation = Quaternion.Euler(
                transform.localRotation.eulerAngles.x + -mouseDelta.y * rotateSpeed * Time.deltaTime,
                transform.localRotation.eulerAngles.y + mouseDelta.x * rotateSpeed * Time.deltaTime,
                0
            );
        }

        prevMousePos = Input.mousePosition;
    }

    private void FixedUpdate()
    {
        rBody.angularVelocity = Vector3.zero;
        //If we don't have a move direction, only zero velocity if it's not zeroed already.
        if (moveDir != Vector3.zero || rBody.velocity != Vector3.zero)
        {
            rBody.velocity = moveDir.normalized * moveSpeed * Time.deltaTime;
        }
    }
}
