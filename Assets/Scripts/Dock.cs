using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : MonoBehaviour
{
    List<Ship> enemyShips, allyShips;
    public GameController.Team dockTeam;
    Cannon cannon;

    void Awake()
    {
        enemyShips = new List<Ship>();
        allyShips = new List<Ship>();
    }
    void Start() {
        cannon = transform.GetChild(1).GetComponent<Cannon>();
    }

    void Update()
    {
        foreach (Ship ally in allyShips)
        {
            ally.RegenarateHealth();
        }
        if(enemyShips.Count>0){
            cannon.attackToPoint(enemyShips[0].transform.position);
        }

    }

    public void addEnemyShip(Ship ship)
    {
        enemyShips.Add(ship);
        if (enemyShips.Count > allyShips.Count+1)
        {
            Debug.Log("swapped teams");
            swapTeams();
        }
    }

    public void addAllyShip(Ship ship)
    {
        allyShips.Add(ship);
        Debug.Log("new ally added " + ship.name);
    }

    public void removeEnemyShip(Ship ship)
    {
        enemyShips.Remove(ship);
    }

    public void removeAllyShip(Ship ship)
    {
        allyShips.Remove(ship);
    }

    public void swapTeams()
    {
        List<Ship> temp = new List<Ship>(allyShips);
        allyShips = enemyShips;
        enemyShips = temp;
        if (dockTeam == GameController.Team.black)
        {
            setTeam(GameController.Team.white);
        }
        else
        {
            setTeam(GameController.Team.black);
        }
    }

    public GameController.Team getTeam()
    {
        return dockTeam;
    }

    public void setTeam(GameController.Team newTeam)
    {
        dockTeam = newTeam;
        Debug.Log("new team set "+ newTeam);
    }

    private void OnTriggerEnter(Collider other)
    {
        Dock dock = GetComponent<Dock>();
        if (other.tag == "ship")
        {
            Ship othership = other.GetComponent<Ship>();
            if (othership.getTeam() == dock.getTeam())
            {
                dock.addAllyShip(othership);
            }
            else if (dock.getTeam() == GameController.Team.neutral)
            {
                dock.setTeam(othership.getTeam());
                dock.addAllyShip(othership);
            }
            else
            {
                dock.addEnemyShip(othership);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Dock dock = GetComponent<Dock>();
        if (other.tag == "ship")
        {
            Ship othership = other.GetComponent<Ship>();
            if (othership.getTeam() == dock.getTeam())
            {
                dock.removeAllyShip(othership);
            }
            else
            {
                dock.removeEnemyShip(othership);
            }
        }
    }

    public List<Ship> getAllyShips() { return allyShips; }

    public List<Ship> getEnemyShips() { return enemyShips; }


}
