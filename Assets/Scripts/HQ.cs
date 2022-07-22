using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : MonoBehaviour
{
    List<Ship> enemyShips, allyShips;
    public GameController.Team HQTeam;
    Cannon[] cannons = new Cannon[2];
    int maxHealth = 20, health;
    StatusBar statusBar;
    Canvas statusCanvas;

    void Awake()
    {
        statusCanvas = GetComponentInChildren<Canvas>();
        statusBar = statusCanvas.gameObject.GetComponentInChildren<StatusBar>();
        enemyShips = new List<Ship>();
        allyShips = new List<Ship>();
    }
    void Start() {
        cannons[0] = transform.GetChild(2).GetComponent<Cannon>();
        cannons[1] = transform.GetChild(3).GetComponent<Cannon>();
        statusBar.setMaxHealth(maxHealth);
        statusBar.setColor(HQTeam == GameController.Team.black ? Color.black : Color.white);
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

    public GameController.Team getTeam()
    {
        return HQTeam;
    }


    public void getDamage(int damage)
    {
        health -= damage;
        statusBar.changeHealth(health);

        if(health == 0){
            Debug.Log("We Lost "+ HQTeam);
            GlobalEventManager.invokeGameFinish();
        }
    }

    public List<Ship> getAllyShips() { return allyShips; }

    public List<Ship> getEnemyShips() { return enemyShips; }


}
