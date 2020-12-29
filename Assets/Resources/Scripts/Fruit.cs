using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public string fruitType;

    void Start()
    {
        fruitType = this.gameObject.name;
    }

    void Update()
    {
        this.transform.Rotate(Vector3.up, 20 * Time.deltaTime);
    }
}
