using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractory : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform throwOrigin;
    public float throwForce = 10f;
    public float resetTime = 3f; 
    public float aimSens = 100f;
    public int trajectory = 30;

    private Rigidbody rb;
    private Vector3 intVelocity;
    private Vector3 startPos;
    private bool isThrown = false;
    private bool isCursorLocked = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = trajectory + 1;

        LockCursor(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            LockCursor(isCursorLocked);
        }

        if (!isThrown)
        {
            Aim();
            DrawTrajectory();
        }

        if (Input.GetMouseButtonDown(0) && !isThrown)
        {
            Throw();
        }
    }

    void Aim()
    {
        float horizontal = Input.GetAxis("Mouse X") * aimSens * Time.deltaTime;
        float vertical = -Input.GetAxis("Mouse Y") * aimSens * Time.deltaTime;

        throwOrigin.Rotate(Vector3.up, horizontal, Space.World);
        throwOrigin.Rotate(Vector3.right, vertical, Space.Self);
    }

    void DrawTrajectory()
    {
        intVelocity = throwOrigin.forward * throwForce;
        Vector3 startPos = throwOrigin.position;

        Vector3[] trajectoryPoints = new Vector3[trajectory + 1];
        trajectoryPoints[0] = startPos;

        for (int i = 1; i <= trajectory; i++)
        {
            float t = i / (float)trajectory;
            trajectoryPoints[i] = Trajectory(startPos, intVelocity, t);
        }

        lineRenderer.SetPositions(trajectoryPoints);
    }

    void Throw()
    {
        intVelocity = throwOrigin.forward * throwForce;
        rb.velocity = intVelocity;
        isThrown = true;
        lineRenderer.positionCount = 0;
        Invoke("ResetPosition", resetTime);
    }

    void ResetPosition()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPos;
        isThrown = false;
        lineRenderer.positionCount = trajectory + 1;
    }

    Vector3 Trajectory(Vector3 startPos, Vector3 startVel, float time)
    {
        Vector3 gravity = Physics.gravity;
        return startPos + startVel * time + 0.5f * gravity * time * time;
    }

    void LockCursor(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}
