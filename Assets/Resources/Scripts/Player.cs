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

                foreach (GameObject fruit in BubbleEmitter.requiredfruits)
                {
                    if(fruit.name== other.transform.GetChild(0).name)
                    {
                        print("Pop the correct fruit!");
                        Transform fruitUI = BubbleEmitter.UIFruitList.transform.Find(fruit.name);
                        fruitUI.GetChild(2).gameObject.SetActive(true);
                    }
                }
                Destroy(other.gameObject);
            }

        }
    }
}
