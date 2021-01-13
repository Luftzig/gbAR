using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public GameObject bubbleBurstPrefab;

    void Awake()
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");

        if (player.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Player: Trigger event {other.gameObject.name}");
        if (other.name == "Bubble(Clone)")
        {
            if (bubbleBurstPrefab)
            {
                GetComponent<AudioSource>().Play();
                GameObject explosion = Instantiate(bubbleBurstPrefab, other.transform.position, bubbleBurstPrefab.transform.rotation);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
                for(int i=0; i< BubbleEmitter.requiredfruits.Count;i++)
                {
                    if(BubbleEmitter.requiredfruits[i].name== other.transform.GetChild(0).name)
                    {
                        Transform fruitUI = BubbleEmitter.UIFruitList.transform.Find(BubbleEmitter.requiredfruits[i].name);
                        fruitUI.GetChild(2).gameObject.SetActive(true);
                        BubbleEmitter.requiredfruits.RemoveAt(i);
                    }
                }
                Destroy(other.gameObject);
            }

        }
    }
}
