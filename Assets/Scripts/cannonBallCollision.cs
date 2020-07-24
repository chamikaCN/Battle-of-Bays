using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannonBallCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "whiteShip" || other.tag == "blackShip")
        {
            Ship otherShip = other.GetComponent<Ship>();
            if (otherShip.getTeam() != GetComponent<CannonBall>().getTeam())
            {
                //explode
                otherShip.getDamage(1);
                GetComponent<CannonBall>().DestroyBall();
            }
            else if (otherShip != GetComponent<CannonBall>().getParentShip())
            {
                GetComponent<CannonBall>().DestroyBall();
            }
        }
        else if (other.tag == "water")
        {
            //play splash sound
            GetComponent<CannonBall>().DestroyBall();
        }
        else if ((other.tag == "ground"))
        {
            //explode
            GetComponent<CannonBall>().DestroyBall();
        }
    }
}
