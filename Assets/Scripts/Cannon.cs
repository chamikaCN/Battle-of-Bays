using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public enum CannonType { ship, dock}
    public CannonType cannonType;
    GameController.Team team;
    bool isAttacked;
    public GameObject cannonBall;
    void Start()
    {
        isAttacked = false;
        if (cannonType == CannonType.ship)
        {
            team = transform.parent.parent.GetComponent<Ship>().getTeam();
        }else{
            team = GetComponentInParent<Dock>().getTeam();
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
        yield return new WaitForSeconds(1);
        isAttacked = false;
    }

    public GameController.Team getTeam()
    {
        return team;
    }
}
