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
    public enum Team { white, black, neutral }
    Team playerTeam = Team.black;
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
        currentShip.activatePlayerControl();
        currentShip.activateSelector();
    }

    public void playerMovement(Vector3 point)
    {
        currentShip.moveToPoint(point);
    }

    public void changeShip(Ship newShip)
    {
        currentShip.deactivatePlayerControl();
        currentShip.deactivateSelector();
        currentShip = newShip;
        CameraController.instance.setTransform(currentShip.transform);
        currentShip.activateSelector();
        currentShip.activatePlayerControl();
    }

    public Camera getCamera()
    {
        return cam;
    }

    public void DestroyCheck(Ship ship)
    {
        foreach (GameObject item in placedDocks)
        {
            if (item.GetComponent<Dock>().getAllyShips().Contains(ship)) { item.GetComponent<Dock>().removeAllyShip(ship); }
            else if (item.GetComponent<Dock>().getEnemyShips().Contains(ship)) { item.GetComponent<Dock>().removeEnemyShip(ship); }
        }
        if (ship == currentShip)
        {
            if (placedBlackShips.Count > 1)
            {
                placedBlackShips.Remove(ship.gameObject);
                currentShip = placedBlackShips[0].GetComponent<Ship>();
                CameraController.instance.setTransform(currentShip.transform);
                currentShip.activateSelector();
                foreach (GameObject s in placedWhiteShips)
                {
                    s.GetComponent<Ship>().removeEnemy(ship);
                }
                ship.getDestroyed();

            }
            else
            {
                Debug.Log("Game Over Won");
            }
        }
        else if (ship.getTeam() == playerTeam)
        {
            placedBlackShips.Remove(ship.gameObject);
            foreach (GameObject s in placedWhiteShips)
            {
                s.GetComponent<Ship>().removeEnemy(ship);
            }
            ship.getDestroyed();
        }
        else
        {
            if (placedWhiteShips.Count > 1)
            {
                placedWhiteShips.Remove(ship.gameObject);
                foreach (GameObject s in placedBlackShips)
                {
                    s.GetComponent<Ship>().removeEnemy(ship);
                }
                ship.getDestroyed();
            }
            else
            {
                Debug.Log("Game Over Lost");
            }

        }
        
    }
}
