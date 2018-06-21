using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] float lifeTime;
    [SerializeField] float speed;

    Vector3 moveDirection;
    RaycastHit hit;
    bool hitAsteroid;
    Collider[] asteroidsHit;

    public void AmmoStart(Vector3 dir)
    {
        moveDirection = dir;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // move the bullet over time
        transform.Translate(moveDirection * Time.deltaTime * speed);
    }

    private void FixedUpdate()
    {
        asteroidsHit = Physics.OverlapSphere(transform.position, .25f, GameManager.gm.asteroidLayer);
        if (asteroidsHit.Length > 0)
        {
            for (int i = 0; i < asteroidsHit.Length; i++)
            {
                asteroidsHit[i].GetComponent<Asteroid>().AsteroidHit();
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitAsteroid)
        {
            // when ammo hits asteroid
            other.gameObject.GetComponent<Asteroid>().AsteroidHit();
            hitAsteroid = true;
            Destroy(gameObject);
        }
    }
}
