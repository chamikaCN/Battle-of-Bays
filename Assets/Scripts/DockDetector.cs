using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Dock dock = GetComponentInParent<Dock>();
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
        Dock dock = GetComponentInParent<Dock>();
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
}
