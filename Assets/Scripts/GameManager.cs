﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Object = System.Object;

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
    public GameObject markerPrefab;

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
            return gamePlane == null ? (GameState) this : new LocatePlayer(manager);
        }
    }

    public sealed class LocatePlayer : GameState
    {
        private readonly GameManager manager;
        private bool canStart = false;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private GameObject marker;

        public LocatePlayer(GameManager manager)
        {
            this.manager = manager;
        }

        public override void Start()
        {
            // manager.debugText.text = "Starting hand-tracking";
        }

        public override void Update(GameManager target)
        {
            manager.textField.text = "Select the point where the player will start";
            var bubbleEmitter = manager.bubbleEmitter;
            Debug.DrawRay(bubbleEmitter.transform.position, Vector3.up);
            // canStart = true;
            if (Input.touchCount == 0)
            {
                return;
            }

            if (manager.arRaycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.Planes))
            {
                target.textField.text = "Game plane set!";
                var arRaycastHit = hits.First(); // can this throw an exception?
                bubbleEmitter.transform.position = arRaycastHit.pose.position;
                bubbleEmitter.transform.SetParent(manager.arSessionOrigin.transform);
                if (marker == null)
                {
                    marker = GameObject.Instantiate(manager.markerPrefab, manager.arSessionOrigin.transform);
                }

                marker.transform.position = arRaycastHit.pose.position;
                // bubbleEmitter.SetActive(true);
            }
        }

        public override GameState GetNext()
        {
            return canStart ? new PlayLevel(new LevelSettings(), manager) : (GameState) this;
        }
    }

    public sealed class PlayLevel : GameState
    {
        private readonly GameManager manager;
        public LevelSettings LevelSettings { get; }

        public PlayLevel(LevelSettings levelSettings, GameManager gameManager)
        {
            this.manager = gameManager;
            this.LevelSettings = levelSettings;
        }

        public override void Update(GameManager target)
        {
            manager.textField.text = "Play";
            var needle = GameObject.Find("Needle");
            if (needle != null)
            {
                needle.GetComponent<MeshRenderer>().enabled = true;
                var position = needle.transform.position;
                manager.debugText.text =
                    $"Needle at: {position}, {(Camera.current.transform.position - position).magnitude}";
            }
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