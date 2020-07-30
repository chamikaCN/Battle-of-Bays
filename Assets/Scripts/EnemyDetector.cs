using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    Ship target;
    private void Update()
    {
        //transform.Rotate(new Vector3(0, 2, 0) * Time.deltaTime * 50f, Space.World);

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
                if (hit.collider.GetComponent<Ship>() != target)
                {
                    target = hit.collider.GetComponent<Ship>();
                }
                if (target != null)
                {
                    transform.parent.GetComponent<Cannon>().attackToPoint(target.transform.position + new Vector3(0, 2, 0));
                }
            }
        }

    }

    public void undetect(){
        target = null;
    }
}
