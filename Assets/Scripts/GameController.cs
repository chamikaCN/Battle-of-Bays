﻿using System.Collections;
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

        setTeam(PlayerPrefs.GetString("Team", "Black") == "Black" ? Team.black : Team.white);

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

    void Start()
    {
        Debug.Log("tiger " + System.DateTime.Now);
        generator = GetComponent<MapGenerator>();
        random = new System.Random();
        gameSeed = random.Next(10000);
        generator.GenerateMap(gameSeed);
        generator.calculateDockPlacements(6);
        generator.draw2DMapTexture();
        placedPlayerShips = generator.PlaceShips(playerTeam == Team.black ? blackShip : whiteShip, 3);
        placedEnemyShips = generator.PlaceShips(playerTeam == Team.black ? whiteShip : blackShip, 3);
        currentShip = placedPlayerShips[0].GetComponent<Ship>();
        CameraController.instance.setTransform(currentShip.transform);
        currentShip.activatePlayerControl();
        currentShip.activateSelector();
    }

    public void selectHQ(int index)
    {
        playerHQ = generator.placeHQ(playerTeam == Team.black ? blackHQ : whiteHQ, index);
        EnemyHQ = generator.placeHQ(playerTeam == Team.black ? whiteHQ : blackHQ, random.Next(0, 5));
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
        foreach (GameObject item in placedDocks)
        {
            if (item.GetComponent<Dock>().getAllyShips().Contains(ship)) { item.GetComponent<Dock>().removeAllyShip(ship); }
            else if (item.GetComponent<Dock>().getEnemyShips().Contains(ship)) { item.GetComponent<Dock>().removeEnemyShip(ship); }
        }
        if (playerHQ.GetComponent<HQ>().getAllyShips().Contains(ship)) { playerHQ.GetComponent<HQ>().removeAllyShip(ship); }
        else if (playerHQ.GetComponent<HQ>().getEnemyShips().Contains(ship)) { playerHQ.GetComponent<HQ>().removeEnemyShip(ship); }
        if (EnemyHQ.GetComponent<HQ>().getAllyShips().Contains(ship)) { EnemyHQ.GetComponent<HQ>().removeAllyShip(ship); }
        else if (EnemyHQ.GetComponent<HQ>().getEnemyShips().Contains(ship)) { EnemyHQ.GetComponent<HQ>().removeEnemyShip(ship); }

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
            }

        }
    }

    public GameController.Team getTeam() { return playerTeam; }

    void setTeam(Team team)
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
}
