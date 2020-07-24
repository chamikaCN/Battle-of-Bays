using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    Ship parentShip;
    string shipTeam;
    Rigidbody cb;
    Vector3 direction;
    [Range(0, 5)]
    public float shotSpeed;
    void Start()
    {
        cb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        cb.GetComponent<Rigidbody>().velocity = direction * shotSpeed;
        cb.GetComponent<Rigidbody>().useGravity = true;
    }

    public void setTarget(Vector3 dir, Ship ship)
    {
        direction = dir - transform.position + new Vector3(0, 1, 0);
        parentShip = ship;
        shipTeam = parentShip.getTeam();
    }

    public string getTeam()
    {
        return shipTeam;
    }

    public Ship getParentShip()
    {
        return parentShip;
    }

    public void DestroyBall()
    {
        Destroy(this.gameObject);
    }


}
