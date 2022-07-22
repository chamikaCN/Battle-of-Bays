using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : MonoBehaviour
{
    List<Ship> enemyShips, allyShips;
    public GameController.Team dockTeam;
    Cannon cannon;
    int maxHealth = 10, health;
    StatusBar statusBar;
    Canvas statusCanvas;
    GameObject indicatorB, indicatorW;

    void Awake()
    {
        statusCanvas = GetComponentInChildren<Canvas>();
        statusBar = statusCanvas.gameObject.GetComponentInChildren<StatusBar>();
        enemyShips = new List<Ship>();
        allyShips = new List<Ship>();
        indicatorB = transform.GetChild(2).gameObject;
        indicatorW = transform.GetChild(3).gameObject;
    }
    void Start()
    {
        GlobalEventManager.shipDestroyed += onShipDestroyed;
        cannon = transform.GetChild(1).GetComponent<Cannon>();
        statusBar.setMaxHealth(maxHealth);
        health = maxHealth;
    }

    void Update()
    {
        foreach (Ship ally in allyShips)
        {
            ally.RegenarateHealth();
        }
        if (enemyShips.Count > 0)
        {
            cannon.attackToPoint(enemyShips[0].transform.position);
        }

    }

    public void addEnemyShip(Ship ship)
    {
        enemyShips.Add(ship);
    }

    public void addAllyShip(Ship ship)
    {
        allyShips.Add(ship);
    }

    public void removeEnemyShip(Ship ship)
    {
        enemyShips.Remove(ship);
    }

    public void removeAllyShip(Ship ship)
    {
        ship.stopRegenarateAnim();
        allyShips.Remove(ship);
    }

    public void swapTeams()
    {
        List<Ship> temp = new List<Ship>(allyShips);
        allyShips = enemyShips;
        enemyShips = temp;
        statusBar.setMaxHealth(maxHealth);
        health = maxHealth;
        setTeam(dockTeam == GameController.Team.black ? GameController.Team.white : GameController.Team.black);
    }

    public GameController.Team getTeam()
    {
        return dockTeam;
    }

    public void setTeam(GameController.Team newTeam)
    {
        dockTeam = newTeam;
        statusBar.setColor(newTeam == GameController.Team.black ? Color.black : Color.white);
        indicatorB.SetActive(newTeam == GameController.Team.black ? true : false);
        indicatorW.SetActive(newTeam == GameController.Team.white ? true : false);
    }

    public void getDamage(int damage)
    {
        health -= damage;
        statusBar.changeHealth(health);

        if (health == 0)
        {
            swapTeams();
        }
    }

    public void onShipDestroyed(Ship ship)
    {
        if (allyShips.Contains(ship)) { removeAllyShip(ship); }
        else if (enemyShips.Contains(ship)) { removeEnemyShip(ship); }
    }


}
