using System;
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

    public GameObject gamePanel, mapPanel, startPanel, loadingPanel;
    public Slider slider;
    public RawImage mapImage;
    public Joystick cameraJoystick;
    public Button[] dockPlacementButtons;
    public Button[] teamSelectionButtons;
    public Button playbutton, quitButton, okButton;
    int selectedHQindex = -1, selectedTeamIndex = -1;
    Color32 playerCol, enemyCol;

    void Start()
    {
        startPanel.SetActive(true);
        GlobalEventManager.gameStarted += onGameStarted;
        GlobalEventManager.gameConfigured += onGameConfigured;
        GlobalEventManager.gameFinished += onGameFinished;
    }

    void Update()
    {
        CameraJoystickUpdate();
        SliderValueUpdate();
        ClickUpdate();
    }

    public void RestartGame()
    {
        startPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void uiPlayButtonClick()
    {
        GlobalEventManager.invokeGameStarted();
    }

    public void uiSelectHQButtonClick(int index)
    {
        if (selectedHQindex != index)
        {
            if (selectedHQindex > -1)
            {
                dockPlacementButtons[selectedHQindex].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
            }
            selectedHQindex = index;
            dockPlacementButtons[selectedHQindex].GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 0);
        }
    }

    public void uiSelectTeamButtonClick(int index)
    {
        if (selectedTeamIndex != index)
        {
            if (selectedTeamIndex > -1)
            {
                teamSelectionButtons[selectedTeamIndex].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
            }
            selectedTeamIndex = index;
            teamSelectionButtons[selectedTeamIndex].GetComponent<RectTransform>().localScale = new Vector3(1.15f, 1.15f, 0);
        }
    }

    public void uiCompleteSelectionbuttonClick()
    {
        if (selectedHQindex + selectedTeamIndex > -1)
        {
            GameController.Team playerTeam = (selectedTeamIndex == 0 ? GameController.Team.white : GameController.Team.black);
            GlobalEventManager.invokeGameConfigured(playerTeam, selectedHQindex);
        }
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

    public void setupButtons(int x, int z, int width, int length, int index, Vector3 place)
    {
        Vector3[] mapCorners = new Vector3[4];
        mapImage.GetComponent<RectTransform>().GetWorldCorners(mapCorners);

        float Xposition = (x * 1f / width) * (mapCorners[3].x - mapCorners[0].x) + mapCorners[0].x;
        float Yposition = (z * 1f / length) * (mapCorners[1].y - mapCorners[0].y) + mapCorners[0].y;

        var rectTransform = dockPlacementButtons[index].GetComponent<RectTransform>();
        dockPlacementButtons[index].GetComponent<DockButton>().setPlacement(place);
        rectTransform.SetParent(transform.GetChild(2).GetComponent<RectTransform>());
        rectTransform.position = new Vector2(Xposition, Yposition);
    }

    public void TestSwapTeams()
    {
        GameController.instance.TestSwapTeams();
    }

    public void onGameStarted(){
        loadingPanel.SetActive(true);
        startPanel.SetActive(false);
        mapPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }

    public void onGameConfigured(GameController.Team team, int HQindex){
        loadingPanel.SetActive(true);
        teamSelectionButtons[selectedTeamIndex].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
        dockPlacementButtons[selectedHQindex].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
        mapPanel.SetActive(false);
        gamePanel.SetActive(true);
        loadingPanel.SetActive(false);
        slider.value = 0.4f;
    }

    public void onGameFinished(){
        gamePanel.SetActive(false);
        startPanel.SetActive(true);
    }



}
