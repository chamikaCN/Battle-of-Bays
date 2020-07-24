using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    string team;
    bool isAttacked;
    public GameObject cannonBall;
    void Start()
    {
        isAttacked = false;
        team = transform.parent.parent.tag;
    }

    public void attackToPoint(Vector3 point)
    {
        if (!isAttacked)
        {
            Debug.Log("Attacked by " + name);
            transform.GetChild(0).LookAt(point, Vector3.up);
            GameObject cb = Instantiate(cannonBall, transform.GetChild(0).position, Quaternion.identity);
            cb.GetComponent<CannonBall>().setTarget(point,transform.parent.parent.GetComponent<Ship>());
            isAttacked = true;
            StartCoroutine(attackReset());
        }
    }

    IEnumerator attackReset()
    {
        yield return new WaitForSeconds(1);
        isAttacked = false;
    }
}
