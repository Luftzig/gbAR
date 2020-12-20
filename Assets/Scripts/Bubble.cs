using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public GameObject bubbleBurstPrefab;
    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Cube")
        {
            if (bubbleBurstPrefab)
            {
                GameObject explosion = (GameObject)Instantiate(bubbleBurstPrefab, transform.position, bubbleBurstPrefab.transform.rotation);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
                Debug.Log("Pop");
                Destroy(gameObject);
            }

        }
    }
}
