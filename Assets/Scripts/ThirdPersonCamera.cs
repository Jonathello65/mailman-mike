using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    private Camera cam;

    [SerializeField] private float distance = 3.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    [SerializeField]private float sensitvityX = 4.0f;
    [SerializeField]private float sensitiviyY = 1.0f;

    private const float Y_ANGLE_MIN = -15.0f;
    private const float Y_ANGLE_MAX = 85.0f;

    void Start()
    {
        // Reference to main camera. Removes and confines cursor
        cam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        currentX = target.eulerAngles.y;
    }

    void Update()
    {
        // Gets mouse movement, confines vertical movement so camera doesn't loop around player or go under the ground
        currentX += Input.GetAxis("Mouse X");
        currentY -= Input.GetAxis("Mouse Y");

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    void LateUpdate()
    {
        // Rotates camera using mouse movements
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = target.position + rotation * dir;
        transform.LookAt(target.position);
    }
}
