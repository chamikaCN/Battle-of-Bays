using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ship : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject selector;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void moveToPoint(Vector3 point){
        agent.SetDestination(point);
    }

    public void activateSelector(){
        selector.SetActive(true);
    }

    public void deactivateSelector(){
        selector.SetActive(false);
    }
    
}
