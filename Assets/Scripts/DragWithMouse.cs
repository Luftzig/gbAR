using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragWithMouse : MonoBehaviour
{

    private Vector3 mOffset;
    private float mZCoord;

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMousePos();
    }



    private Vector3 GetMousePos()
    {

        // Pixel coordinates of mouse (x,y)
        Vector3 mousePos = Input.mousePosition;

        // z coordinate of game object on screen
        mousePos.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePos);

    }



    void OnMouseDrag()
    {
        transform.position = GetMousePos() + mOffset;
    }
}
