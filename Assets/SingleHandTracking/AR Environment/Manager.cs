using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Manager : MonoBehaviour {
    #region Singleton
    public static Manager instance;

    private void Awake() {
        instance = this;
    }
    #endregion

    public GameObject HandOnSpace;
    public RaycastOnPlane RaycastOnPlane;

    public Vector3[] GetHandLandmarks(){
        return GetComponent<ARHandProcessor>().CurrentHand.GetLandmarks();
    }

    public Vector3 GetHandLandmark(int landmark_index){
        return GetComponent<ARHandProcessor>().CurrentHand.GetLandmark(landmark_index);
    }
}