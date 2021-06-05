using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ship : MonoBehaviour
{
    public GameObject explosion, selector,wreck;
    GameObject Regenarate, Fire;
    public float sp;
    int health, maxHealth = 8;
    HealthBar healthBar;
    Canvas healthCanvas;
    public GameController.Team shipTeam;
    NavMeshAgent agent;
    Vector3 direction;
    Cannon[] cannons;
    bool playerControlled, regenarating;

    void Awake()
    {
        healthCanvas = GetComponentInChildren<Canvas>();
        healthBar = healthCanvas.gameObject.GetComponentInChildren<HealthBar>();
        cannons = GetComponentsInChildren<Cannon>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Fire = transform.GetChild(3).gameObject;
        Regenarate = transform.GetChild(4).gameObject;
        healthBar.setMaxHealth(maxHealth);
        playerControlled = false;
        health = maxHealth;
        regenarating = false;
    }

    void LateUpdate()
    {
        if (!playerControlled && direction != null)
        {
            //       transform.LookAt(new Vector3(direction.x, 0, direction.z), Vector3.up);
            //         Vector3 lookAtGoal = new Vector3(direction.x, transform.position.y, direction.z);
            //         Vector3 dir = lookAtGoal - transform.position;
            //         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5);
        }
    }

    public void moveToPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }

    public void activateSelector()
    {
        selector.SetActive(true);
    }

    public void deactivateSelector()
    {
        selector.SetActive(false);
    }

    public void activatePlayerControl()
    {
        playerControlled = true;
    }

    public void deactivatePlayerControl()
    {
        playerControlled = false;
    }

    public GameController.Team getTeam()
    {
        return shipTeam;
    }

    public void getDamage(int damage, Vector3 direction)
    {
        health = health - damage;

        //Vector3 lTargetDir = -(direction - transform.position);
        //lTargetDir.y = 0.0f;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.deltaTime * 100);

        this.direction = direction;
        if (health == 4)
        {
            //Instantiate(flames, transform.position + new Vector3(0, 3f, 0), transform.rotation, transform);
            Fire.SetActive(true);
        }
        healthBar.changeHealth(health);
        if (health == 0)
        {
            GameController.instance.DestroyCheck(this);
        }
    }

    public void getDestroyed()
    {

        Instantiate(explosion, transform.position + new Vector3(0, 1, 0), transform.rotation);
        GameObject gw = Instantiate(wreck, transform.position - new Vector3(0, 5, 0), transform.rotation) ;
        gw.transform.localScale = gw.transform.localScale * 0.3f;
        Destroy(this.gameObject);
    }

    public void RegenarateHealth()
    {
        if (!regenarating)
        {
            if (health < maxHealth)
            {
                Regenarate.SetActive(true);
                Fire.SetActive(false);
                regenarating = true;
                StartCoroutine(regenarateReset());
            }
            else
            {
                stopRegenarateAnim();
            }
        }
    }

    public void stopRegenarateAnim()
    {
        Regenarate.SetActive(false);
    }

    public void removeEnemy(Ship ship)
    {
        foreach (Cannon can in cannons)
        {
            GetComponentInChildren<EnemyDetector>().undetect();
        }
    }

    IEnumerator regenarateReset()
    {
        yield return new WaitForSeconds(5);
        health += 1;
        healthBar.changeHealth(health);
        regenarating = false;
    }
}

