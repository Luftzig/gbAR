using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bubbleBurstPrefab;
    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Bubble(Clone)")
        {
            if (bubbleBurstPrefab)
            {
                GameObject explosion = (GameObject)Instantiate(bubbleBurstPrefab, other.transform.position, bubbleBurstPrefab.transform.rotation);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
                Debug.Log("Pop: "+other.transform.GetChild(0).name);
                Destroy(other.gameObject);
            }

        }
    }
}
