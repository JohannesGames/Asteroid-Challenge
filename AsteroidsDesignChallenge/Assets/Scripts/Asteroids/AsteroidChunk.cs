using UnityEngine;
using System.Collections;

public class AsteroidChunk : Asteroid
{
    public AsteroidBig mainAsteroid;
    [Tooltip("The amount of rotational force applied to a Big Asteroid upon a hit")]
    [SerializeField] float hitForceBig = 100;
    [Tooltip("The amount of rotational force applied to a Huge Asteroid upon a hit")]
    [SerializeField] float hitForceHuge = 20;
    [SerializeField] ParticleSystem particleTrail;
    Vector3 moveDirection;
    bool setup;

    // Use this for initialization
    void Start()
    {
        mainAsteroid.allAsteroidChunks.Add(this);
    }

    void Update()
    {
        if (isActive)
        {
            // move chunk
            MoveChunk();

            // if round is over, delete self
            if (!GameManager.gm.inGame) Destroy(gameObject);
        }
    }

    public override void AsteroidInteraction()
    {
        base.AsteroidInteraction();

        if (!transform.parent)  // if not part of a big or huge asteroid
        {
            //  destroy chunk if flying freely
            GameManager.gm.AddPoints(GameManager.PointEvent.chunkDestroyed);
            GameManager.gm.wm.RemoveChunk(this);
            Destroy(gameObject);
        }
        else AsteroidChunkSetup();
    }

    void AsteroidChunkSetup()
    {
        if (!setup)
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
                mainAsteroid.rb.AddTorque(mainAsteroid.randomVector + randomVector * (mainAsteroid.asteroidSize == AsteroidSize.Big ? 100 : 50));
                rotationVector = mainAsteroid.rb.angularVelocity;
            }
            maxMoveSpeed = Random.Range(maxMoveSpeed / 2, maxMoveSpeed);
            rotationSpeed = Random.Range(rotationSpeed / 2, rotationSpeed);
            mainAsteroid = null;
            GetComponent<WrapObject>().enabled = true;
            Instantiate(particleTrail, transform);
            // add points and add chunk to wave manager
            GameManager.gm.AddPoints(GameManager.PointEvent.chunkHit);
            GameManager.gm.wm.AddChunk(this);
            setup = true;
        }
    }

    void MoveChunk()
    {
        transform.Translate(randomVector * Time.deltaTime * maxMoveSpeed, Space.World);
        transform.Rotate(rotationVector);
    }
}
