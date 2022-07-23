using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : SupplyBase
{
    int maxHealth = 20;
    Cannon[] cannons = new Cannon[2];

    void Awake()
    {
        statusCanvas = GetComponentInChildren<Canvas>();
        statusBar = statusCanvas.gameObject.GetComponentInChildren<StatusBar>();
        enemyShips = new List<Ship>();
        allyShips = new List<Ship>();
    }
    void Start() {
        GlobalEventManager.shipDestroyed += onShipDestroyed;
        cannons[0] = transform.GetChild(2).GetComponent<Cannon>();
        cannons[1] = transform.GetChild(3).GetComponent<Cannon>();
        statusBar.setMaxHealth(maxHealth);
        statusBar.setColor(team == GameController.Team.black ? Color.black : Color.white);
        maxHealth = 20;
        health = maxHealth;
    }

    void Update()
    {
        foreach (Ship ally in allyShips)
        {
            ally.RegenarateHealth();
        }
        if(enemyShips.Count>1){
            cannons[0].attackToPoint(enemyShips[0].transform.position);
            cannons[1].attackToPoint(enemyShips[1].transform.position);
        }else if(enemyShips.Count>0){
            cannons[0].attackToPoint(enemyShips[0].transform.position);
            cannons[1].attackToPoint(enemyShips[0].transform.position);
        }

    }

    public void getDamage(int damage)
    {
        health -= damage;
        statusBar.changeHealth(health);

        if(health == 0){
            Debug.Log("We Lost "+ HQTeam);
            GlobalEventManager.invokeGameFinished();
        }
    }

}
