using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        cam = GameController.instance.getCamera().transform;
    }
    void Update()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
