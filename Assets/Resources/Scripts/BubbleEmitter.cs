using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleEmitter : MonoBehaviour
{
    public GameObject bubblePrefab;
    public GameObject fruitUIPrefab;
    public int numbOfFruitRequired = 3;
    public int numOfBubbles = 3;
    public static List<GameObject> fruitsList;
    public static List<GameObject> generatedfruits;
    public static List<GameObject> requiredfruits;
    public static GameObject UIFruitList;
    public float maxInitBubbleHeight = 1f;
    public float maxInitBubbleRange = 2f;

    // Emit bubbles randomly in the scene
    void Start()
    {
        fruitsList = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/FruitPrefabs"));
        generatedfruits = GetRandomItemsFromList<GameObject>(fruitsList, numOfBubbles);
        requiredfruits = GetRandomItemsFromList<GameObject>(generatedfruits, numbOfFruitRequired);
        EmitBubble();
        UIFruitList = GameObject.Find("CheckList");
        foreach (GameObject fruit in requiredfruits)
        {
            GameObject UIintsatnce = (GameObject) Instantiate(fruitUIPrefab);
            UIintsatnce.name = fruit.name;
            UIintsatnce.transform.SetParent(UIFruitList.transform);
            UIintsatnce.GetComponentInChildren<Text>().text = fruit.name;
        }
    }

    private void EmitBubble()
    {
        for (int i = 0; i < numOfBubbles; i++)
        {
            GameObject bubbleInstance = (GameObject) Instantiate(bubblePrefab);
            GameObject fruitInstance = (GameObject) Instantiate(generatedfruits[i]);
            fruitInstance.name = generatedfruits[i].name;
            fruitInstance.transform.parent = bubbleInstance.transform;
            bubbleInstance.transform.position = new Vector3(Random.Range(-maxInitBubbleRange, maxInitBubbleRange),
                Random.Range(0f, maxInitBubbleHeight), 0);
        }
    }

    public static List<T> GetRandomItemsFromList<T>(List<T> list, int number)
    {
        // For remove picked items from
        List<T> tmpList = new List<T>(list);
        // For move items to
        List<T> newList = new List<T>();

        // make sure tmpList isn't already empty
        while (newList.Count < number && tmpList.Count > 0)
        {
            int index = Random.Range(0, tmpList.Count);
            newList.Add(tmpList[index]);
            tmpList.RemoveAt(index);
        }

        return newList;
    }

    public static void RemoveItemFromRequirements(GameObject obj)
    {
        requiredfruits.Remove(obj);
    }
}