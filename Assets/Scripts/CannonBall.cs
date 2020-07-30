using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosion, splash;
    GameController.Team shipTeam;
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

    public void setTarget(Vector3 dir, GameController.Team team)
    {
        direction = dir - transform.position + new Vector3(0, 1, 0);
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

}
