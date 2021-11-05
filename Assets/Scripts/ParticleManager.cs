using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject particle;
    public void Emmit(Transform transform)
    {
        GameObject newpar = Instantiate(particle, transform.position, Quaternion.identity);
        newpar.SetActive(true);
        newpar.GetComponent<ParticleSystem>().Play();
        //Destroy(newpar, newpar.GetComponent<ParticleSystem>().main.duration);
        Destroy(newpar, 1f);
    }
}
