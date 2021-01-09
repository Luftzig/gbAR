using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragWithMouse : MonoBehaviour
{

    private Vector3 offset;
    private float zCoord;

    void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMousePos();
    }

    private Vector3 GetMousePos()
    {

        // Pixel coordinates of mouse (x,y)
        Vector3 mousePos = Input.mousePosition;

        // z coordinate of game object on screen
        mousePos.z = zCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePos);

    }

    void OnMouseDrag()
    {
        transform.position = GetMousePos() + offset;
    }
}
