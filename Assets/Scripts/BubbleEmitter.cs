using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEmitter : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float timeSpan = 1f;
    public int numOfBubbles = 5;
    // Emit bubbles randomly in the scene
    void Start()
    {
        StartCoroutine(EmittBubble());
    }
    private IEnumerator EmittBubble()
    {
        for(int i=0; i<numOfBubbles;i++)
        {
            yield return new WaitForSeconds(timeSpan);

            GameObject bubbleInstance = (GameObject)Instantiate(bubblePrefab);
            bubbleInstance.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(0f, 3f), 0);
        }
    }
}
