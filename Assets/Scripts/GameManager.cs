using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    private GameState gameState;
    public ARPlaneManager arPlaneManager;
    public ARRaycastManager arRaycastManager;
    public Text textField;
    public Text debugText;
    public float areaThreshold = 1.5f * 1.5f;
    public GameObject bubbleEmitter;
    public GameObject hand;
    public GameObject handTracking;

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
                    target.textField.text = "Game plane set!";
                    manager.bubbleEmitter.transform.position = gamePlane.center;
                    manager.bubbleEmitter.transform.SetParent(gamePlane.transform);
                    manager.bubbleEmitter.SetActive(true);
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

        public LocatePlayer(GameManager manager)
        {
            this.manager = manager;
        }

        public override void Start()
        {
            // manager.handTracking.SetActive(true);
            manager.debugText.text = "Starting hand-tracking";
        }

        public override void Update(GameManager target)
        {
            manager.textField.text = "Looking for player";
            Debug.DrawRay(manager.bubbleEmitter.transform.position, Vector3.up);
            var needle = GameObject.Find("Needle");
            if (needle != null)
            {
                needle.GetComponent<MeshRenderer>().enabled = true;
                manager.debugText.text = $"Needle at: {needle.transform.position}";
            }
        }

        public override GameState GetNext()
        {
            return canStart ? new PlayLevel(new LevelSettings(), manager) : (GameState) this;
        }
    }

    public sealed class PlayLevel : GameState
    {
        private readonly GameManager gameManager;
        public LevelSettings LevelSettings { get; }

        public PlayLevel(LevelSettings levelSettings, GameManager gameManager)
        {
            this.gameManager = gameManager;
            this.LevelSettings = levelSettings;
        }

        public override void Update(GameManager target)
        {
            throw new NotImplementedException();
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