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
    public List<GameObject> placedPlayerShips { get; set; }
    public List<GameObject> placedEnemyShips { get; set; }
    public List<GameObject> placedDocks { get; set; }
    public GameObject playerHQ { get; set; }
    public GameObject EnemyHQ { get; set; }
    Ship currentShip;

    void Start()
    {
        GlobalEventManager.gameFinished += onGameFinished;
        GlobalEventManager.gameConfigured += onGameConfigured;
        GlobalEventManager.gamePaused += onGamePaused;
        GlobalEventManager.gameResumed += onGameResumed;
    }

    public void SelectPlayerShip()
    {

        currentShip = placedPlayerShips[0].GetComponent<Ship>();
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
                GlobalEventManager.invokeGameFinished(playerTeam);
                HUDManager.instance.RestartGame();
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
                GlobalEventManager.invokeGameFinished(playerTeam == Team.white ? Team.black : Team.white);
                HUDManager.instance.RestartGame();
            }

        }
    }

    public GameController.Team getTeam() { return playerTeam; }

    public void setTeam(Team team)
    {
        playerTeam = team;
    }

    public void spawnShip(Team team, Vector3 baseLocation, int mapPosition)
    {
        if (playerTeam == team)
        {
            placedPlayerShips.Add(GetComponent<MapGenerator>().PlaceShip(team, baseLocation, mapPosition));
        }
        else
        {
            placedEnemyShips.Add(GetComponent<MapGenerator>().PlaceShip(team, baseLocation, mapPosition));
        }
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

    private void onGameConfigured(Team pTeam, int HQindex)
    {
        setTeam(pTeam);
    }


    public void onGameFinished(Team playerTeam)
    {

    }

    public void onGamePaused()
    {
        Time.timeScale = 0;
    }

    public void onGameResumed()
    {
        Time.timeScale = 1;
    }

}
