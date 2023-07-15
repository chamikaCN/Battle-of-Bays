using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : SupplyBase
{
    int maxHealth = 10;
    Cannon cannon;
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

    public void swapTeams()
    {
        StopCoroutine(shipGenerator);
        List<Ship> temp = new List<Ship>(allyShips);
        allyShips = enemyShips;
        enemyShips = temp;
        statusBar.setMaxHealth(maxHealth);
        health = maxHealth;
        setTeam(team == GameController.Team.black ? GameController.Team.white : GameController.Team.black);
    }

    public void setTeam(GameController.Team newTeam)
    {
        team = newTeam;
        statusBar.setColor(newTeam == GameController.Team.black ? Color.black : Color.white);
        indicatorB.SetActive(newTeam == GameController.Team.black ? true : false);
        indicatorW.SetActive(newTeam == GameController.Team.white ? true : false);
        shipGenerator =  StartCoroutine(shipGenerate(300));
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

}
