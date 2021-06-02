using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockButton : MonoBehaviour
{
    public int id;
    Vector3 placement;

    public void clicked()
    {
        HUDManager.instance.SelectHQButton(id - 1);
    }

    public void setPlacement(Vector3 place)
    {
        placement = place;
    }

    public Vector3 getPlacement()
    {
        return placement;
    }
}
