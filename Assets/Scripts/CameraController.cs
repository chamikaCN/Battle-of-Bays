using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singleton

    public static CameraController instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("many cameracontollers");
            return;
        }

        instance = this;
    }
    #endregion
    Transform target;

    Vector3 offset;
    float yawSpeed, updownSpeed;
    float currrentZoom, currentYaw, pitch;
    float currentUpdown;
    


    void Start()
    {
        currrentZoom = 1.4f;
        currentYaw = 1f;
        pitch = 5f;
        currentUpdown = 1f;
        offset = new Vector3(100, 30, 120);
        updownSpeed = 8f;
        yawSpeed = 12f;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 additionVector = target.position + offset * currrentZoom;
            Mathf.Clamp(additionVector.y, 1.5f, 6f);
            transform.position = additionVector;
            transform.LookAt(target.position + Vector3.up * pitch);

            transform.RotateAround(target.position, Vector3.up, currentYaw);
            currentUpdown = Mathf.Clamp(currentUpdown, 0f, 15f);
            transform.RotateAround(target.position, transform.right, currentUpdown);
        }
    }

    public void setTransform(Transform transform)
    {
        target = transform;

    }

    public void CalculateCameraMovement(float horizontal, float vertical)
    {
        currentYaw -= horizontal * yawSpeed * Time.deltaTime * 3f;
        currentUpdown -= vertical * updownSpeed * Time.deltaTime * 4f;
    }

    public void CalculateCameraZoom(float requestZoom)
    {
        currrentZoom = (requestZoom)*1.2f + 0.5f;
        // currrentZoom = requestZoom;
        // Debug.Log("req "+ requestZoom);
    }

    public void changeOffset(string team)
    {
        if (team == "Blue")
        {
            offset = new Vector3(-offset.x, offset.y, offset.z);
        }
    }

}
