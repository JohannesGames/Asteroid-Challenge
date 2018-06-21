﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    public enum AsteroidSize
    {
        Big,
        Chunk
    }

    [Header("Basic Stats")]

    public AsteroidSize asteroidSize;

    [SerializeField] float maxMoveSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] bool isActive;
    Vector3 randomVector;
    Vector3 rotationVector;
    Vector3 newWorldPos;

    [Header("Asteroid Big")]
    [SerializeField] Rigidbody rb;
    [SerializeField] List<Asteroid> allAsteroidChunks;

    [Header("Asteroid Chunk")]
    [SerializeField] Asteroid mainAsteroid;
    Vector3 moveDirection;

    // Wrapping
    Vector3 screenPos;
    enum WrapRequired
    {
        Top,
        Bottom,
        Left,
        Right
    }

    // called when big asteroid is spawned and when asteroid chunk is shot off
    public void AsteroidHit()
    {
        // set this asteroid to active
        isActive = true;

        if (asteroidSize == AsteroidSize.Big)
        {
            // generate a random vector to use for velocity and torque
            randomVector = (Vector3.left * Random.Range(-1, 1) + Vector3.forward * Random.Range(.1f, .9f)).normalized;
            randomVector.y = 0;
            // set velocity to a random vector at somewhere between half of max speed and max speed
            rb.velocity = randomVector * Random.Range(maxMoveSpeed / 2, maxMoveSpeed);
            // add some rotation
            rb.AddTorque(randomVector * rotationSpeed);
        }
        else if (!transform.parent)
        {
            //  destroy chunk if flying freely
            GameManager.gm.AddPoints(GameManager.PointEvent.chunkDestroyed);
            Destroy(gameObject);
        }
        else AsteroidChunkSetup();
    }

    void AsteroidChunkSetup()
    {
        // generate a random vector to use for velocity and rotation
        randomVector = (Vector3.left * (Random.Range(-1, 1) + .1f) + Vector3.forward * (Random.Range(-1, 1) + .1f)).normalized;
        randomVector.y = 0;
        // not childed to main asteroid any more
        transform.parent = null;
        // tell previous parent that this asteroid is gone
        if (mainAsteroid)
        {
            mainAsteroid.RemoveChunk(this);
            mainAsteroid.rb.AddTorque(mainAsteroid.randomVector + randomVector * 100);
            rotationVector = mainAsteroid.rb.angularVelocity;
        }
        maxMoveSpeed = Random.Range(maxMoveSpeed / 2, maxMoveSpeed);
        rotationSpeed = Random.Range(rotationSpeed / 2, rotationSpeed);
        mainAsteroid = null;
        GameManager.gm.AddPoints(GameManager.PointEvent.chunkHit);
    }

    public void RemoveChunk(Asteroid toRemove)
    {
        if (allAsteroidChunks.Count > 0)
        {
            // remove chunk that was shot
            allAsteroidChunks.Remove(toRemove);

            // check whether any chunks remain
            if (allAsteroidChunks.Count <= 2)
            {
                // if 2 or fewer chunks remain, split them all
                for (int i = allAsteroidChunks.Count - 1; i > 0; i--)
                {
                    allAsteroidChunks[i].AsteroidHit();
                }
                GameManager.gm.AddPoints(GameManager.PointEvent.asteroidSplit);
                Destroy(gameObject);
            }
        }
    }

    void MoveChunk()
    {
        transform.Translate(randomVector * Time.deltaTime * maxMoveSpeed, Space.World);
        transform.Rotate(rotationVector);
    }

    void Update()
    {
        if (isActive)
        {
            // check if asteroid's world position is within screenspace
            IsOnScreen();

            // if chunk, move without rigidbody
            if (asteroidSize == AsteroidSize.Chunk) MoveChunk();

            // if round is over, delete self
            if (!GameManager.gm.inGame) Destroy(gameObject);
        }
    }

    void IsOnScreen()
    {
        screenPos = GameManager.gm.mainCamera.WorldToScreenPoint(transform.position);
        // check x is within screen bounds
        if (screenPos.x < 0)
        {
            WrapObject(WrapRequired.Right);
        }
        if (screenPos.x > GameManager.gm.mainCamera.pixelWidth)
        {
            WrapObject(WrapRequired.Left);
        }
        // check y is within screen bounds
        if (screenPos.y < 0)
        {
            WrapObject(WrapRequired.Top);
        }
        if (screenPos.y > GameManager.gm.mainCamera.pixelHeight)
        {
            WrapObject(WrapRequired.Bottom);
        }
    }

    void WrapObject(WrapRequired wrapReq)
    {
        switch (wrapReq)
        {
            case WrapRequired.Top:
                screenPos = new Vector3(screenPos.x, GameManager.gm.mainCamera.pixelHeight, screenPos.z);
                newWorldPos = GameManager.gm.mainCamera.ScreenToWorldPoint(screenPos);
                newWorldPos.y = 0;
                transform.position = newWorldPos;
                break;
            case WrapRequired.Bottom:
                screenPos = new Vector3(screenPos.x, 0, screenPos.z);
                newWorldPos = GameManager.gm.mainCamera.ScreenToWorldPoint(screenPos);
                newWorldPos.y = 0;
                transform.position = newWorldPos;
                break;
            case WrapRequired.Left:
                screenPos = new Vector3(0, screenPos.y, screenPos.z);
                newWorldPos = GameManager.gm.mainCamera.ScreenToWorldPoint(screenPos);
                newWorldPos.y = 0;
                transform.position = newWorldPos;
                break;
            case WrapRequired.Right:
                screenPos = new Vector3(GameManager.gm.mainCamera.pixelWidth, screenPos.y, screenPos.z);
                newWorldPos = GameManager.gm.mainCamera.ScreenToWorldPoint(screenPos);
                newWorldPos.y = 0;
                transform.position = newWorldPos;
                break;
        }
    }
}