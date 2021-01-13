using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Object = System.Object;
using Plane = UnityEngine.Plane;
using Vector3 = UnityEngine.Vector3;

public class GameManager : MonoBehaviour
{
    private GameState gameState;
    public ARSessionOrigin arSessionOrigin;
    public ARPlaneManager arPlaneManager;
    public ARRaycastManager arRaycastManager;
    public Text textField;
    public Text debugText;
    public float areaThreshold = 1.5f * 1.5f;
    public GameObject bubbleEmitter;
    public GameObject hand;
    public ARHandProcessor handProcessor;
    public GameObject markerPrefab;
    public Plane playingPlane;

    void Start()
    {
        gameState = new GameState.LearnEnvironment(this);
        gameState.Start();
    }

    // Update is called once per frame
    void Update()
    {
        gameState.Update(this);
        var nextState = gameState.GetNext();
        if (nextState != gameState)
        {
            nextState.Start();
            gameState = nextState;
        }
    }
}

public abstract class GameState
{
    private GameState()
    {
    }

    public abstract void Update(GameManager target);

    public virtual void Start()
    {
    }

    public virtual GameState GetNext()
    {
        return this;
    }

    public sealed class LearnEnvironment : GameState
    {
        private ARPlaneManager arPlaneManager;
        private GameManager manager;
        private ARPlane gamePlane;

        public LearnEnvironment(GameManager manager)
        {
            this.manager = manager;
            arPlaneManager = manager.arPlaneManager;
        }

        public override void Start()
        {
            for (var i = 0; i < manager.hand.transform.childCount; i++)
            {
                manager.hand.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            }
        }

        public override void Update(GameManager target)
        {
            var mainPlane = IterateTrackables(arPlaneManager.trackables)
                .Where(plane => plane.alignment == PlaneAlignment.HorizontalUp)
                .OrderByDescending(CalcPlaneArea)
                .FirstOrDefault();
            if (mainPlane != null)
            {
                if (manager.debugText != null)
                {
                    manager.debugText.text = $"Play area is {CalcPlaneArea(mainPlane)}";
                }

                if (CalcPlaneArea(mainPlane) > manager.areaThreshold)
                {
                    gamePlane = mainPlane;
                }
            }
            else
            {
                target.textField.text = "Scan a plane to play";
            }
        }

        private float CalcPlaneArea(ARPlane plane)
        {
            return plane.size.x * plane.size.y;
        }

        public override GameState GetNext()
        {
            return gamePlane == null ? (GameState) this : new PlaceMarker(manager);
        }
    }

    public sealed class PlaceMarker : GameState
    {
        private readonly GameManager manager;
        private bool canStart = false;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private GameObject marker;

        public PlaceMarker(GameManager manager)
        {
            this.manager = manager;
        }

        public override void Start()
        {
            // manager.debugText.text = "Starting hand-tracking";
        }

        public override void Update(GameManager target)
        {
            if (null == marker)
            {
                manager.textField.text = "Select the point where the player will start";
                TryPlaceMarker(target);
            }
            else
            {
                manager.textField.text = "Click the marker again to start!";
                ConfirmMarker();
            }
        }

        private void ConfirmMarker()
        {
            if (Input.touchCount == 0)
            {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(Camera.current.ScreenPointToRay(Input.GetTouch(0).position), out hit))
            {
                if (hit.collider.gameObject == marker)
                {
                    CreatePlayingPlane(out manager.playingPlane);
                    canStart = true;
                    // We don't need to track planes anymore.
                    return;
                }

                if (manager.arRaycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.Planes))
                {
                    RepositionGameStart(manager.bubbleEmitter, hits.First());
                }
            }
        }

        private void TryPlaceMarker(GameManager target)
        {
            var bubbleEmitter = manager.bubbleEmitter;
            if (Input.touchCount == 0)
            {
                return;
            }

            if (manager.arRaycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.Planes))
            {
                target.textField.text = "Game plane set!";
                var arRaycastHit = hits.First(); // can this throw an exception?
                manager.debugText.text = $"Point selected: {arRaycastHit.pose.position}";
                RepositionGameStart(bubbleEmitter, arRaycastHit);
            }
        }

        private void CreatePlayingPlane(out Plane playingPlane)
        {
            var markerPosition = marker.transform.position;
            var markerUp = markerPosition + Vector3.up;
            var cameraToMarker = (Camera.current.transform.position - markerPosition).normalized;
            var planePoint = Vector3.Cross(cameraToMarker, Vector3.up);
            playingPlane = new Plane(Vector3.ProjectOnPlane(cameraToMarker, Vector3.up).normalized, markerPosition);
        }

        private void RepositionGameStart(GameObject bubbleEmitter, ARRaycastHit arRaycastHit)
        {
            bubbleEmitter.transform.position = arRaycastHit.pose.position;
            bubbleEmitter.transform.SetParent(manager.arSessionOrigin.transform);
            if (marker == null)
            {
                marker = GameObject.Instantiate(manager.markerPrefab, manager.arSessionOrigin.transform);
            }

            marker.transform.position = arRaycastHit.pose.position;
            marker.transform.rotation.SetLookRotation(Camera.current.transform.position - marker.transform.position);
        }

        public override GameState GetNext()
        {
            return canStart ? new PlayLevel(new LevelSettings(), manager) : (GameState) this;
        }
    }

    public sealed class PlayLevel : GameState
    {
        private readonly GameManager manager;
        private GameObject needle;
        public LevelSettings LevelSettings { get; }

        public PlayLevel(LevelSettings levelSettings, GameManager gameManager)
        {
            this.manager = gameManager;
            this.LevelSettings = levelSettings;
        }

        public override void Update(GameManager target)
        {
            manager.textField.text = "Play";
            if (needle != null)
            {
                var position = needle.transform.position;
                manager.debugText.text =
                    $"Needle at: {position}, {(Camera.current.transform.position - position).magnitude}\n" +
                    $"Emitter at: {manager.bubbleEmitter.transform.position}";
            }
        }

        public override void Start()
        {
            for (var i = 0; i < manager.hand.transform.childCount; i++)
            {
                manager.hand.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
            }
            manager.arPlaneManager.detectionMode = PlaneDetectionMode.None;
            needle = GameObject.Find("Needle");
            needle.GetComponent<MeshRenderer>().enabled = true;
            manager.handProcessor.gameOrigin = manager.bubbleEmitter;
            manager.bubbleEmitter.SetActive(true);
        }
    }

    public sealed class LevelComplete : GameState
    {
        public int Score { get; }

        public LevelComplete(int score)
        {
            Score = score;
        }

        public override void Update(GameManager target)
        {
            throw new NotImplementedException();
        }
    }

    private static IEnumerable<T> IterateTrackables<T>(TrackableCollection<T> collection)
    {
        var iterator = collection.GetEnumerator();
        while (iterator.MoveNext())
        {
            yield return iterator.Current;
        }
    }
}

public sealed class LevelSettings
{
}