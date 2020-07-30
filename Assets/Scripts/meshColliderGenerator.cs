using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshColliderGenerator : MonoBehaviour
{
    #region Singleton
    public static meshColliderGenerator instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("many cameracontollers");
            return;
        }

        instance = this;
    }
    #endregion
    MeshCollider mc ;
    void Start()
    {
        mc = GetComponent<MeshCollider>();
    }

    public void setMesh(Mesh m){
        mc.sharedMesh = m;
    }
}
