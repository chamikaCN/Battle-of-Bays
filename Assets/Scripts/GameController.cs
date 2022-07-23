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
    Team playerTeam;
    MapGenerator generator;
    public Camera cam;
    System.Random random;
    int gameSeed;
    public LayerMask waterLayerMask, shipsLayerMask, docksLayerMask;
    public GameObject blackShip, whiteShip, dock, blackHQ, whiteHQ;
    List<GameObject> placedPlayerShips, placedEnemyShips, placedDocks;
    GameObject playerHQ, EnemyHQ;
    Ship currentShip;

    public void Start(){
        GlobalEventManager.gameFinished += onGameFinished;
    }

    public void MapGeneration()
    {
        generator = GetComponent<MapGenerator>();
        random = new System.Random();
        gameSeed = random.Next(10000);
        generator.GenerateMap(gameSeed);
        generator.calculateDockPlacements(6);
        generator.drawTexture();
    }


    public void ObjectPlacement()
    {
        generator.BuildNavmesh();
        placedPlayerShips = generator.PlaceShips(playerTeam == Team.black ? blackShip : whiteShip, 3, playerHQ.transform.position, true);
        placedEnemyShips = generator.PlaceShips(playerTeam == Team.black ? whiteShip : blackShip, 3, EnemyHQ.transform.position, false);
        currentShip = placedPlayerShips[0].GetComponent<Ship>();
        CameraController.instance.setTransform(currentShip.transform);
        currentShip.activatePlayerControl();
        currentShip.activateSelector();
    }

    public void selectHQ(int index)
    {
        playerHQ = generator.placePlayerHQ(playerTeam == Team.black ? blackHQ : whiteHQ, index);
        EnemyHQ = generator.placeEnemyHQ(playerTeam == Team.black ? whiteHQ : blackHQ);
        placedDocks = generator.PlaceDocks(dock);
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

        if (ship == currentShip)
        {
            if (placedPlayerShips.Count > 1)
            {
                placedPlayerShips.Remove(ship.gameObject);
                currentShip = placedPlayerShips[0].GetComponent<Ship>();
                CameraController.instance.setTransform(currentShip.transform);
                currentShip.activateSelector();
                foreach (GameObject s in placedEnemyShips)
                {
                    s.GetComponent<Ship>().removeEnemy(ship);
                }
                ship.getDestroyed();

            }
            else
            {
                Debug.Log("Game Over Won");
                GlobalEventManager.invokeGameFinished();
                HUDManager.instance.StartGame();
            }
        }
        else if (ship.getTeam() == playerTeam)
        {
            placedPlayerShips.Remove(ship.gameObject);
            foreach (GameObject s in placedEnemyShips)
            {
                s.GetComponent<Ship>().removeEnemy(ship);
            }
            ship.getDestroyed();
        }
        else
        {
            if (placedEnemyShips.Count > 1)
            {
                placedEnemyShips.Remove(ship.gameObject);
                foreach (GameObject s in placedPlayerShips)
                {
                    s.GetComponent<Ship>().removeEnemy(ship);
                }
                ship.getDestroyed();
            }
            else
            {
                Debug.Log("Game Over Lost");
                GlobalEventManager.invokeGameFinished();
                HUDManager.instance.StartGame();
            }

        }
    }

    public GameController.Team getTeam() { return playerTeam; }

    public void setTeam(Team team)
    {
        playerTeam = team;
    }

    public void TestSwapTeams()
    {
        currentShip.deactivatePlayerControl();
        currentShip.deactivateSelector();
        setTeam(playerTeam == GameController.Team.black ? GameController.Team.white : GameController.Team.black);
        List<GameObject> temp = new List<GameObject>(placedPlayerShips);
        placedPlayerShips = placedEnemyShips;
        placedEnemyShips = temp;
        currentShip = placedPlayerShips[0].GetComponent<Ship>();
        CameraController.instance.setTransform(currentShip.transform);
        currentShip.activateSelector();
        currentShip.activatePlayerControl();

    }

    public void onGameFinished() {
        generator.clearPlacedObjects();
    }

}
