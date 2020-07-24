using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    #region Singleton
    public static HUDManager instance;
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

    public GameObject gamePanel;
    public Slider slider;
    public Joystick cameraJoystick;
    Color32 playerCol, enemyCol;

    void Start()
    {
        gamePanel.SetActive(true);
        slider.value = 0.4f;
    }

    void Update()
    {
        CameraJoystickUpdate();
        SliderValueUpdate();
        ClickUpdate();
    }

    void SliderValueUpdate()
    {
        CameraController.instance.CalculateCameraZoom(slider.value);
    }

    void CameraJoystickUpdate()
    {
        float hor = cameraJoystick.Horizontal, ver = cameraJoystick.Vertical;
        if (Mathf.Abs(ver * hor) > 0)
        {
            CameraController.instance.CalculateCameraMovement(hor, ver);
        }
    }

    void ClickUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverGameObject())
        {
            Ray ray = GameController.instance.cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1500, GameController.instance.shipsLayerMask))
            {
                GameController.instance.changeShip(hit.collider.GetComponent<Ship>());
            }
            else if (Physics.Raycast(ray, out hit, 1500, GameController.instance.waterLayerMask))
            {
                GameController.instance.playerMovement(hit.point);
            }
        }

    }

    public static bool IsPointerOverGameObject()
    {
        //check mouse
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return true;

        //check touch
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                return true;
        }

        return false;
    }

}
