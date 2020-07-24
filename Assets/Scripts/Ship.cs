using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ship : MonoBehaviour
{
    int health = 15;
    HealthBar healthBar;
    Canvas healthCanvas;
    string team;
    NavMeshAgent agent;
    public GameObject selector;

    void Awake()
    {
        healthCanvas = GetComponentInChildren<Canvas>();
        healthBar = healthCanvas.gameObject.GetComponentInChildren<HealthBar>();
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        team = tag;
        healthBar.setMaxHealth(health);

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

    public string getTeam()
    {
        return team;
    }

    public void getDamage(int damage)
    {
        health = health - damage;
        healthBar.changeHealth(health);
        if (health == 0)
        {
            getDestroyed();
        }
    }

    void getDestroyed()
    {
        Debug.Log("I was killed");
    }

}
