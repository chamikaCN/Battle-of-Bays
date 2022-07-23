using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SupplyBase : MonoBehaviour
{
    protected List<Ship> enemyShips, allyShips;
    public GameController.Team team;
    protected int health;
    protected StatusBar statusBar;
    protected Canvas statusCanvas;


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
        return team;
    }

    public void onShipDestroyed(Ship ship)
    {
        if (allyShips.Contains(ship)) { removeAllyShip(ship); }
        else if (enemyShips.Contains(ship)) { removeEnemyShip(ship); }
    }
}
