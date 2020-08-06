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

    public float Xdiv,Ydiv;
    public GameObject gamePanel,mapPanel;
    public Slider slider;
    public Joystick cameraJoystick;
    public Button[] buttons;
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
        // if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
        // {
        //     CameraController.instance.CalculateCameraZoom(Input.GetAxis("Mouse ScrollWheel")/10);
        // }
        // if (Input.touchCount == 2)
        // {
        //     Debug.Log("got two");
        //     Touch touchZero = Input.GetTouch(0);
        //     Touch touchOne = Input.GetTouch(1);

        //     Vector2 tzPrev = touchZero.position - touchZero.deltaPosition;
        //     Vector2 toPrev = touchOne.position - touchOne.deltaPosition;

        //     float prevMag = (tzPrev - toPrev).magnitude;
        //     float currMag = (touchZero.position - touchOne.position).magnitude;

        //     float dif = currMag - prevMag;
        //     Debug.Log(dif);
        //     CameraController.instance.CalculateCameraZoom(dif*0.01f);
        // }
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
                if (GameController.instance.getTeam() == hit.collider.GetComponent<Ship>().getTeam())
                {
                    GameController.instance.changeShip(hit.collider.GetComponent<Ship>());
                }
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

    public void setupButtons(int x, int z,int width, int length, int index, Vector3 place){
        float screenWidth = GetComponent<CanvasScaler>().referenceResolution.x;
        float screenHeight = GetComponent<CanvasScaler>().referenceResolution.y;

        float Xposition = (x * 1f / width) * screenWidth;
        float Yposition = (z * 1f / length) * screenHeight;

        var rectTransform = buttons[index].GetComponent<RectTransform>();
        buttons[index].GetComponent<DockButton>().setPlacement(place);
        rectTransform.SetParent(transform.GetChild(2).GetComponent<RectTransform>());
        rectTransform.position = new Vector2(Xposition/Xdiv, Yposition/Ydiv);
    }

    public void selectHQ(int index)
    {
        GameController.instance.selectHQ(index);
        mapPanel.SetActive(false);
    }

    

    public void TestSwapTeams(){
        GameController.instance.TestSwapTeams();
    }

    

}
