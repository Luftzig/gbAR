using MediapipeHandTracking;
using UnityEngine;

public class ARHandProcessor : MonoBehaviour {
    private GameObject Hand = default;
    private HandRect currentHandRect = default;
    private HandRect oldHandRect = default;
    private ARHand currentHand = default;
    private bool isHandRectChange = default;

    void Start() {
        Hand = Manager.instance.HandOnSpace;
        currentHand = new ARHand();
        currentHandRect = new HandRect();
        oldHandRect = new HandRect();
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate() {
        if (GetComponent<ARFrameProcessor>().HandProcessor == null) return;
        float[] handRectData = GetComponent<ARFrameProcessor>().HandProcessor.getHandRectData();
        float[] handLandmarksData = GetComponent<ARFrameProcessor>().HandProcessor.getHandLandmarksData();

        if (null != handRectData) {
            currentHandRect = HandRect.ParseFrom(handRectData);
            if (!isHandStay()) {
                oldHandRect = currentHandRect;
                isHandRectChange = true;
            } else {
                isHandRectChange = false;
            }
        }

        if (null != handLandmarksData && !float.IsNegativeInfinity(GetComponent<ARFrameProcessor>().ImageRatio)) {
            currentHand.ParseFrom(handLandmarksData, GetComponent<ARFrameProcessor>().ImageRatio);
        }

        Debug.Assert(Hand != null, $"Hand is null! {Hand}");
        if (!Hand.activeInHierarchy) return;
        for (int i = 0; i < Hand.transform.childCount-1; i++) {
            Hand.transform.GetChild(i).transform.position = currentHand.GetLandmark(i);
        }
        PlaceNeedle();
    }

    private void PlaceNeedle()
    {
        var needle = GameObject.Find("Needle");
        if (needle == null)
        {
            Debug.LogWarning("Failed to find the needle");
            return;
        }

        var wrist = currentHand.GetLandmark((int) ARHand.HandJoints.Wrist);
        var indexRoot = currentHand.GetLandmark((int) ARHand.HandJoints.IndexFingerMCP);
        var thumbRoot = currentHand.GetLandmark((int) ARHand.HandJoints.ThumbMCP);
        needle.transform.position = indexRoot + (indexRoot - wrist) / 2;
    }

    private bool isHandStay() {
        return currentHandRect.XCenter == oldHandRect.XCenter &&
            currentHandRect.YCenter == oldHandRect.YCenter &&
            currentHandRect.Width == oldHandRect.Width &&
            currentHandRect.Height == oldHandRect.Height &&
            currentHandRect.Rotaion == oldHandRect.Rotaion;
    }

    public ARHand CurrentHand { get => currentHand; }
    public bool IsHandRectChange { get => isHandRectChange; }
    public HandRect CurrentHandRect { get => currentHandRect; }
}