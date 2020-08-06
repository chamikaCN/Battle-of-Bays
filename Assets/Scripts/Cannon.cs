using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public enum CannonType { ship, dock,hq}
    public CannonType cannonType;
    GameController.Team team;
    bool isAttacked;
    int resetTime;
    public GameObject cannonBall;
    void Start()
    {
        isAttacked = false;
        if (cannonType == CannonType.ship)
        {
            resetTime = 1;
            team = transform.parent.parent.GetComponent<Ship>().getTeam();
        }else if (cannonType == CannonType.dock){
            resetTime = 3;
            team = GetComponentInParent<Dock>().getTeam();
        }else{
            resetTime = 5;
            team = GetComponentInParent<HQ>().getTeam();
        }
    }

    public void attackToPoint(Vector3 point)
    {
        if (!isAttacked)
        {
            transform.GetChild(0).LookAt(point, Vector3.up);
            GameObject cb = Instantiate(cannonBall, transform.GetChild(0).position, Quaternion.identity);
            cb.GetComponent<CannonBall>().setTarget(point, team);
            isAttacked = true;
            StartCoroutine(attackReset());
        }
    }

    IEnumerator attackReset()
    {
        yield return new WaitForSeconds(resetTime);
        isAttacked = false;
    }

    public GameController.Team getTeam()
    {
        return team;
    }
}
