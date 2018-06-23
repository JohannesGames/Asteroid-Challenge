using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WrapObject))]
public class Asteroid : MonoBehaviour
{
    public enum AsteroidSize
    {
        Big,
        Chunk,
        Huge
    }

    [Header("Basic Stats")]

    public AsteroidSize asteroidSize;

    [SerializeField] protected float maxMoveSpeed;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected bool isActive;
    [HideInInspector]
    public Vector3 randomVector;
    protected Vector3 rotationVector;
    Vector3 newWorldPos;
    
    // called when big asteroid is spawned and when asteroid chunk is shot
    public virtual void AsteroidInteraction()
    {
        // set this asteroid to active
        isActive = true;
    }
}