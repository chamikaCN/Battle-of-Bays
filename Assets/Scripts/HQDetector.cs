using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HQ dock = GetComponentInParent<HQ>();
        if (other.tag == "ship")
        {
            Ship othership = other.GetComponent<Ship>();
            if (othership.getTeam() == dock.getTeam())
            {
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
        HQ dock = GetComponentInParent<HQ>();
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
