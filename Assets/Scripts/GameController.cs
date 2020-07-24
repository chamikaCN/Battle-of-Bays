using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Singleton
    public static GameController instance;
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
    MapGenerator generator;
    public Camera cam;
    System.Random random;
    int gameSeed;
    public LayerMask waterLayerMask, shipsLayerMask;
    public GameObject blackShip, whiteShip, dock;
    List<GameObject> placedBlackShips, placedWhiteShips, placedDocks;
    Ship currentShip;
    void Start()
    {
        generator = GetComponent<MapGenerator>();
        random = new System.Random();
        gameSeed = random.Next(10000);
        generator.GenerateMap(gameSeed);
        placedBlackShips = generator.PlaceShips(blackShip, 3);
        placedDocks = generator.PlaceDocks(dock, 5);
        placedWhiteShips = generator.PlaceShips(whiteShip, 3);
        currentShip = placedBlackShips[0].GetComponent<Ship>();
        CameraController.instance.setTransform(currentShip.transform);
        currentShip.activateSelector();
    }

    public void playerMovement(Vector3 point){
        currentShip.moveToPoint(point);
    }

    public void changeShip(Ship newShip){
        currentShip.deactivateSelector();
        currentShip = newShip;
        CameraController.instance.setTransform(currentShip.transform);
        currentShip.activateSelector();
    }

    public Camera getCamera(){
        return cam;
    }
}
