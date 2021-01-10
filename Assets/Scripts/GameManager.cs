using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    private GameState gameState;
    public ARPlaneManager arPlaneManager;
    public ARRaycastManager arRaycastManager;
    public Text textField;
    public float areaThreshold = 1.5f * 1.5f;
    public GameObject bubbleEmitter;

    void Start()
    {
        gameState = new GameState.LearnEnvironment(this);
    }

    // Update is called once per frame
    void Update()
    {
        gameState.Update(this);
        gameState = gameState.GetNext();
    }
}

public abstract class GameState
{
    private GameState()
    {
    }

    public abstract void Update(GameManager target);

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

        public override void Update(GameManager target)
        {
            var mainPlane = Iterate(arPlaneManager).Where(plane => plane.alignment == PlaneAlignment.HorizontalUp)
                .OrderBy(plane => plane.size.magnitude)
                .FirstOrDefault();
            if (mainPlane != null)
            {
                Debug.Log($"Game plane is: {mainPlane.center}, {mainPlane.size.magnitude}, {mainPlane.normal}");
                Debug.DrawRay(mainPlane.center, mainPlane.normal, Color.magenta, 20, true);
                if (mainPlane.size.magnitude > manager.areaThreshold)
                {
                    gamePlane = mainPlane;
                    target.textField.text = "Game plane set!";
                    manager.bubbleEmitter.transform.position = gamePlane.center;
                    manager.bubbleEmitter.SetActive(true);
                }
            }
            else
            {
                target.textField.text = "Scan a plane to play";
            }
        }

        public override GameState GetNext()
        {
            if (gamePlane == null)
            {
                return this;
            }
            else
            {
                return new LocatePlayer(manager);
            }
        }
    }

    public sealed class LocatePlayer : GameState
    {
        private readonly GameManager manager;

        public LocatePlayer(GameManager manager)
        {
            this.manager = manager;
        }

        public override void Update(GameManager target)
        {
            manager.textField.text = "Looking for player";
            Debug.DrawRay(manager.bubbleEmitter.transform.position, Vector3.up);
        }
    }

    public sealed class PlayLevel : GameState
    {
        public LevelSettings LevelSettings { get; }

        public PlayLevel(LevelSettings levelSettings)
        {
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

    static IEnumerable<ARPlane> Iterate(ARPlaneManager planeManager)
    {
        var iterator = planeManager.trackables.GetEnumerator();
        while (iterator.MoveNext())
        {
            yield return iterator.Current;
        }
    }
}

public sealed class LevelSettings
{
}