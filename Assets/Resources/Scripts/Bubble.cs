using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public GameObject bubbleBurstPrefab;
    float motionAngle = 0.0f;
    private Vector3 wobbleVelocity;
    private float motionSpeed;
    private float motionWidth;
    private Vector3 GetBubbleMotion(float motionSpeed, float motionWidth, float motionGravity)
    {
        motionAngle += Mathf.PI / 180.0f * motionSpeed;
        return new Vector3(Mathf.Sin(motionAngle) * motionWidth, motionGravity, 0.0f);
    }
    private void Start()
    {
        // Randomize the motion of bubbles
        motionSpeed = Random.Range(0.1f, 0.5f);
        motionWidth = Random.Range(-0.5f, 0.5f);
    }

    private void Update()
    {
        wobbleVelocity = GetBubbleMotion(motionSpeed, motionWidth, 0.05f);
        Vector3 bubblePos = new Vector3(transform.position.x, transform.position.y, 5);
        transform.position = bubblePos + wobbleVelocity * Time.deltaTime;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "HandOnSpace")
        {
            if (bubbleBurstPrefab)
            {
                GameObject explosion = (GameObject)Instantiate(bubbleBurstPrefab, other.transform.position, bubbleBurstPrefab.transform.rotation);
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
