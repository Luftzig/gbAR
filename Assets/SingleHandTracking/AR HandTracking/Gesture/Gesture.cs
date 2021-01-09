using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Gesture : MonoBehaviour {

  #region Singleton
    public static Gesture instance;

    private void Awake() {
        instance = this;
    }
    #endregion

    private ARHandProcessor handProcessor;
    private Queue<ARHand> handQueue;
    private Camera cam;

    public Text text;

    private void Start() {
        cam = Camera.main;
        handQueue = new Queue<ARHand>();
        handProcessor = Manager.instance.gameObject.GetComponent<ARHandProcessor>();
        StartCoroutine(UpdateHand());
        StartCoroutine(checkTapShoot());
    }

    IEnumerator UpdateHand() {
        while (true) {
            yield return new WaitForSeconds(0.05f);
            handQueue.Enqueue(handProcessor.CurrentHand.Clone());
            if (handQueue.Count > 5) handQueue.Dequeue();
        }
    }
    //test
    public IEnumerator checkTapShoot() {
        while (true) {
            yield return new WaitForSeconds(0.05f);
            if (isAgree()) {
                text.text = "AGREE";
            } else if (checkTap()) {
                text.text = "TAP";
            } else if (checkShoot()) {
                text.text = "SHOOT";
            } else if (isSpiderMan()) {
                text.text = "SPIDER MAN";
            } else {
                text.text = "NONE";
            }
        }
    }

    #region CheckTap
    public bool checkTap() {
        if (handQueue.Count < 5) return false;
        ARHand[] hands = handQueue.ToArray();
        if (angleFingerStraight(hands[0], 5)) {
            if (angleIndexWithHand(hands[0]) < angleIndexWithHand(hands[2]) &&
            angleIndexWithHand(hands[2]) > angleIndexWithHand(hands[4]) &&
            isHigherByLandmark(hands[0], hands[2], 8, 0) &&
            isHigherByLandmark(hands[4], hands[2], 8, 0) &&
            angleIndexWithHand(hands[2]) > 15) {
                return true;
            }
        }
        return false;
    }

    public float angleIndexWithHand(ARHand hand) {
        return Vector3.Angle(hand.GetLandmark(5) - hand.GetLandmark(0), hand.GetLandmark(8) - hand.GetLandmark(5));
    }

    #endregion

    #region Check Shoot
    public bool checkShoot() {
        if (handQueue.Count < 5) return false;
        ARHand[] hands = handQueue.ToArray();
        if (angleFingerStraight(hands[0], 5) && angleFingerStraight(hands[0], 9) && angleFingerStraight(hands[0], 13) &&
        // angleFingerStraight(hands[2], 5) && angleFingerStraight(hands[2], 9) && angleFingerStraight(hands[2], 13) &&
        angleFingerStraight(hands[4], 5) && angleFingerStraight(hands[4], 9) && angleFingerStraight(hands[4], 13)) {
            if (isHigherByLandmark(hands[2], hands[0], 0, 0) && isHigherByLandmark(hands[2], hands[0], 8, Screen.height / 40) &&
            isHigherByLandmark(hands[2], hands[4], 0, 0) && isHigherByLandmark(hands[2], hands[4], 8, Screen.height / 40)) {
                return true;
            }
        }
        return false;
    }

    // return true nếu 1 cao hơn 2
    public bool isHigherByLandmark(ARHand state1, ARHand state2, int landmark, int distance) {
        return cam.WorldToScreenPoint(state1.GetLandmark(landmark)).y - cam.WorldToScreenPoint(state2.GetLandmark(landmark)).y > distance;
    }

    #endregion

    public bool isAgree() {
        if (handQueue.Count < 5) return false;
        ARHand[] hands = handQueue.ToArray();
        if (angleIndexWithHand(hands[4]) > 20 && angleFinger(hands[4], 5) > 20 && angleBig(hands[4]) > 20 &&
        isHigerThan(hands[4], 8, 4, 0) && isHigerThan(hands[4], 12, 8, Vector3.Distance(cam.WorldToScreenPoint(hands[4].GetLandmark(8)), cam.WorldToScreenPoint(hands[4].GetLandmark(6))))) {
            return true;
        }
        return false;
    }

    public bool isSpiderMan() {
        if (handQueue.Count < 5) return false;
        ARHand[] hands = handQueue.ToArray();
        if (angleFingerStraight(hands[4], 1) && angleFingerStraight(hands[4], 5) && angleFingerStraight(hands[4], 17) &&
        isHigerThan(hands[4], 10, 12, 0) && isHigerThan(hands[4], 14, 16, 0)) {
            return true;
        }
        return false;
    }

    public bool isHigerThan(ARHand hand, int landmark1, int landmark2, float distance) {
        return cam.WorldToScreenPoint(hand.GetLandmark(landmark1)).y - cam.WorldToScreenPoint(hand.GetLandmark(landmark2)).y > distance;
    }

    public bool angleFingerStraight(ARHand hand, int fingerWirst) {
        return Vector3.Angle(hand.GetLandmark(fingerWirst + 1) - hand.GetLandmark(fingerWirst), hand.GetLandmark(fingerWirst + 2) - hand.GetLandmark(fingerWirst + 1)) < 20;
    }

    public float angleFinger(ARHand hand, int fingerWirst) {
        return Vector3.Angle(hand.GetLandmark(fingerWirst + 1) - hand.GetLandmark(fingerWirst), hand.GetLandmark(fingerWirst + 3) - hand.GetLandmark(fingerWirst + 1));
    }

    public float angleBig(ARHand hand) {
        return Vector3.Angle(hand.GetLandmark(3) - hand.GetLandmark(2), hand.GetLandmark(4) - hand.GetLandmark(3));
    }
}
