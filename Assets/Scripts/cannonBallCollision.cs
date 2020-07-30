using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannonBallCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ship")
        {
            Ship otherShip = other.GetComponent<Ship>();
            if (otherShip.getTeam() != GetComponent<CannonBall>().getTeam())
            {
                //explode
                otherShip.getDamage(1,GetComponent<CannonBall>().getDirection());
                GetComponent<CannonBall>().DestroyBall("other");
            }
            // else if (otherShip != GetComponent<CannonBall>().getParentShip())
            // {
            //     GetComponent<CannonBall>().DestroyBall();
            // }
            else{Debug.Log(other.name);}
        }
        else if (other.tag == "water")
        {
            //play splash sound
            GetComponent<CannonBall>().DestroyBall("water");
            Debug.Log(other.name);
        }
        else if ((other.tag == "ground"))
        {
            //explode
            GetComponent<CannonBall>().DestroyBall("other");
            Debug.Log(other.name);
        }
        else{Debug.Log(other.name);}
    }
}
