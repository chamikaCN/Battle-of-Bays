using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public float lifeTime;
    void Start()
    {
        StartCoroutine(endExplosion());
    }

    IEnumerator endExplosion()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }

}
