using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEmitter : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float timeSpan = 1f;
    public int numOfBubbles = 3;
    public static GameObject[] fruitsList;

    // Emit bubbles randomly in the scene
    void Start()
    {
        fruitsList = Resources.LoadAll<GameObject>("Prefabs/FruitPrefabs");
        StartCoroutine(EmittBubble());
    }
    private IEnumerator EmittBubble()
    {
        for(int i=0; i<numOfBubbles;i++)
        {
            yield return new WaitForSeconds(timeSpan);

            GameObject bubbleInstance = (GameObject)Instantiate(bubblePrefab);
            GameObject fruitInstance = (GameObject)Instantiate(fruitsList[i]);
            fruitInstance.name = fruitsList[i].name;
            fruitInstance.transform.parent = bubbleInstance.transform;
            bubbleInstance.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(0f, 3f), 0);
        }
    }
}
