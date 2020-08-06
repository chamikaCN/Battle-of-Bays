using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosion, splash;
    GameController.Team shipTeam;
    public Cannon.CannonType cannonType;
    Rigidbody cb;
    Vector3 direction;
    public float shotSpeed;
    void Start()
    {
        cb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        cb.GetComponent<Rigidbody>().velocity = direction * shotSpeed * Time.deltaTime;
        cb.GetComponent<Rigidbody>().useGravity = true;
    }

    public void setTarget(Vector3 dir, GameController.Team team)
    {
        direction = (dir - transform.position).normalized;
        shipTeam = team;
    }

    public GameController.Team getTeam()
    {
        return shipTeam;
    }

    public void DestroyBall(string type)
    {
        if (type == "water")
        {
            Instantiate(splash, transform.position, transform.rotation);
        }
        else
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }
        Destroy(this.gameObject);
    }

    public Vector3 getDirection()
    {
        return cb.velocity * -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ship")
        {
            Ship otherShip = other.GetComponent<Ship>();
            if (otherShip.getTeam() != getTeam())
            {
                //explode
                otherShip.getDamage(1, getDirection());
                DestroyBall("other");
            }
            // else if (otherShip != GetComponent<CannonBall>().getParentShip())
            // {
            //     GetComponent<CannonBall>().DestroyBall();
            // }
        }
        else if (other.tag == "dock" && cannonType == Cannon.CannonType.ship)
        {
            Dock otherDock = other.GetComponent<Dock>();
            if (otherDock.getTeam() != getTeam())
            {
                //explode
                otherDock.getDamage(1);
                DestroyBall("other");
            }
        }
        else if (other.tag == "hq" && cannonType == Cannon.CannonType.ship)
        {
            HQ otherDock = other.GetComponent<HQ>();
            if (otherDock.getTeam() != getTeam())
            {
                //explode
                otherDock.getDamage(1);
                DestroyBall("other");
            }
        }
        else if (other.tag == "water")
        {
            //play splash sound
            DestroyBall("water");
        }
        else if (other.tag == "ground")
        {
            //explode
            DestroyBall("other");
        }
    }

}
