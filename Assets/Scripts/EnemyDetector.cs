using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    GameObject target;
    
    private void Update()
    {
        float MaxAngleDeflection = 60.0f;
        float SpeedOfPendulum = 1.0f;

        float angle = MaxAngleDeflection * Mathf.Sin(Time.time * SpeedOfPendulum);
        transform.localRotation = Quaternion.Euler(0, angle, 0);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50, GameController.instance.shipsLayerMask))
        {
            if (hit.collider.GetComponent<Ship>().getTeam() != transform.parent.GetComponent<Cannon>().getTeam())
            {
                if (hit.collider.gameObject != target)
                {
                    target = hit.collider.gameObject;
                }
                if (target != null)
                {
                    transform.parent.GetComponent<Cannon>().attackToPoint(target.transform.position + new Vector3(0, 2, 0));
                }
            }
        }else if (Physics.Raycast(ray, out hit, 50, GameController.instance.docksLayerMask))
        {
            if (hit.collider.tag == "dock")
            {
                GameController.Team dockTeam = hit.collider.GetComponentInParent<Dock>().getTeam();
                if (dockTeam != transform.parent.GetComponent<Cannon>().getTeam() && dockTeam != GameController.Team.neutral)
                {
                    if (hit.collider.gameObject != target)
                    {
                        target = hit.collider.gameObject;
                    }
                    if (target != null)
                    {
                        transform.parent.GetComponent<Cannon>().attackToPoint(target.transform.position + new Vector3(0, 3, 0));
                    }
                }
            }else if (hit.collider.tag == "hq")
            {
                GameController.Team hqTeam = hit.collider.GetComponentInParent<HQ>().getTeam();
                if (hqTeam != transform.parent.GetComponent<Cannon>().getTeam())
                {
                    if (hit.collider.gameObject != target)
                    {
                        target = hit.collider.gameObject;
                    }
                    if (target != null)
                    {
                        transform.parent.GetComponent<Cannon>().attackToPoint(target.transform.position + new Vector3(0, 3, 0));
                    }
                }
            }
        }

    }

    public void undetect(){
        target = null;
    }
}
