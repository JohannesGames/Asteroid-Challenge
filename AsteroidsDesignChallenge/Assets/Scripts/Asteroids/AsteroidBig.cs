using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidBig : Asteroid
{
    public Rigidbody rb;
    public List<AsteroidChunk> allAsteroidChunks;
    protected float horizontalRandomValue;
    protected float verticalRandomValue;
    protected Vector3 initialMovementFudge = new Vector3(1, 0, 0);

    void Update()
    {
        if (isActive)
        {
            // if round is over, delete self
            if (!GameManager.gm.inGame) Destroy(gameObject);
        }
    }

    public override void AsteroidInteraction()
    {
        base.AsteroidInteraction();

        // generate a random vector to use for velocity and torque (add movement fudge to avoid moving straight up or down)
        horizontalRandomValue = Random.Range(-1.0f, 1.0f);
        verticalRandomValue = Random.Range(-1.0f, 1.0f);
        randomVector = (Vector3.left * horizontalRandomValue + initialMovementFudge * (horizontalRandomValue / Mathf.Abs(horizontalRandomValue)) + 
            Vector3.forward * verticalRandomValue).normalized;
        // add some rotation
        rb.AddTorque(randomVector * rotationSpeed, ForceMode.VelocityChange);
        randomVector.y = 0;
        // set velocity to a random vector at somewhere between half of max speed and max speed
        rb.velocity = randomVector * Random.Range(maxMoveSpeed / 2, maxMoveSpeed);
    }

    public void RemoveChunk(AsteroidChunk toRemove)
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
                    allAsteroidChunks[i].AsteroidInteraction();
                }
                GameManager.gm.AddPoints(GameManager.PointEvent.asteroidSplit);
                GameManager.gm.wm.RemoveAsteroid(this);
                Destroy(gameObject);
            }
        }
    }
}
